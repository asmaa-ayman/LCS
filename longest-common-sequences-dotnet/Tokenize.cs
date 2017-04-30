using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;
using System.Text.RegularExpressions;

namespace longest_common_sequences_dotnet
{
    static class Tokenize
    {

        /// <summary>
        /// Process the files:
        /// 1- Reading all files O(n)
        /// 2- Constructing the subsequences O(n)
        /// 3- Looping on the subsequences to count their occurrences and score O(n^2)
        /// 
        /// Read files and in each file: read each line separately
        /// </summary>
        public static void ProcessFiles(List<string> ListOfFiles)
        {
            ArrayList AllSubsequences = new ArrayList();
            ArrayList LcsCandidates = new ArrayList();
            List<LcsCandidate> candidatesList = new List<LcsCandidate>();

            List<string> ContentsOfAllFiles = new List<string>();

            // 1- Read all the files and put these in a main storage
            foreach (string file in ListOfFiles)
            {
                //This one is working on the files line by line - does not deal with sequences that cross lines
                ContentsOfAllFiles.AddRange(File.ReadAllLines(file));
                ContentsOfAllFiles = RemoveBlankEntries(ContentsOfAllFiles);

            }//end foreach: 1- Reading the files to a list

            // 2- Constructing the subsequences
            foreach (string lineOfCode in ContentsOfAllFiles)
            {
                string[] parts = Regex.Split(lineOfCode, @"([-/.+(){}=!?,;:<>#%&|^~@$\\[\\])\s*|\s+");
                parts = RemoveBlankEntries(parts);

                //Generate all possible subsequqnces of this line of code
                ArrayList GeneratedSubsequences = GenerateAllPossibleSubsequences(parts.ToList());

                //Loop on the recently generated subsequences to check the LCS Candidates
                foreach (List<string> subseq in GeneratedSubsequences)
                {
                    bool isPresent = false;

                    //Loop on all distinct subsequences to check if this subsequence exists among the LCS Cadidates or not
                    foreach (LcsCandidate existing in candidatesList)
                    {
                        //if this sequence exists in the candidates: increment its occurrence times
                        if (existing.tokens.SequenceEqual(subseq))
                        {
                            //This subsequence is present in the LCS Candidates, so do not add it, just increment its occurrence
                            isPresent = true;
                            existing.occurrences += 1;
                            break; //no need to continue as this list has distinct candidates

                        }//endif

                    }//endforeach: looping on existing candidates

                    //If this is a new candidate with at least two tokens --to avoid adding single literals: Add it!
                    if (!isPresent && subseq.Count > 1)
                    {
                        // A new candidate is here to be added: a new distinct subsequene
                        LcsCandidate newLcsCandidate = new LcsCandidate() { tokens = subseq, occurrences = 1 };
                        candidatesList.Add(newLcsCandidate);

                    }//endif

                }//endforeach: looping on all the recently generated subsequences.

                //not used till now
                //AllSubsequences.AddRange(GeneratedSubsequences);

            }//end foreach: 2- Constructing the Subsequences


            //Filter Candidates: remove single occurrences

            //Order by occurrences
            var q = candidatesList.OrderByDescending(c => c.occurrences);
            List<LcsCandidate> filteredCandidates = q.ToList();

            //The subsequence should repeat at least twice and the tokens should be at least two
            int index = filteredCandidates.FindIndex(s => s.occurrences == 1);
            if (index != -1)
                filteredCandidates.RemoveRange(index, filteredCandidates.Count - index);

            foreach (LcsCandidate candidate in filteredCandidates)
            {
                candidate.CalculateScore();
                candidate.ReconstructSourceCode();
            }

            //Order according to the tokens count
            List<LcsCandidate> longestSharedCandidates = filteredCandidates.OrderByDescending(s => s.score).ThenByDescending(c => c.tokens.Count).ToList();

            //after your loop            
            WriteCsvReport(longestSharedCandidates);

            Console.WriteLine("The End!");

        }//end method

        public static ArrayList GenerateAllPossibleSubsequences(List<string> sequenceOfLiterals)
        {
            ArrayList allPossibleSubsequences = new ArrayList();

            if (sequenceOfLiterals != null && sequenceOfLiterals.Count > 0)
            {
                //Construct the subsequences
                //Having the sequence of individual literals, we are going to construct mini-subsequences
                //each subsequence will contain a different number of literals:
                //minimum subsequence size is 1, as we don't have blank lines
                //maximum subsequence size is the count of all literals in the line of code/code segment

                //loop on all subsequences
                for (int count = 1; count <= sequenceOfLiterals.Count; count++)
                {
                    //make a list of size equal to the new subsequence
                    List<string> newSubsequence = new List<string>(count);

                    //construct the subsequences
                    for (int i = 0; i < count; i++)
                    {
                        //Add the literals to the newly created subsequence until it is size is reached
                        newSubsequence.Add(sequenceOfLiterals[i]);
                    }

                    //add the newly generated subsequence to the big list
                    allPossibleSubsequences.Add(newSubsequence);

                }//endfor

            }//endif: at least one sequence is out there

            return allPossibleSubsequences;

        }//end method GenerateAllPossibleSubsequences

        static List<string> RemoveBlankEntries(List<string> ListOfStrings)
        {
            ListOfStrings = ListOfStrings.Where(x => !string.IsNullOrEmpty(x)).ToList();

            return ListOfStrings;

        }//end method

        static string[] RemoveBlankEntries(string[] arrayOfStrings)
        {
            string[] cleanArray = arrayOfStrings.Where(x => !string.IsNullOrEmpty(x)).ToArray();

            return cleanArray;

        }//end method

        public static void WriteCsvReport(List<LcsCandidate> filteredCandidates)
        {
            string path = Program.CurrentPath + "\\CSVReports";

            //prepare for the CSV Report
            var csv = new StringBuilder();

            foreach (LcsCandidate candidate in filteredCandidates)
            {
                //For example CSV columns are: CSV: score,tokens,count,"sourcecode"
                var newLine = $"score:{candidate.score},tokens:{candidate.tokens.Count},occurrences:{candidate.occurrences},seq:[{candidate.sourceCode}]{Environment.NewLine}";
                csv.AppendLine(newLine);

            }
            // Create a file name for the file you want to create. 
            string fileName = "LCS-" + Guid.NewGuid().ToString() + ".csv";

            // Use Combine again to add the file name to the path.
            path = Path.Combine(path, fileName);

            File.WriteAllText(path, csv.ToString());

        }//end method


    }//end class
}
