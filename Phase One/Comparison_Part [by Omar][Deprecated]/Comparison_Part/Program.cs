using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VDS.RDF;
using VDS.RDF.Parsing;

namespace Comparison_Part
{
    class Program
    {
        static void Main(string[] args)
        {
            List<String> urisStringsToCompare = new List<String>();
            urisStringsToCompare.Add("http://data.linkedmdb.org/resource/film/1008");
            urisStringsToCompare.Add("http://data.linkedmdb.org/resource/film/1");

            Comparison c = new Comparison(urisStringsToCompare);

        /////////////////////////////////////////////////////////////////For Testing
            int i = 0;

            foreach (INode x in c.CommonPredicate_Subject)
            {
                Console.WriteLine(x);

                foreach (ComparisonElement element in c.ComparisonElement)
                {
                    Console.WriteLine('\t' + element.ElementURI);

                    for (int j = 0; j < element.CommonPredicateObject_Subject[i].Count; j++)
                    {
                        Console.WriteLine("\t\t" + element.CommonPredicateObject_Subject[i][j]);
                    }
                }
                i++;
            }

            Console.WriteLine("\n\n\n");

            i = 0;

            foreach (INode x in c.CommonPredicate_Object)
            {
                Console.WriteLine(x);

                foreach (ComparisonElement element in c.ComparisonElement)
                {
                    Console.WriteLine('\t' + element.ElementURI);

                    for (int j = 0; j < element.CommonPredicateSubject_Object[i].Count; j++)
                    {
                        Console.WriteLine("\t\t" + element.CommonPredicateSubject_Object[i][j]);
                    }
                }
                i++;
            }
          
        }
    }
}
