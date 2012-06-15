using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHunspell;
namespace StemSynonymsTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Hunspell hunspell = new Hunspell("en_us.aff", "en_us.dic");

            

            //The folliwng is the trying of the spell checking
            Console.WriteLine("Trying Spell Checking for the word 'Recommendation'");
            Console.WriteLine(hunspell.Spell("Recommendation"));
            

            //The following is the trying of the suggesstions
            Console.WriteLine("\n\n");
            Console.WriteLine("Trying the suggesstions of the word 'Recommnedatio'");
            List<string> suggesstions = new List<string>();
            suggesstions = hunspell.Suggest("Recommnedatio");
            foreach (string item in suggesstions)
            {
                Console.WriteLine("    --" + item);
            }


            //The following is the trying of analysis of word
            Console.WriteLine("\n\n");
            Console.WriteLine("Analyze the word 'children'");
            List<string> morphs = hunspell.Analyze("children");
            foreach (string morph in morphs)
            {
                Console.WriteLine("Morph is: " + morph);
            }



            //The following is the trying of Stemming
            Console.WriteLine("\n\n");
            Console.WriteLine("Find the word stem of the word 'children'");
            List<string> stems = hunspell.Stem("children");
            foreach (string stem in stems)
            {
                Console.WriteLine("Word Stem is: " + stem);
            }


            //Now for the synonym functions
            Console.WriteLine("\n\n\nThesaurus/Synonym Functions");
            Console.WriteLine("¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯");

            //Creating a new instance of the thesarus 
            MyThes thes = new MyThes("th_en_us_v2.dat");

            //Synonyms for words
            Console.WriteLine("Get the synonyms of the plural word 'children'");
            ThesResult tr = thes.Lookup("how", hunspell);

            if (tr.IsGenerated)
                Console.WriteLine("Generated over stem (The original word form wasn't in the thesaurus)");
            foreach (ThesMeaning meaning in tr.Meanings)
            {
                Console.WriteLine();
                Console.WriteLine("  Meaning: " + meaning.Description);
                foreach (string synonym in meaning.Synonyms)
                {
                    Console.WriteLine("    Synonym: " + synonym);

                }
            }

        }
    }
}
