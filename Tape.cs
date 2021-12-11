using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SBD_PROJEKT1
{
    class Tape
    {
        public string filename;
        public long current_read_line = 0;
        public long current_write_line = 0;
        public int read_counter = 0;
        public int write_counter = 0;
        public long reader_filesize;
        public long reader_position;
        public long writer_filesize = 1;
        public long writer_position = 0;
        int record_size = 0;
        int size = 0;
        public Tape(string filename, int record_size,int size)
        {
            this.filename = filename;
            this.record_size = record_size;
            this.size = size;   
            reader_filesize = 1;
            reader_position = 0;
        }
        public long GetFileLenght()
        {
            return this.reader_filesize;
        }
        public long GetCurrentPosition()
        {
            return this.reader_position;
        }

        public Tuple<uint, uint>[]  Read()
        {
            uint record1 = 0;
            uint record2 = 0;
            int buffer_size = 0;
            buffer_size = size / record_size;
            
            Tuple<uint, uint>[] buffer = new Tuple<uint, uint>[buffer_size];
            using (BinaryReader reader = new BinaryReader(File.Open(filename, FileMode.OpenOrCreate)))
            {
                this.reader_filesize = reader.BaseStream.Length;
                reader.BaseStream.Position = this.reader_position;
                for (int i = 0; i < buffer.Length; i++)
                {
                    if (reader.BaseStream.Position + 8 > reader.BaseStream.Length)
                    {
                        this.reader_position = reader.BaseStream.Position;
                        read_counter++;
                        return buffer;
                    }
                    record1 = reader.ReadUInt32();
                    record2 = reader.ReadUInt32();
                    buffer[i] = new Tuple<uint, uint>(record1, record2);
                }
                this.reader_position = reader.BaseStream.Position;
            }
            read_counter++;
            return buffer;
        }

        public void Write(Tuple<uint, uint>[] buffer)
        {
            using (BinaryWriter writer = new BinaryWriter(File.Open(filename, FileMode.OpenOrCreate)))
            {
                //writer_filesize = writer.BaseStream.Length;
                writer.BaseStream.Position = writer_position;
                for (int i = 0; i < buffer.Length; i++)
                {
                    writer.Write(buffer[i].Item1);
                    writer.Write(buffer[i].Item2);
                    
                }
                writer_position = writer.BaseStream.Position;
            }
            write_counter++;
        }
        public void ClearTape()
        {
            File.WriteAllText(filename, string.Empty);
            reader_filesize = 1;
            reader_position = 0;
            writer_filesize = 1;
            writer_position = 0;
        }
        public bool IsEmpty()
        {
            if (new FileInfo(filename).Length == 0)
            {
                return true;
            }
            return false;
        }
        public void DisplayResult()
        {
            using (BinaryReader reader = new BinaryReader(File.Open(filename, FileMode.OpenOrCreate)))
            {
                while(reader.BaseStream.Position < reader.BaseStream.Length)
                {
                    uint radius = reader.ReadUInt32();
                    uint alpha = reader.ReadUInt32();
                    double p = Convert.ToDouble(alpha) / 360 * Math.PI * Math.Pow(Convert.ToDouble(radius), 2);
                    Console.Write(Math.Round(p,2) + " ");
                }
                Console.WriteLine();
            }
        }
        public bool TryRead()
        {
            long n=0;
            using (BinaryReader reader = new BinaryReader(File.Open(filename, FileMode.OpenOrCreate)))
            {
                n = reader.BaseStream.Length;
            }
            if (n == 0)
                return false;
            else
                return true;
        }
    }
}
