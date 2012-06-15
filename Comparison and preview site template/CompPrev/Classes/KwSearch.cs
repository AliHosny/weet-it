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
           List<int> scores=new List<int>();
           List<string> uris = new List<string>();
           string comma_sep_uris;
               
           for (int i = 0; i < keywords.Count; i++)
           {
               //query = "select distinct * where{" +
               //    "<http://dbpedia.org/resource/Inception> ?x ?y}";
               
               //here a for loop to query the similar keywords and add found uris to uris List<string>
               //if a uri is not found a "not found" string is put instead of the uri


               query = "select distinct  ?subject ?literal ?redirects where{" +



"?subject <http://www.w3.org/2000/01/rdf-schema#label> ?literal." +

"optional {   ?subject <http://dbpedia.org/ontology/wikiPageRedirects> ?redirects}." +
"optional {?subject <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> ?type}." +



"Filter (!bound(?type) || !(?type=<http://www.w3.org/2004/02/skos/core#Concept>))." +



"?literal bif:contains '\"" + keywords[i] + "\"'.}";

                   //query = "select distinct * where  {?t1 <http://www.w3.org/2000/01/rdf-schema#label> \"The Dark Knight\"   @en }";

                   result = remoteEndPoint.QueryWithResultSet(query);
                   //QueryProcessor.closeConnection();
                   if (result.Count == 0)
                   {
                       uris.Add("");
                       continue;
                   }
                   else if (result.Count == 1)
                   {
                       if ((result[0].Value("redirects") == null))
                           uris.Add(result[0].Value("subject").ToString());
                       else
                           uris.Add(result[0].Value("redirects").ToString());
                       continue;
                   }
                   else 
                   {
                       
                       int new_value;
                       int min_value=1000;
                       int max_index=0;
                       for ( int j = 0; j < result.Count; j++)
                       {

                         new_value=(  computeLevenshteinDistance(keywords[i],result[j].Value("literal").ToString()));
                         scores.Add(new_value);
                           if(new_value<min_value)
                           {
                               max_index=j;
                               min_value = new_value;
                           }
                           else if (new_value == min_value)
                           {
                               if (result[j].Value("redirects") == null)
                               {
                                   max_index = j;
                                   min_value = new_value;

                               }
                               else
                               {
                                   min_value = new_value;
                               }

                           }
                       }
                       if ((result[max_index].Value("redirects") == null))
                           uris.Add(result[max_index].Value("subject").ToString());
                       else
                           uris.Add(result[max_index].Value("redirects").ToString());
                      
                       min_value = 0;
                   }
                   
               


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
        public static int computeLevenshteinDistance(string s, string t)
        {
            int n = s.Length;
            int m = t.Length;
            int[,] d = new int[n + 1, m + 1];

            // Step 1
            if (n == 0)
            {
                return m;
            }

            if (m == 0)
            {
                return n;
            }

            // Step 2
            for (int i = 0; i <= n; d[i, 0] = i++)
            {
            }

            for (int j = 0; j <= m; d[0, j] = j++)
            {
            }

            // Step 3
            for (int i = 1; i <= n; i++)
            {
                //Step 4
                for (int j = 1; j <= m; j++)
                {
                    // Step 5
                    int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;

                    // Step 6
                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }
            // Step 7
            return d[n, m];
        }
    }
}