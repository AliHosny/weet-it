using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VDS.RDF.Query;
using VDS.RDF;
using ObjectsRelationFactory;
namespace KwSearch
{
    /// <summary>
    /// Process search for single word or multiple-words queries
    /// </summary>
    public static class KwSearch
    {     static string[] versus_delimeter =new string[] {" vs ","VS","Vs"};


    /// <summary>
    /// splits a text by the "vs" keyword and return a List of splitted strings
    /// </summary>
    /// <param name="text">string to be split</param>
    /// <returns></returns>
       public static List<string> ParseTextByVs(string text)
       {
           
           List<string> Parsed_keywords = (text.Split(versus_delimeter,StringSplitOptions.RemoveEmptyEntries)).ToList<string>();
           for (int i = 0; i < Parsed_keywords.Count;i++ )
           {
               Parsed_keywords[i] = Parsed_keywords[i].Trim();


           }
           System.Diagnostics.Debug.WriteLine(Parsed_keywords[0]);
           return Parsed_keywords;
       
       }
       /// <summary>
       /// Generates a list of keywords which are similar to the given keyword with different capiitalizations and formats
       /// </summary>
       /// <param name="keyword">The keyword</param>
       /// <returns>
       /// List of similar keywords in addition to the originan keyword
       /// </returns>
       

        private static string Find_URIs(List<string> keywords)
       {
           SparqlRemoteEndpoint remoteEndPoint = new SparqlRemoteEndpoint(new Uri("http://localhost:8890/sparql"));
           

           
           SparqlResultSet result = new SparqlResultSet();
           string query = null;
           List<string> similar_keywords = new List<string>();
           List<string> uris = new List<string>();
           string comma_sep_uris;
               
           for (int i = 0; i < keywords.Count; i++)
           {
               
               //here a for loop to query the similar keywords and add found uris to uris List<string>
               //if a uri is not found a "not found" string is put instead of the uri
               

                   query="select distinct  * where{"+



"?subject <http://www.w3.org/2000/01/rdf-schema#label> ?literal."+

"optional {   ?subject <http://dbpedia.org/ontology/wikiPageRedirects> ?redirects}."+
"optional {?subject <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> ?type"+
         " Filter (!(str(?type)=\"http://www.w3.org/2004/02/skos/core#Concept\"))}. "+

   
"Filter (!(str(?type)=\"http://www.w3.org/2004/02/skos/core#Concept\"))."+


       
"?literal bif:contains '\""+keywords[i]+"\"'."+
"Filter regex (str(?literal),\"^"+keywords[i]+"$\",'i') }";
                   //query = "select distinct * where  {?t1 <http://www.w3.org/2000/01/rdf-schema#label> \"The Dark Knight\"   @en }";

                   result = remoteEndPoint.QueryWithResultSet(query);
                   //QueryProcessor.closeConnection();
                   if (result.Count == 0)
                   {
                       uris.Add("");
                       continue;
                   }
                   else 
                   {
                       if((result[0].Value("redirects")==null))
                        uris.Add(result[0].Value("subject").ToString());
                       else
                           uris.Add(result[0].Value("redirects").ToString());
                       continue;
                   }
                   //else
                   //{
                   //    for (int j = 0; j < result.Count; j++)
                   //    {
                   //        for (int k = j+1; k < result.Count ; k++)
                   //        {
                   //            if(result[j].Value("subject").ToString().Equals(result[k].Value("subject")))
                   //            {
                   //               result.
                   //            }


                   //        }
                   //    }
                   //}

               


           }

           comma_sep_uris = string.Join(",", uris.ToArray());
           return comma_sep_uris;
       }
        //public static scoring()
        //{
        

        //}
        /// <summary>
        /// get the uris matching with given keywords(single keyword or multiple keywords separated by versus
        /// </summary>
        /// <param name="input_query">takes the query text whether it contains vs or single keyword</param>
        /// <returns></returns>
        public static string get_URI_s(string input_query)
        { 
        
        return Find_URIs(ParseTextByVs(input_query));
        }

    }
}