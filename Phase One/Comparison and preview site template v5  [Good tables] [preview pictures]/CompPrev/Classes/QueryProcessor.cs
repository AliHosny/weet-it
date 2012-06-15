using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VDS.RDF;
using VDS.RDF.Query;
using System.IO;
using VDS.RDF.Storage;
using VDS.RDF.Parsing;
using System.Text.RegularExpressions;

/*The communication happens between the methods inside this class thorugh the properties of the class itself.
 * the public lists
 */
namespace ObjectsRelationFactory
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
                
                manager = new VirtuosoManager("localhost", 1111, "DB", "dba", "dba");
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
                //manager.Close(false);
                //isConnectionStarted = false;
            }

        }


        /// <summary>
        /// converts the localhost to dbpedia, not recommended
        /// </summary>
        /// <param name="input">the localhost input</param>
        /// <returns>the dbpeida link</returns>
        public static string convertToDbpedia(string input)
        {
            //testing purposes only
            if(input.Contains("localhost"))
            {
            int count = 0;
            string toreplace = "";
            for (int i = 0; i < input.Length; i++)
            {
                if (count == 2)
                    toreplace += input[i];
                if (input[i] == '/')
                    count++;

            }
            input = input.Replace(toreplace, "dbpedia.org/");

            }
            return input;

        }


        /// <summary>
        /// converts any domain name to the localhost:8890
        /// </summary>
        /// <param name="input">url with any domain name</param>
        /// <returns>converted url</returns>
        public static string convertToLocalhost(string input)
        {
            int count = 0;
            string toreplace = "";
            for (int i = 0; i < input.Length; i++)
            {
                if (count == 2)
                    toreplace += input[i];
                if (input[i] == '/')
                    count++;

            }
            input = input.Replace(toreplace, "localhost:8890/");
            return input;

        }


        /// <summary>
        /// Gets label of given node.
        /// </summary>
        /// <param name="node">Node</param>
        /// <returns>String of the node label</returns>
        public static string getLabelIfAny(string node)
        {
            if (node.Contains("http://"))
            {
                QueryProcessor.startConnection();

                SparqlResultSet results = QueryProcessor.ExecuteQueryWithString("select ?x where {<" + node + "> <http://www.w3.org/2000/01/rdf-schema#label> ?x}");// FILTER (langMatches(lang(?x), \"EN\"))}");

                QueryProcessor.closeConnection();

                if (results.Count != 0)
                    return results[0].Value("x").ToString();
                else
                    return node.ToString();
            }
            else
                return node.ToString();
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

                //

                //temp.firstObj = obj1;
                //temp.lastObj = obj2;

                temp = setOriginalObjects(temp,input.queryText);
                
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
        /// helper function to manage the changes of the queries first and last objects
        /// </summary>
        /// <param name="temp">the innerResult object ot set it's first and last object</param>
        /// <param name="input">the query string</param>
        /// <returns></returns>
        private static ResSetToJSON.innerResult setOriginalObjects(ResSetToJSON.innerResult temp, string input)
        {
            //List<string> urls = new List<string>();
            string one="";
            string two="";
            MatchCollection mc = Regex.Matches(input, @"\<(.*?)\>", RegexOptions.IgnoreCase);
            one = mc[0].Value.Replace("<", "");
            temp.firstObj = one.Replace(">", "");

            two = mc[1].Value.Replace("<", "");
            temp.lastObj = two.Replace(">", "");

            return temp;
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
                //just in cas someone didn't open the connection
                if (!isConnectionStarted)
                    startConnection();
                
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
