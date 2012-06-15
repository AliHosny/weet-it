using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace NLI
{
    class Program
    {

        static void Main(string[] args)
        {
            util.clearLog();
            util.log("starting .............");
            Lexicon mylexicon = new Lexicon();

            List<LexiconPredicate> predicates = mylexicon.getPredicates("founder", 30, 30);
            foreach (LexiconPredicate predicate in predicates)
            {
                util.log(predicate.ToSimpleString());
            }

            //List<LexiconLiteral> literals = mylexicon.getLiterals("moon", 30, 20);

            //foreach (LexiconLiteral l in literals)
            //{
            //    util.log(l.ToSimpleString());
            //}

            QueryGenerator Q = new QueryGenerator("how much are the assets of microsoft");


        }


    }
}
