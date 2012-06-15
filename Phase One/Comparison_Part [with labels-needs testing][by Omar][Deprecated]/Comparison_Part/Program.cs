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
            urisStringsToCompare.Add("http://dbpedia.org/page/Resource_Description_Framework");
            urisStringsToCompare.Add("http://dbpedia.org/page/Resource_Description_Framework");

            Comparison c = new Comparison(urisStringsToCompare, "http://dbpedia.org/sparql");
        
            
            /////////////////////////////////////////////////////////////////For Testing


            int i = 0;

            foreach (INode x in c.CommonPredicate_Subject)
            {
                Console.WriteLine(x);

                foreach (ComparisonElement element in c.ComparisonElement)
                {
                    Console.WriteLine('\t' + element.ElementURI);

                    for (int j = 0; j < element.CommonPredicateObject_String[i].Count; j++)
                    {
                        Console.WriteLine("\t\t" + element.CommonPredicateObject_String[i][j]);
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

                    for (int j = 0; j < element.CommonPredicateSubject_String[i].Count; j++)
                    {
                        Console.WriteLine("\t\t" + element.CommonPredicateSubject_String[i][j]);
                    }
                }
                i++;
            }
        }           
    }
}
