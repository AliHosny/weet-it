using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LexiconLiteral
{
    class Program
    {
        static void Main(string[] args)
        {
            LexiconLiteral ll = new LexiconLiteral();
            List<string> output = new List<string>();
            output = ll.getPermutations("location of los anglos city");

            foreach (var item in output)
            {
                Console.WriteLine(item);
            }



        }
    }
}
