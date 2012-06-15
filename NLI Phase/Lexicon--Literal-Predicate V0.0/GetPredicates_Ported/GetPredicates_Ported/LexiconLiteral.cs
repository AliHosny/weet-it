using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GetPredicates_Ported
{
    class LexiconLiteral : LexiconToken 
    {

        public string typeOfOwner { get; set; }
        public string predicate { get ; set; } 

        public int score { get; set; }


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
            s += "typeOfOwner : " + this.typeOfOwner + "\n";
            s += "predicate : " + this.predicate+ "\n";
            s += "score :" + this.score + "\n";
            s += "-------------------------------------\n";
            return s;
        }

    }
}
