using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace longest_common_sequences_dotnet
{
    class Program
    {
        public static string CurrentPath;

        static void Main(string[] args)
        {
            Init();

            //Start the stopwatch
            Stopwatch stopwatch = Stopwatch.StartNew();

            //Prepare the list of files that you are going to work on
            List<string> listOfFiles = PrepareListOfFiles(args);

            //If a valid list of files is found, start processing these files:
            if (listOfFiles != null)
                Tokenize.ProcessFiles(listOfFiles);
            else
                Console.WriteLine("No files to process. The program is going to die.");

            //End Stopwatch
            stopwatch.Stop();
            //Display Elapsed time in Milliseconds
            Console.WriteLine(stopwatch.ElapsedMilliseconds + "Elapsed milliseconds");

        }//end main method

        public static List<string> PrepareListOfFiles(string[] args)
        {
            List<string> listOfFiles = null;

            if (args.Length == 0)
            {
                Console.WriteLine("Nothing was passed from the commandline, going to check the internal directory: SourceFiles");

                //Check if the internal directory exists and has files
                string InternalDirectory = Directory.GetCurrentDirectory() + "\\SourceFiles";

                if (Directory.Exists(InternalDirectory))
                {
                    listOfFiles = Directory.GetFiles(InternalDirectory)?.ToList();
                    Console.WriteLine("{0} files found in the SourceFiles directory.", listOfFiles.Count);
                }
                else
                    Console.WriteLine("Couldn't find anything to parse..");

            }//endif
            else
            {
                string TargetPath = args[0];

                if (TargetPath.Contains(".txt"))
                {
                    //It is one file that has the list of files
                    //Read the contents of the file
                    listOfFiles = File.ReadAllLines(TargetPath).ToList();

                    Console.WriteLine("{0} files found in the list of files.", listOfFiles.Count);

                }//endif: filename.txt
                else
                {
                    //It is the path to files
                    listOfFiles = Directory.GetFiles(args[0])?.ToList();

                    Console.WriteLine("{0} files found in the directory.", listOfFiles.Count);
                }

            }//end else: there are commandline arguments

            return listOfFiles;

        }//end methods

        public static void Init()
        {
            CurrentPath = Directory.GetCurrentDirectory();

            string ReportsDirectory = CurrentPath + "\\CSVReports";

            Console.WriteLine("The CSV Reports Directory is {0}", ReportsDirectory);

            if (!Directory.Exists(ReportsDirectory))
            {
                Directory.CreateDirectory(ReportsDirectory);
            }

        }//end method: Init

    }//end class

}//end namespace
