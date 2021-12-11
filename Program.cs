using System;
namespace SBD_PROJEKT1
{
    class Program
    {
        static void Main(string[] args)
        {

            Initializer init = new Initializer("data.txt", "tape3.dat","output.txt");
            init.DeleteRemainingTapes();
            Console.WriteLine("Type 1 if you want to randomly generate data");
            Console.WriteLine("Type 2 if you want to enter data from keyboard");
            Console.WriteLine("Type 3 if you want to load data from txt file");
            string opt = Console.ReadLine();
            if(opt == "1")
            {
                Console.Clear();
                Console.WriteLine("How many records do you want to generate?");
                string n = Console.ReadLine();
                //Console.WriteLine("How big you want the records to be?");
                string max = "50";
                init.GenerateData("data.txt", Convert.ToInt32(n), Convert.ToInt32(max));
            }
            else if(opt == "2")
            {
                Console.Clear();
                Console.WriteLine("How many records do you want to enter?");
                string n = Console.ReadLine();
                init.EnterData("data.txt", Convert.ToInt32(n));
            }

            Console.WriteLine();
            Console.WriteLine("Enter size of disk page");
            int disk_page_size = Convert.ToInt32(Console.ReadLine());
            int record_size = 8;
            int b = disk_page_size / record_size;
            int N = Convert.ToInt32(init.LoadDataFromFile());
            int r = init.CountInitialRuns();
            bool sorted, display;
            Tape tape3 = new Tape("tape3.dat",record_size, disk_page_size);
            Tape tape1 = new Tape("tape1.dat", record_size, disk_page_size);
            Tape tape2 = new Tape("tape2.dat", record_size, disk_page_size);
            Buffer buffer = new Buffer(disk_page_size/record_size);

            Console.WriteLine("Initial file to be sorted");
            tape3.DisplayResult();
            init.DisplayBeforeSort();
            Console.WriteLine();
            Console.WriteLine("Do you want to display tapes after each phase?");
            string display_opt = Console.ReadLine();
            if (display_opt == "yes")
                display = true;
            else
                display = false;
            int phase_counter = 0;
            
            double max_rw = Convert.ToInt32(Math.Ceiling((4 * N * Math.Log2(N))/b));
            double avg_rw = Convert.ToInt32(Math.Ceiling((4 * N * Math.Log2(r)) / b));
            double avg_phase = Convert.ToInt32(Math.Ceiling(Math.Log2(r)));
            double max_phase = Convert.ToInt32(Math.Ceiling(Math.Log2(N)));
            Console.Clear();
            Console.WriteLine("Records are displayed as circular sectors");
            Console.WriteLine("Records before sort: ");
            tape3.DisplayResult();
            Console.WriteLine();
            while (true)
            {
                phase_counter++;
                if (display)
                    Console.WriteLine("Phase: " + phase_counter);
                
                buffer.ClearBuffer();

                buffer.Distribiute(tape1,tape2,tape3, display);
                
                buffer.ClearBuffer();

                sorted = buffer.Merge(tape1, tape2, tape3, display);
                if (sorted)
                {
                    Console.WriteLine();
                    break;
                } 
            }
            Console.WriteLine("Records after sort: ");
            tape3.DisplayResult();
            Console.WriteLine();
            Console.WriteLine("Initual numer of runs " + r);
            Console.WriteLine("Buffer size: " + b);
            Console.WriteLine("Number of records: " + N);
            Console.WriteLine("Theoretical number of max read/writes: " + max_rw);
            Console.WriteLine("Theoretical number of read/writes: " + avg_rw);
            Console.WriteLine();
            int reads_and_writes = tape1.read_counter + tape1.write_counter + tape2.read_counter + tape2.write_counter + tape3.read_counter + tape3.write_counter;
            Console.WriteLine("File sorted");
            Console.WriteLine("Number o Read/Writes: " + reads_and_writes);
            Console.WriteLine("Number of phases: " + phase_counter);
            Console.WriteLine("Theoretical numer of max phases: " + max_phase);
            Console.WriteLine("Theoretical numer of phases: " + avg_phase);
            init.DisplayAfterSort();
        }
    }
}
