using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GetPredicates_Ported
{
    class LexiconPredicate : LexiconToken 
    {

        public List<string> domains { get; set; }
        public List<string> ranges { get; set; }
        public string type { get; set; }
        public int score { get; set; }

        public LexiconPredicate()
        {
            domains = new List<string>();
            ranges = new List<string>();
        }

        /// <summary>
        /// returns a string the descriping the predicate 
        /// </summary>
        /// <returns>predicate containning all the elements of the predicate</returns>
        public override string ToSimpleString()
        {

            string s = "";

            s += "URI: " + this.URI + "\n";
            s += "label: " + this.label + "\n";
            s += "QuestionMatch : " + this.QuestionMatch + "\n";
            s += "identifier : " + this.identifier + "\n";
            s += "domains: \n";
            if (this.domains.Count() != 0)
            {
                foreach (string domain in this.domains)
                    s += domain + " \n ";
            }
            s += "ranges: \n";
            if (this.ranges.Count() != 0)
            {
                foreach (string range in this.ranges)
                    s += range + "\n";
            }
            s += "score :" + this.score + "\n";
            s += "-------------------------------------\n";
            return s;
        }

    }
}
