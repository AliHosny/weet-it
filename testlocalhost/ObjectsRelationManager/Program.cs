using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VDS.RDF;
using VDS.RDF.Query;
using VDS.RDF.Storage;
using VDS.RDF.Parsing;
namespace ObjectsRelationManager
{
    class Program
    {
        static void Main(string[] args)
        {
            //BaseEndpoint endpoint=new BaseEndpoint(new Uri("http://192.168.33.2:8890/"));
            //SparqlRemoteEndpoint endpoint = new SparqlRemoteEndpoint(new Uri("http://192.168.33.2:8890/"));
            //SparqlResultSet result=new SparqlResultSet();
            //result = endpoint.QueryWithResultSet("select * where{<http://dbpedia.org/resource/Inception> ?x ?y}");
            //int x;

            //SparqlConnector s22 = new SparqlConnector(new Uri("http://192.168.33.2:8890"));
            //s22.Query("select * where{<http://dbpedia.org/resource/Inception> ?x ?y}");

            //VirtuosoManager manager = new VirtuosoManager("http://192.168.33.2:8890", 1111, "DB", "dba", "dba");
            //Object ali=manager.Query("sparql Select * WHERE { < http://dbpedia.org/resource/Egypt > ?p1 < http://dbpedia.org/resource/Syria > }LIMIT 5");
            
            //Graph g = new Graph();
            //UriLoader.Load(g, new Uri("http://localhost:8890/resource/Barack_Obama"));
            //foreach (Triple t in g.Triples)
            //{
            //    Console.WriteLine(t.ToString());
            //}

            //QueryProcessor("http://graphs.org/resource/Inception");


            ObjectsRelationManager relManager = new ObjectsRelationManager();
            relManager.startConnection();
            relManager.generateQueries("http://dbpedia.org/resource/Egypt", "http://dbpedia.org/resource/Syria");


            while (relManager.IsEndOfResults != true)
            {
                Console.WriteLine(relManager.getNextResult());



                //just to go forward
                ConsoleKeyInfo s = new ConsoleKeyInfo();
                s = Console.ReadKey();
                if (s.KeyChar == 'a')
                    break;

            }
            relManager.closeConnection();


        }
    }
}
