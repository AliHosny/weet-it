using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GetPredicates_Ported
{
    abstract class LexiconToken
    {
        public string URI { get; set; }
        public string label { get; set; }
        public string QuestionMatch { get; set; }
        public string identifier { get; set; }

        

        /// <summary>
        /// to returns the component of thelexicon token in a simple string 
        /// </summary>
        /// <returns>string containing the components of the lexicon token </returns>
        public abstract string ToSimpleString();
   
    }
}
