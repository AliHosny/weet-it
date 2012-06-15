using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GetPredicates_Ported
{
    class LexiconPredicate
    {
        public string URI { get; set; }
        public string label { get; set; } 
        
        public string QuestionMatch { get; set; }
        
        public string identifier { get; set; }
        public string domain { get; set; }
        public string range { get; set; }
        public string type { get; set; }

        public int score { get; set; }

        public LexiconPredicate()
        {
            
        }
    }
}
