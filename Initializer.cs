using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SBD_PROJEKT1
{
    internal class Initializer
    {
        string destination = "";
        string source = "";
        string output = "";
        public Initializer(string source, string destination,string output)
        {
            this.source = source;
            this.destination = destination;
            this.output = output;
        }

        public double LoadDataFromFile()
        {
            uint data;
            double n=0;
            using (StreamReader reader = new StreamReader(File.Open(source, FileMode.OpenOrCreate)))
            {
                using (BinaryWriter writer = new BinaryWriter(File.Open(destination, FileMode.OpenOrCreate)))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        data = UInt32.Parse(line);
                        writer.Write(data);
                        n = n + 0.5;
                    }
                }
            }
            return n;
        }
        public void GenerateData(string source, int n, int max_number)
        {
            File.Delete(source);
            using (StreamWriter writer = new StreamWriter(File.Open(source, FileMode.OpenOrCreate)))
            {
                Random rnd = new Random();
                int random;
                for (int i = 0; i < n; i++)
                {
                    random = rnd.Next(1, max_number);
                    uint r = Convert.ToUInt32(random);
                    random = rnd.Next(1, max_number);
                    uint alpha = Convert.ToUInt32(random);
                    writer.WriteLine(r);
                    writer.WriteLine(alpha);
                }
            }
        }

        public void EnterData(string source, int n)
        {
            File.Delete(source);
            using (StreamWriter writer = new StreamWriter(File.Open(source, FileMode.OpenOrCreate)))
            {
                string val;
                for (int i = 0; i < n; i++)
                {
                    Console.WriteLine("Enter radius: ");
                    val = Console.ReadLine();
                    uint r = Convert.ToUInt32(val);
                    Console.WriteLine("Enter central angle: ");
                    val = Console.ReadLine();
                    uint alpha = Convert.ToUInt32(val);
                    writer.WriteLine(r);
                    writer.WriteLine(alpha);
                }
            }
        }

        public int CountInitialRuns()
        {
            int r = 1;
            uint data1,data2;
            string line;
            double p, lw_p=0;
            using (StreamReader reader = new StreamReader(File.Open(source, FileMode.OpenOrCreate)))
            {
                while(true)
                {
                    line = reader.ReadLine();
                    if (line == null)
                        break;
                    data1 = UInt32.Parse(line);
                    line = reader.ReadLine();
                    data2 = UInt32.Parse(line);

                    p = Convert.ToDouble(data2) / 360 * Math.PI * Math.Pow(Convert.ToDouble(data1), 2);
                    if (p < lw_p)
                        r++;
                    lw_p = p;
                }
                
            }
            return r;
        }
        public void DisplayBeforeSort()
        {
            using (StreamWriter writer = new StreamWriter(File.Open(output, FileMode.OpenOrCreate)))
            {
                writer.Write("Tape3 before sorting:  ");
                using (StreamReader reader = new StreamReader(File.Open(source, FileMode.OpenOrCreate)))
                {
                    uint data1, data2;
                    string line;
                    double p;
                    while (true)
                    {
                        line = reader.ReadLine();
                        if (line == null)
                            break;
                        data1 = UInt32.Parse(line);
                        line = reader.ReadLine();
                        data2 = UInt32.Parse(line);
                        p = Convert.ToDouble(data2) / 360 * Math.PI * Math.Pow(Convert.ToDouble(data1), 2);
                        writer.Write(Math.Round(p, 2) + " ");
                    }
                }
                writer.WriteLine();
            }
        }
        public void DisplayAfterSort()
        {
            using (StreamWriter writer = new StreamWriter(File.Open(output, FileMode.Append)))
            {
                writer.Write("Tape3 after sorting:  ");
                using (BinaryReader reader = new BinaryReader(File.Open(destination, FileMode.OpenOrCreate)))
                {
                    while (reader.BaseStream.Position < reader.BaseStream.Length)
                    {
                        uint radius = reader.ReadUInt32();
                        uint alpha = reader.ReadUInt32();
                        double p = Convert.ToDouble(alpha) / 360 * Math.PI * Math.Pow(Convert.ToDouble(radius), 2);
                        writer.Write(Math.Round(p, 2) + " ");
                    }
                }
                writer.WriteLine();
            }
        }
        public void DeleteRemainingTapes()
        {
            File.Delete("tape1.dat");
            File.Delete("tape2.dat");
            File.Delete("tape3.dat");
        }
    }
}
