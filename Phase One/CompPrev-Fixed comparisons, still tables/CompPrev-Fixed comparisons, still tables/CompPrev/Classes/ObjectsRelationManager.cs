using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ObjectsRelationFactory
{
    class ObjectsRelationManager
    {
        //is it the end of the results yet?
        private bool isEndOfResults=false;
        public bool IsEndOfResults
        {
            get {return isEndOfResults;}      
                    
        }

        //private variables
        private List<SPARQLQueryBuilder.InnerQuery> generatedQueriesList;


        //The two objects to compare between
        private string obj1;
        private string obj2;


        private static bool isConnectionStarted = false;

        /// <summary>
        /// starts the connection to the server
        /// </summary>
        public  void startConnection()
        {
            //Initiating the manager(to be added to constructor?)
            if (!isConnectionStarted)
            {
                QueryProcessor.startConnection();
                isConnectionStarted = true;
            }

        }

        /// <summary>
        /// closes the connection of the server
        /// </summary>
        public  void closeConnection()
        {
            if (isConnectionStarted)
            {
                QueryProcessor.closeConnection();
                isConnectionStarted = false;
            }

        }


        /// <summary>
        /// Builds and returns a set of queries to find relations between two object1 and object2.
        /// </summary>
        /// <param name="object1">object1</param>
        /// <param name="object2">object2</param>
        /// <param name="maxDistance">MaxiumDistance between the two objects</param>
        /// <param name="limit">Limit of results</param>
        /// <param name="ignoredObjects">List of strings of names of objects be ignored in the Queries</param>
        /// <param name="ignoredProperties">List of strings of names of properties to be ignored in the Queries</param>
        /// <param name="avoidCycles">Integer value which indicates whether we want to suppress cycles , 0 = no cycle avoidance ,  1 = no intermediate object can be object1 or object2 ,   2 = like 1 + an object can not occur more than once in a connection</param>
        /// <returns>false means an error happened, true means it's ok</returns>
        public bool generateQueries(string object1, string object2, int maxDistance=7, int limit=5, List<string> ignoredObjects = null, List<string> ignoredProperties = null, int avoidCycles = 0)
        {
            //resetting the bool
            isEndOfResults = false;

            //to make other methods see the two objects
            obj1 = object1;
            obj2 = object2;

            SPARQLQueryBuilder builder = new SPARQLQueryBuilder();
            generatedQueriesList=builder.buildQueries(object1, object2, maxDistance, limit, ignoredObjects, ignoredProperties, avoidCycles);
            
            //if an error happened
            if (generatedQueriesList.Count < 1)
                return false;
            
            return true;
        }

        /// <summary>
        /// Query the next result 
        /// </summary>
        /// <returns>the JsonObject of the next result</returns>
        public string getNextResult()
        {
            //inner results to execute queries to
            List<ResSetToJSON.innerResult> results = new List<ResSetToJSON.innerResult>();

            //the query gets processed and the query is removed from the query list till it's empty
            if (generatedQueriesList != null && generatedQueriesList.Count > 0)
            {
                //making the nextQuery
                results = QueryProcessor.ExecuteQueryWithInnerQuery(generatedQueriesList[0], obj1, obj2);

                //removing the query done from the list
                generatedQueriesList.RemoveAt(0);

                string res=ResSetToJSON.ToJsonObj(results);
                //generating the JsonObj
                return res;
            }

            else
            {
                //termenating
                isEndOfResults = true;
                return "";
            }

        }


    }
}
