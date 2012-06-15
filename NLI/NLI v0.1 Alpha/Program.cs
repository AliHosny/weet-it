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

            //List<LexiconPredicate> predicates = mylexicon.getPredicates("child", 30, 30);
            //foreach (LexiconPredicate predicate in predicates)
            //{
            //    util.log(predicate.ToSimpleString());
            //}

            //List<LexiconLiteral> literals = mylexicon.getLiterals("Inception", 25, 35);

            //foreach (LexiconLiteral l in literals)
            //{
            //    util.log(l.ToSimpleString());
            //}

            answerGenerator answerGenerator = new answerGenerator();

            List<QueryBucket> queries = answerGenerator.generateQueries("could you please tell me how many child does hosni mubarak has ?");

            List<questionAnswer> answers = answerGenerator.executeQueries(queries);
           
        }


    }
}
