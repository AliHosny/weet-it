using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VDS.RDF.Query;
using VDS.RDF; 

namespace RelationGenerator
{
    public static class util
    {
        public static string getLabel(String URI)
        {
            //at least best one for now
            URI = Uri.EscapeUriString(URI);
            SparqlRemoteEndpoint endpoint = new SparqlRemoteEndpoint(new Uri("http://localhost:8890/sparql"));
            string query = "select * where {<" + URI + "> <http://www.w3.org/2000/01/rdf-schema#label> ?obj}";
            SparqlResultSet results = endpoint.QueryWithResultSet(query);
            //if there's no results from the first query, we will try to get the name 
            if (results.Count < 1)
            {
                string name_query = "select * where {<" + URI + "> <http://xmlns.com/foaf/0.1/name> ?obj}";
                results = endpoint.QueryWithResultSet(name_query);

                //if there's no result from the second query
                //get the name after the /
                if (results.Count < 1)
                {
                    string toreturn = new string(URI.ToCharArray().Reverse().ToArray());//URI.Reverse().ToString();
                    toreturn = toreturn.Remove(toreturn.IndexOf("/"));
                    toreturn = new string(toreturn.ToCharArray().Reverse().ToArray());
                    toreturn = toreturn.Replace("_", " ");
                    //TODO : get back the encoding
                    toreturn = toreturn.Trim();
                    return toreturn;
                }
                else
                {
                    //returning
                    return ((LiteralNode)results[0].Value("obj")).Value;
                }
            }
            else
            {
                //returning it
                return ((LiteralNode)results[0].Value("obj")).Value;
            }
        }



    }
}
