using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VDS.RDF;
using VDS.RDF.Query;
using System.IO;
using VDS.RDF.Storage;
using VDS.RDF.Parsing;


/*The communication happens between the methods inside this class thorugh the properties of the class itself.
 * the public lists
 */
namespace ObjectsRelationManager
{
    public static class QueryProcessor
    {
        public static VirtuosoManager manager;
        private static bool isConnectionStarted = false;
        
        /// <summary>
        /// starts connection with the server
        /// </summary>
        public static void startConnection()
        {
            //Initiating the manager(to be added to constructor?)
            if (!isConnectionStarted)
            {
                //manager = new VirtuosoManager("Server=localhost;Uid=dba;pwd=dba;Connection Timeout=500");
                
                manager = new VirtuosoManager("http://192.168.33.2:8890", 1111, "DB", "dba", "dba");
                isConnectionStarted = true;
            }

        }

        /// <summary>
        /// closes connection to the server
        /// </summary>
        public static void closeConnection()
        {
            if (isConnectionStarted)
            {
                manager.Close(true);
                isConnectionStarted = false;
            }

        }


        /// <summary>
        /// gets the graph from the input Uri(changes the uri to localhost)
        /// </summary>
        /// <param name="input">the input string uri</param>
        /// <returns>the output graph</returns>
        public static Graph getGraphFromURIWithlocalHost(string input)
        {            
            Graph g = new Graph(); 
           
            //replacing the domainname with localhost
            int count = 0;
            string toreplace = "";
            for (int i=0; i < input.Length;i++ )
            {
                if (count == 2)
                    toreplace += input[i];
                if (input[i] == '/')
                    count++;               

            }
            input = input.Replace(toreplace, "localhost:8890/");

            //getting the rdf
            try
            {
                UriLoader.Load(g, new Uri(input));
            }
            catch{}
            return g;
            
        }

        /// <summary>
        /// Exectues a certain List of InnerQuery objects
        /// </summary>
        /// <param name="input">the list of innerquery to be queried</param>
        /// <returns>a list of resultSet one for each innerquery.queryText</returns>
        public static List<ResSetToJSON.innerResult> ExecuteQueryWithInnerQuery(SPARQLQueryBuilder.InnerQuery input,string obj1,string obj2)
        {
            //list to hold the results
            List<ResSetToJSON.innerResult> resultsList = new List<ResSetToJSON.innerResult>();

            try
            {
                //temp result holder
                ResSetToJSON.innerResult temp;
                //fetching results and passing to the list

                temp = new ResSetToJSON.innerResult();
                temp.firstObj = obj1;
                temp.lastObj = obj2;
                temp.connectState = input.connectState;
                temp.resultSets = ExecuteQueryWithString(input.queryText);
                
                //if there's any results add it
                if (temp.resultSets.Count > 0)
                    resultsList.Add(temp);


            }
            catch { }            
            return resultsList;
            
        }


        /// <summary>
        /// overload of Execute query
        /// </summary>
        /// <param name="input">the query text as string</param>
        /// <returns></returns>
        public static SparqlResultSet ExecuteQueryWithString(string input)
        {
            //list to hold the results
            SparqlResultSet resultSet = new SparqlResultSet();
            try
            {
                
                //making the query
                Object result = manager.Query(input);
                //Object result = manager.ExecuteQuery(input);
                resultSet = (SparqlResultSet)result;                
               
            }
            catch { }
            return resultSet;

        }
    }
}
