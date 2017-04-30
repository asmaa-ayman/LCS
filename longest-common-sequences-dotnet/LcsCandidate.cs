using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace longest_common_sequences_dotnet
{
    public class LcsCandidate
    {
        public List<string> tokens { get; set; }
        public int occurrences { get; set; }
        public double score { get; set; }
        public string sourceCode { get; set; }

        public void ReconstructSourceCode()
        {
            foreach (string token in tokens)
            {
                sourceCode += "\"" + token + "\"" + ',';
            }

            int index = sourceCode.LastIndexOf(',');
            sourceCode = sourceCode.Remove(index, 1);
        }

        public void CalculateScore()
        {
            //score = log2(tokens) * log2(count) --tokens is the number of elements/tokens in the subsequence here
            score = (Math.Log(tokens.Count) / Math.Log(2)) * (Math.Log(occurrences) / Math.Log(2));
            score = Math.Round(score, 2);

        }//endmethod: CalculateScore

    }//end class: LCSCandidate
}
