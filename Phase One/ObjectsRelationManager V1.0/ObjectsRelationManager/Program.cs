using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ObjectsRelationManager
{
    class Program
    {
        static void Main(string[] args)
        {

            ObjectsRelationManager relManager = new ObjectsRelationManager();
            relManager.startConnection();
            relManager.generateQueries("http://dbpedia.org/resource/Inception", "http://dbpedia.org/resource/Batman");

            
            while (relManager.IsEndOfResults != true)
            {
                Console.WriteLine(relManager.getNextResult());

                //just to go forward
                ConsoleKeyInfo s=new ConsoleKeyInfo();
                s=Console.ReadKey();
                if (s.KeyChar =='a')
                    break;
                
            }
            relManager.closeConnection();


        }
    }
}
