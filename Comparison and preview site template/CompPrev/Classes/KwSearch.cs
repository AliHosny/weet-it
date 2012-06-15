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
    {     static string[] versus_delimeter =new string[] {" vs "};


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
       private static List<string> generate_similarkeywords(string keyword)
       {
           string temp=null;

           List<string> similar_keywords = new List<string>();
           List<string> kwrd_words = new List<string>();
           similar_keywords.Add(keyword);
           kwrd_words = ((keyword.ToLower()).Split(' ')).ToList<string>();//splits a single multiword keyword into several words;

           for (int i = 0; i < kwrd_words.Count; i++)//Capitalize first letter of each word

               kwrd_words[i] = char.ToUpper(kwrd_words[i][0]) + kwrd_words[i].Substring(1);

           for (int i = 0; i < kwrd_words.Count; i++)//reconcat first-letter capitalized words to for the original keyword
               temp += (kwrd_words[i] + " ");
           temp = temp.Trim();
           
           similar_keywords.Add(temp);
           similar_keywords.Add( keyword.ToUpper());
           similar_keywords.Add(keyword.ToLower());
           similar_keywords.Add(keyword.Replace(' ','_'));
           similar_keywords.Add(temp.Replace(' ', '_'));
           similar_keywords.Add((keyword.ToUpper()).Replace(' ', '_'));
           similar_keywords.Add((keyword.ToLower()).Replace(' ', '_'));
           return similar_keywords;
          
          
       }

        private static string Find_URIs(List<string> keywords)
       {
           
           SparqlResultSet result = new SparqlResultSet();
           string query = null;
           List<string> similar_keywords = new List<string>();
           List<string> uris = new List<string>();
           string comma_sep_uris;
               
           for (int i = 0; i < keywords.Count; i++)
           {
               similar_keywords = generate_similarkeywords(keywords[i]);
               //here a for loop to query the similar keywords and add found uris to uris List<string>
               //if a uri is not found a "not found" string is put instead of the uri
               for (int j = 0; j < similar_keywords.Count; j++)
               {

                   query="select distinct * where { {?t1 <http://www.w3.org/2000/01/rdf-schema#label> \""+similar_keywords[j]+"\"   @en }}";
                   //query = "select distinct * where  {?t1 <http://www.w3.org/2000/01/rdf-schema#label> \"The Dark Knight\"   @en }";
                   QueryProcessor.startConnection();
                   result = QueryProcessor.ExecuteQueryWithString(query);
                   //QueryProcessor.closeConnection();
                   if (result.Count == 0)
                       continue;
                   else
                   {
                       uris.Add(result[0].Value("t1").ToString());
                       break;
                   }
                  

               }


           }

           comma_sep_uris = string.Join(",", uris.ToArray());
           return comma_sep_uris;
       }
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