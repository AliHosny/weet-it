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
            mylexicon.getPredicates("death place",20,50); 
           
        }
    }
}
