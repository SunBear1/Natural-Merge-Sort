using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SBD_PROJEKT1
{
    class Buffer
    {
        public List<Tuple<uint, uint>> t1 = new List<Tuple<uint, uint>>();
        public List<Tuple<uint, uint>> t2 = new List<Tuple<uint, uint>>();
        public List<Tuple<uint, uint>> t3 = new List<Tuple<uint, uint>>();
        public Tuple<uint, uint> LW = null; //Last Written element
        public int LWT = 1; //Tape that was last written to
        public int size = 0;
        public bool t1_empty = false;
        public bool t2_empty = false;
        public bool t3_empty = false;
        public Buffer(int size)
        {
            this.size = size;
        }

        public List<Tuple<uint, uint>> FillBuffer(Tuple<uint, uint>[] buffer)
        {
            List<Tuple<uint, uint>>  tape = new List<Tuple<uint, uint>>(buffer);
            int n = tape.Count;
            for (int i = 0; i < n; i++)
            {
                if (tape[tape.Count - 1] == null)
                {
                    tape.RemoveAt(tape.Count - 1);
                }
            }
            return tape;
        }
        public Tuple<uint, uint> GetRecord(int tape_number,Tape tape)
        {
            if (tape_number == 1)
            {
                if (t1.Count == 0)
                {
                    if(tape.reader_position < tape.reader_filesize && tape.TryRead())
                        t1 = FillBuffer(tape.Read()); //read more
                    else //i cant read more
                    {
                        t1_empty = true; //t1 empty
                        return null;
                    }
                }
                var tmp = t1[0];
                t1.RemoveAt(0);
                return tmp;

            }
            else if (tape_number == 2)
            {
                if (t2.Count == 0)
                {
                    if (tape.reader_position < tape.reader_filesize && tape.TryRead()) // can i read more?
                        t2 = FillBuffer(tape.Read()); //read more
                    else //i cant read more
                    {
                        t2_empty = true; //t2 empty
                        return null;
                    }
                }
                var tmp = t2[0];
                t2.RemoveAt(0);
                return tmp;
            }
            else 
            {
                if (t3.Count == 0)
                {
                    if (tape.reader_position < tape.reader_filesize) // can i read more?
                        t3 = FillBuffer(tape.Read()); //read more
                    else //i cant read more
                    {
                        t3_empty = true; //t2 empty
                        return null;
                    }
                }
                var tmp = t3[0];
                t3.RemoveAt(0);
                return tmp;
            }
        }
        public void SetRecord(int tape_number, Tuple<uint, uint> record, Tape tape)
        {
            if (tape_number == 1)
            {
                if (t1.Count == this.size)//if buffer is full
                {
                    tape.Write(t1.ToArray());//flush buffer to file
                    t1.Clear();//clear buffer
                }
                t1.Add(record);
                
            }
            else if(tape_number == 2)
            {
                if (t2.Count == this.size)//if buffer is full
                {
                    tape.Write(t2.ToArray());//flush buffer to file
                    t2.Clear();//clear buffer
                }
                t2.Add(record);
            }
            else
            {
                if (t3.Count == this.size)//if buffer is full
                {
                    tape.Write(t3.ToArray());//flush buffer to file
                    t3.Clear();//clear buffer
                }
                t3.Add(record);
            }
        }
        public void Distribiute(Tape tape1, Tape tape2, Tape tape3, bool display)
        {
            Tuple<uint, uint> current; //Record that is being distributed at the moment
            int t1_write_counter = 0;
            int t2_write_counter = 0;
            while (true)
            {
                current = GetRecord(3, tape3);
                if (t3_empty)
                {
                    if (!t1_empty)
                        tape1.Write(t1.ToArray());
                    if (!t2_empty)
                        tape2.Write(t2.ToArray());
                    t3_empty = false;
                    break;
                }
                    
                if (Greater(current, LW) && LWT == 1)
                {
                    SetRecord(1, current, tape1);
                    LWT = 1;
                    t1_write_counter++;
                }
                else if (Greater(current, LW) && LWT == 2)
                {
                    SetRecord(2, current, tape2);
                    LWT = 2;
                    t2_write_counter++;
                }
                else if (Greater(LW,current) && LWT == 1)
                {
                    SetRecord(2, current, tape2);
                    LWT = 2;
                    t2_write_counter++;
                }
                else if (Greater(LW, current) && LWT == 2)
                {
                    SetRecord(1, current, tape1);
                    LWT = 1;
                    t1_write_counter++;
                }
                LW = current;
            }
            //display and clean
            if(display)
            {
                Console.Write("T1:");
                tape1.DisplayResult();
                Console.Write("T2:");
                tape2.DisplayResult();
            }
            tape3.ClearTape();
        }
        public bool Merge(Tape tape1,Tape tape2, Tape tape3, bool display)
        {
            //int n = t1.Count + t2.Count;
            //LevelBuffers();
            bool sorted = true;
            Tuple<uint, uint> a = GetRecord(1, tape1);
            Tuple<uint, uint> b = GetRecord(2, tape2);
            while (true)
            {
                if (t1_empty && t2_empty && a==null && b==null)//if t1 and t2 is empty
                {
                    tape3.Write(t3.ToArray()); //flush remaining records
                    t1_empty = false;
                    t2_empty = false;
                    break;//stop
                }

                if (Greater(b, LW) && Greater(a, LW))
                {
                    if(Greater(a, b))
                    {
                        if (b == null)
                        {
                            if(Greater(LW, a) && !Equal(LW,a))
                                sorted = false;
                            SetRecord(3, a, tape3);
                            LW = a;
                            a = GetRecord(1, tape1);
                        }
                        else
                        {
                            if (Greater(LW, b) && !Equal(LW, b))
                                sorted = false;
                            SetRecord(3, b, tape3);
                            LW = b;
                            b = GetRecord(2, tape2);
                        }
                    }
                    else
                    {
                        if (a == null)
                        {
                            if (Greater(LW, b) && !Equal(LW, b))
                                sorted = false;
                            SetRecord(3, b, tape3);
                            LW = b;
                            b = GetRecord(2, tape2);
                        }
                        else
                        {
                            if (Greater(LW, a) && !Equal(LW, a))
                                sorted = false;
                            SetRecord(3, a, tape3);
                            LW = a;
                            a = GetRecord(1, tape1);
                        }
                    }
                    
                }
                else if (Greater(a, b) && Greater(a, LW))
                {
                    if (Greater(LW, a) && !Equal(LW, a))
                        sorted = false;
                    SetRecord(3, a, tape3);
                    LW = a;
                    a = GetRecord(1, tape1);
                }
                else if (Greater(b, a) && Greater(b, LW))
                {
                    if (Greater(LW, b) && !Equal(LW, b))
                        sorted = false;
                    SetRecord(3, b, tape3);
                    LW = b;
                    b = GetRecord(2, tape2);
                }
                else if(Greater(LW,b) && Greater(LW,a))
                {
                    if(Greater(a,b))
                    {
                        if(b == null)
                        {
                            if (Greater(LW, a) && !Equal(LW, a))
                                sorted = false;
                            SetRecord(3, a, tape3);
                            LW = a;
                            a = GetRecord(1, tape1);
                        }
                        else
                        {
                            if (Greater(LW, b) && !Equal(LW, b))
                                sorted = false;
                            SetRecord(3, b, tape3);
                            LW = b;
                            b = GetRecord(2, tape2);
                        }
                      
                    }
                    else
                    {
                        if(a == null)
                        {
                            if (Greater(LW, b) && !Equal(LW, b))
                                sorted = false;
                            SetRecord(3, b, tape3);
                            LW = b;
                            b = GetRecord(2, tape2);
                        }
                        else
                        {
                            if (Greater(LW, a) && !Equal(LW, a))
                                sorted = false;
                            SetRecord(3, a, tape3);
                            LW = a;
                            a = GetRecord(1, tape1);
                        }
                    }
                }
            }
            if(display)
            {
                Console.Write("T3:");
                tape3.DisplayResult();
                Console.WriteLine();
            }
            tape1.ClearTape();
            tape2.ClearTape();
            return sorted;
        }

        public bool Greater(Tuple<uint, uint> r1, Tuple<uint, uint> r2)
        {
            if (r2 == null)
                return true;
            if (r1 == null)
                return false;
            double p1 = Convert.ToDouble(r1.Item2) / 360 * Math.PI * Math.Pow(Convert.ToDouble(r1.Item1), 2);
            double p2 = Convert.ToDouble(r2.Item2) / 360 * Math.PI * Math.Pow(Convert.ToDouble(r2.Item1), 2);
            if (p1 >= p2)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool Equal(Tuple<uint, uint> r1, Tuple<uint, uint> r2)
        {
            double p1 = Convert.ToDouble(r1.Item2) / 360 * Math.PI * Math.Pow(Convert.ToDouble(r1.Item1), 2);
            double p2 = Convert.ToDouble(r2.Item2) / 360 * Math.PI * Math.Pow(Convert.ToDouble(r2.Item1), 2);
            if (p1 == p2)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public void ClearBuffer()
        {
            t1.Clear();
            t2.Clear();
            t3.Clear();
            LW = null;
        }
        public void LevelBuffers()
        {
            if(t1.Count < t2.Count)
            {
                while(t1.Count != t2.Count)
                {
                    t1.Add(null);
                }
            }
            else
            {
                while (t1.Count != t2.Count)
                {
                    t2.Add(null);
                }
            }
        }
    }
}
