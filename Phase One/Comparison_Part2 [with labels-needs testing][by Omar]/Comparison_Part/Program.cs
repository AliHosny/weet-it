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
            urisStringsToCompare.Add("http://dbpedia.org/resource/Led_Zeppelin_III");
            urisStringsToCompare.Add("http://dbpedia.org/resource/Like_a_Virgin");

            Comparison c = new Comparison(urisStringsToCompare, "http://dbpedia.org/sparql");
        
            
            /////////////////////////////////////////////////////////////////For Testing


            int i = 0;

            foreach (string x in c.CommonPredicate_SubjectLabel)
            {
                Console.WriteLine(x);

                foreach (ComparisonElement element in c.ComparisonElement)
                {
                    Console.WriteLine('\t' + element.ElementLabel);

                    for (int j = 0; j < element.CommonPredicateObject_String[i].Count; j++)
                    {
                        Console.WriteLine("\t\t" + element.CommonPredicateObject_String[i][j]);
                    }
                }
                i++;
            }

            Console.WriteLine("\n\n\n");

            i = 0;

            foreach (string x in c.CommonPredicate_ObjectLabel)
            {
                Console.WriteLine(x);

                foreach (ComparisonElement element in c.ComparisonElement)
                {
                    Console.WriteLine('\t' + element.ElementLabel);

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
