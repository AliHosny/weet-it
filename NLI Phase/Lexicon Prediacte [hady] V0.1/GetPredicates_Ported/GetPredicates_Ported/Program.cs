using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace GetPredicates_Ported
{
    class Program
    {

        static void Main(string[] args)
        {
            util.clearLog();
            util.log("starting .............");
            Lexicon mylexicon = new Lexicon();
            List<LexiconPredicate> predicates = mylexicon.getPredicates("birth Place of barack obama", 20,30);

          foreach (LexiconPredicate predicate in predicates)
          {
              util.log(predicate.ToString());
          }
        }
    }
}
