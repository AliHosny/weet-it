using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VDS.RDF;
using VDS.RDF.Query;
using System.Web;

///usage:
///initialize an instance of the Comparison object giving it a list<string> uris you want to compare between
///then use getComparisonOutput() to get a list<Comparison.ResourceInformation> Object containing all the information about the resources
///Access any information you want to use by using the methods inside ResourceInformation DataStructure

namespace CompareComponent
{
    
    class Comparison
    {
        private SparqlRemoteEndpoint endpoint;
        public List<ResourceInformation> ComparisonOutput;
        public Comparison(List<string> uris, List<string> PredicatesToExclude = null)
        {
            //Initializing the server
            Uri serverUri = new Uri("http://localhost:8890/sparql");            
            endpoint = new SparqlRemoteEndpoint(serverUri);
            endpoint.Timeout = 999999;
            ComparisonOutput = new List<ResourceInformation>();
            getData(uris);
        }
  
        //Our data structure
        public  struct ResourceInformation
        {
            //it's own id
            private KeyValuePair<string, string> ID;
            //predicates information
            //URI, Label
            private List<KeyValuePair<string, string>> predicates_resourceIsSubj;//= new List<KeyValuePair<string, string>>(); 

            //resources information
            //URI,label
            private List<KeyValuePair<string, string>> resources_resourceIsSubj;//= new  List<KeyValuePair<string, string>>();

            //predicates information
            //URI, Label
            private List<KeyValuePair<string, string>> predicates_resourceIsObj;//=new List<KeyValuePair<string, string>>();

            //resources information
            //URI,label
            private List<KeyValuePair<string, string>> resources_resourceIsObj;//=new  List<KeyValuePair<string, string>>();

            //resources put in a form of Pred -> List<it's resources>
            private List<KeyValuePair<KeyValuePair<string, string>, List<KeyValuePair<string, string>>>> rawComparisonObject;

            //common resources between this resource and others in the same component
            private List<KeyValuePair<KeyValuePair<string, string>, List<KeyValuePair<string, string>>>> FinalComparisonObject;

            /// <summary>
            /// gets the id of the resource
            /// </summary>
            /// <returns>uri, label of the id</returns>
            public KeyValuePair<string, string> getID()
            {
                return this.ID;
            }

            /// <summary>
            /// gets all predicates responds to the query < resource> ?pred ?obj
            /// </summary>
            /// <returns>list<predURI,predLabel> </returns>
            public List<KeyValuePair<string, string>> getPredicates_ResourceIsSubj()
            {
                return this.predicates_resourceIsSubj.Distinct().ToList();
            }


            /// <summary>
            /// gets all predicates responds to the query ?subj ?pred < resource>
            /// </summary>
            /// <returns>list<predURI,predLabel> </returns>           
            public List<KeyValuePair<string, string>> getPredicates_ResourceIsObj()
            {
                return this.predicates_resourceIsObj.Distinct().ToList(); 
            }

            /// <summary>
            /// Common Predicates between all the resources we're comparing
            /// </summary>
            /// <returns>list<predURI,predLabel></returns>
            public List<KeyValuePair<string, string>> getCommonPredicates()
            {
                List<KeyValuePair<string, string>> toReturn = new List<KeyValuePair<string, string>>();
                foreach (KeyValuePair<KeyValuePair<string, string>, List<KeyValuePair<string, string>>> item in this.FinalComparisonObject)
                {
                    toReturn.Add(item.Key);
                }
                return toReturn;
            }

            /// <summary>
            /// gets List of resources of a certain predicate
            /// </summary>
            /// <param name="pred">the predicate as a keyValuePair</param>
            /// <returns>the resources attached as a list of keyValuePairs</returns>
            public List<KeyValuePair<string, string>> getResourcesOfPredicate(KeyValuePair<string, string> pred)
            {
                foreach (KeyValuePair<KeyValuePair<string, string>, List<KeyValuePair<string, string>>> item in rawComparisonObject)
                {
                    if (item.Key.Equals(pred))
                        return item.Value;
                }
                //if not foud
                return null;
            }


        }

        public List<ResourceInformation> getComparisonOutput()
        {
            return ComparisonOutput;
        }

        /// <summary>
        /// Fills the FinalComparisonObject of each ResourceInformation 
        /// Part of the resource's info that will return when comparing
        /// </summary>
        /// <param name="input">Resource Information</param>
        /// <returns>The same input but after adding the information of FinalComparisonObject</returns>
        public List<ResourceInformation> getCommon(List<ResourceInformation> input)
        {
            List<List<KeyValuePair<string, string>>> AllPredicates = new List<List<KeyValuePair<string, string>>>();
            List<KeyValuePair<string, string>> commonPredicates = new List<KeyValuePair<string, string>>();

            foreach (ResourceInformation item in input)
            {
                var tempPredicates = new List<KeyValuePair<string, string>>();
                foreach (KeyValuePair<KeyValuePair<string, string>, List<KeyValuePair<string, string>>> item2 in item.rawComparisonObject)
                {
                    tempPredicates.Add(item2.Key);
                }
                AllPredicates.Add(tempPredicates);
                                                
            }

            //Getting the intersection between all the lists
            var intersection = AllPredicates.Skip(1).Aggregate(new HashSet<KeyValuePair<string, string>>(AllPredicates.First()), (h, e) => { h.IntersectWith(e); return h; });
            commonPredicates = intersection.ToList();

            //populate the list and their members to each resourceInformation object
            //foreach (ResourceInformation item in input)            
            util.clearLog();
            for (int i = 0; i < input.Count; i++)
            {
                //logging 
                util.log("##############################" + input[i].ID.Value + "##############################");

                //comparing the common ones with the ones inside each resource to get the ones that match the key(predicate)
                //we should return to the object of copmarison part inside each resourceInformationObject
                foreach (KeyValuePair<string, string> item2 in commonPredicates)
                {
                    //item3 is each item of the predicate->resources list
                    foreach (KeyValuePair<KeyValuePair<string, string>, List<KeyValuePair<string, string>>> item3 in input[i].rawComparisonObject)
                    {
                        //compare here
                        //if they're equal
                        if (item3.Key.Equals(item2))
                        {
                            input[i].FinalComparisonObject.Add(item3);

                            //For logging purposes only
                            util.log(item3.Key.Value.ToString());
                            foreach (KeyValuePair<string,string> com in item3.Value)
                            {
                                string format = "{0," + item3.Key.Value.Length + "}{1,-10}";
                                util.log(string.Format(format," ",com.Value));
                            }                            
                             
                        }

                        
                    }
                    //logging purposes
                    util.log("-------------------------------------------------------------------------------------------------");
                }
               
            }

            return input;
        }

        /// <summary>
        /// logs all resources information, for testing
        /// </summary>
        /// <param name="input">list of all resourceInformation object</param>
        public void logResults(List<ResourceInformation> input)
        {
            //clearing log
            util.clearLog();
            foreach (ResourceInformation item in input)
            {
                util.log("####################################"+item.ID.Value+"###################################################################");
                util.log("####################################" + item.ID.Key + "###################################################################");
                foreach (KeyValuePair<KeyValuePair<string, string>, List<KeyValuePair<string, string>>> item2 in item.rawComparisonObject)
                {
                    util.log(item2.Key.Value);
                    foreach (KeyValuePair<string, string> item3 in item2.Value)
                    {
                        //"{0,-10} {1,10}"
                        string format = "{0," + item2.Key.Value.Length + "}{1,-10}";
                        util.log(string.Format(format," ",item3.Value));
                    }
                }                
            }
        }
        

        /// <summary>
        /// Fills The comparisonComponent of each Resource To compare
        /// </summary>
        /// <param name="input">Object to fill its rawComparisonObject</param>
        /// <returns>The same resource Again</returns>
        private ResourceInformation fillComparisonComponent(ResourceInformation input)
        {           
            var q = from x in input.predicates_resourceIsSubj
                    group x by x into g
                    let count = g.Count()
                    //orderby count descending
                    select new { Value = g.Key, Count = count };

            int counter = 0;
            foreach (var x in q)
            {
                List<KeyValuePair<string, string>> tempRes = new List<KeyValuePair<string, string>>();
                KeyValuePair<KeyValuePair<string, string>, List<KeyValuePair<string, string>>> temp = new KeyValuePair<KeyValuePair<string, string>, List<KeyValuePair<string, string>>>();
                KeyValuePair<string, string> predicates= new KeyValuePair<string, string>();
                predicates = x.Value;
                for (int i = counter; i < counter+x.Count; i++)
                {
                    tempRes.Add(input.resources_resourceIsSubj[i]);                                        
                }
                temp = new KeyValuePair<KeyValuePair<string, string>, List<KeyValuePair<string, string>>>(predicates, tempRes);
                input.rawComparisonObject.Add(temp);
                //Console.WriteLine("Value: " + x.Value + " Count: " + x.Count);
                counter += x.Count;
            }


            /////////
            var q2 = from x in input.predicates_resourceIsObj
                    group x by x into g
                    let count = g.Count()
                    //orderby count descending
                    select new { Value = g.Key, Count = count };

            counter = 0;
            foreach (var x in q2)
            {
                List<KeyValuePair<string, string>> tempRes = new List<KeyValuePair<string, string>>();
                KeyValuePair<KeyValuePair<string, string>, List<KeyValuePair<string, string>>> temp = new KeyValuePair<KeyValuePair<string, string>, List<KeyValuePair<string, string>>>();
                KeyValuePair<string, string> predicates = new KeyValuePair<string, string>();
                predicates = x.Value;
                for (int i = counter; i < counter + x.Count; i++)
                {
                    tempRes.Add(input.resources_resourceIsObj[i]);
                }
                temp = new KeyValuePair<KeyValuePair<string, string>, List<KeyValuePair<string, string>>>(predicates, tempRes);
                input.rawComparisonObject.Add(temp);
                //Console.WriteLine("Value: " + x.Value + " Count: " + x.Count);
            }
            /////////
            return input;
        }

        /// <summary>
        /// gets the label of the uri, if can't find, it removes the last /
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        private string getLabel(string uri)
        {
            Console.WriteLine("label");
           
            //at least best one for now
            uri = Uri.EscapeUriString(uri);
            //uri = util.UrlEncode(uri);
            util.log(uri);
            string query = "select * where {<" + uri + "> <http://www.w3.org/2000/01/rdf-schema#label> ?obj}";
            SparqlResultSet results = endpoint.QueryWithResultSet(query);
            //if there's no results from the first query, we will try to get the name 
            if (results.Count < 1)
            {
                string name_query = "select * where {<" + uri + "> <http://xmlns.com/foaf/0.1/name> ?obj}";
                results = endpoint.QueryWithResultSet(name_query);

                //if there's no result from the second query
                //get the name after the /
                if (results.Count < 1)
                {
                    string toreturn = new string(uri.ToCharArray().Reverse().ToArray());//uri.Reverse().ToString();

                    toreturn = toreturn.Remove(toreturn.IndexOf("/"));
                    toreturn = new string(toreturn.ToCharArray().Reverse().ToArray());
                    toreturn = toreturn.Replace("_", " ");
                    //TODO : get back the encoding
                    toreturn = toreturn.Trim();
                    util.log(toreturn + " [manual]");
                    return toreturn;
                }
                else
                {
                    util.log(((LiteralNode)results[0].Value("obj")).Value+ " [name]");
                    return ((LiteralNode)results[0].Value("obj")).Value;
                    //return results[0].Value("obj").ToString();
                }

            }
            else
            {
                util.log(((LiteralNode)results[0].Value("obj")).Value +" [label]");
                return ((LiteralNode)results[0].Value("obj")).Value;
            }
        }

        

        public void getData(List<string> Input_URIs)
        {
            //should be returned
            List<ResourceInformation> ResourceInformationList = new List<ResourceInformation>();
            foreach (string item in Input_URIs)
            {
                //constructing the queries, we had to do FROM<dbpedia.org> to prevent garbage data, should be faster too
                string querySide1 = "SELECT * FROM <http://dbpedia.org> WHERE {<" + item + "> ?pred ?obj}";
                string querySide2 = "SELECT * FROM <http://dbpedia.org> WHERE {?subj ?pred <" + item + ">}";
                               
                
                //doing both queries              
                SparqlResultSet res1 = new SparqlResultSet();
                SparqlResultSet res2 = new SparqlResultSet();
                res1 = endpoint.QueryWithResultSet(querySide1);
                res2 = endpoint.QueryWithResultSet(querySide2);

                //Initialize the resource
                ResourceInformation temp = new ResourceInformation();
                temp.predicates_resourceIsSubj = new List<KeyValuePair<string, string>>();
                temp.resources_resourceIsSubj = new List<KeyValuePair<string, string>>();
                temp.predicates_resourceIsObj = new List<KeyValuePair<string, string>>();
                temp.resources_resourceIsObj = new List<KeyValuePair<string, string>>();
                temp.rawComparisonObject = new List<KeyValuePair<KeyValuePair<string, string>, List<KeyValuePair<string, string>>>>();
                temp.FinalComparisonObject = new List<KeyValuePair<KeyValuePair<string, string>, List<KeyValuePair<string, string>>>>();
                temp.ID = new KeyValuePair<string, string>();

                //Filling all the information in case  <ourResourceID> ?x ?y
                foreach (SparqlResult item_res1 in res1)
                {             
                    //string tempo=getLabel(((LiteralNode)item_res1.Value("pred")).ToString());
                    if (String.Equals(((INode)(item_res1.Value("obj"))).GetType().Name.ToString(), "UriNode"))
                    {
                        temp.resources_resourceIsSubj.Add(new KeyValuePair<string, string>(item_res1.Value("obj").ToString(), getLabel(item_res1.Value("obj").ToString())));
                        
                    }
                    else
                    {
                        string onlyValue = ((LiteralNode)item_res1.Value("obj")).Value;
                        temp.resources_resourceIsSubj.Add(new KeyValuePair<string, string>(item_res1.Value("obj").ToString(), onlyValue));

                        //To fill out the ID component
                        if (String.Equals(item_res1.Value("pred").ToString(), "http://www.w3.org/2000/01/rdf-schema#label"))
                        {
                            temp.ID = new KeyValuePair<string, string>(item, temp.resources_resourceIsSubj[temp.resources_resourceIsSubj.Count - 1].Value);
                        }
                    }

                    temp.predicates_resourceIsSubj.Add(new KeyValuePair<string, string>(item_res1.Value("pred").ToString(), getLabel(item_res1.Value("pred").ToString())));                    

                }

                //Filling all the information in case ?x ?y <ourResourceID>
                foreach (SparqlResult item_res2 in res2)
                {
                    //We add of to the name, child of .....etc 
                    temp.predicates_resourceIsObj.Add(new KeyValuePair<string, string>(item_res2.Value("pred").ToString(), getLabel(item_res2.Value("pred").ToString()) + " of"));

                    if (String.Equals(((INode)(item_res2.Value("subj"))).GetType().Name.ToString(), "UriNode"))
                        temp.resources_resourceIsObj.Add(new KeyValuePair<string, string>(item_res2.Value("subj").ToString(), getLabel(item_res2.Value("subj").ToString())));
                    else
                    {
                        string onlyValue = ((LiteralNode)item_res2.Value("subj")).Value;
                        temp.resources_resourceIsSubj.Add(new KeyValuePair<string, string>(item_res2.Value("subj").ToString(), onlyValue));
                    }

                }
                
                //filling comparison component
                temp=fillComparisonComponent(temp);                

                //Addding the resource to the list of resourceInformation Objects
                ResourceInformationList.Add(temp);

                //Copying it to the globalVariable
                ComparisonOutput = ResourceInformationList;
                
            }
            //getting common between URIs
            Console.WriteLine("getting Common");
            ResourceInformationList=getCommon(ResourceInformationList);

            
            //logging
            logResults(ResourceInformationList);
            
        }


    

    }
}
