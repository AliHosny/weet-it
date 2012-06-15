using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GetPredicates_Ported
{
    class LexiconLiteral : LexiconToken 
    {
        //public string URI { get; set; }
        //public string label { get; set; }
        //public string QuestionMatch { get; set; }
        //public string identifier { get; set; }
        
        public List<string> typeOfOwner { get; set; }
        public string predicate { get ; set; } 
        public int score { get; set; }

        #region constructors
        public LexiconLiteral()
        {
            typeOfOwner = new List<string>(); 

        }
        public LexiconLiteral(string URI , string label , string QuestionMatch  , string typeOfOwner)
        {
            this.URI = URI;
            this.label = label;
            this.QuestionMatch = QuestionMatch;
            List<string> typeOfOwnerList = new List<string>();
            typeOfOwnerList.Add(typeOfOwner);
            this.typeOfOwner = typeOfOwnerList; 
        }
        public LexiconLiteral(string URI, string label, string QuestionMatch, List<string> typeOfOwnerList)
        {
            this.URI = URI;
            this.label = label;
            this.QuestionMatch = QuestionMatch;
            this.typeOfOwner = typeOfOwnerList;
        }
        #endregion 

        /// <summary>
        /// returns a string the descriping the Literal 
        /// </summary>
        /// <returns>predicate containning all the elements of the Literal</returns>
        public override string ToSimpleString()
        {
            string s = "";

            s += "URI: " + this.URI + "\n";
            s += "label: " + this.label + "\n";
            s += "QuestionMatch : " + this.QuestionMatch + "\n";
            s += "identifier : " + this.identifier + "\n";
            s += "typeOfOwner : " + string.Join("\n" , this.typeOfOwner.ToArray());
            s += "predicate : " + this.predicate+ "\n";
            s += "score :" + this.score + "\n";
            s += "-------------------------------------\n";
            return s;
        }

    }
}
