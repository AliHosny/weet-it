using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Data;
using VDS.RDF;
using VDS.RDF.Query;

namespace ObjectsRelationFactory
{
    public static class ResSetToJSON
    {
        #region variables
        //private static string s;
        static List<List<string>> z = new List<List<string>>();
        
        public struct innerResult
        {
            public SparqlResultSet resultSets;
            public int connectState;
            public string firstObj, lastObj;
        }
        #endregion


        public static List<string> ToListOfJsonObj(List<innerResult> b)
        {
            List<string> toReturn = new List<string>();

            //if the results are empty
            if (b.Count == 0)
                return toReturn;

            z = innerResultToURIs(b);

            string s = "";
            foreach (List<string> m in z)
            {
                m.Insert(0, b[0].firstObj);
                m.Insert(m.Count, b[0].lastObj);

            }
            foreach (List<string> x in z)
            {
                
                s = "{" + toJSONNode(x) + "," + toJSONEdge(x) + "}";
                toReturn.Add(s);
                //textBox3.Text += toJSONEdge(x) + "\n";
            }

            return toReturn;
        }


        public static string ToJsonObj(List<innerResult> b)
        {
            if (b.Count == 0)
                return "";
            z = innerResultToURIs(b);
            string s = "";
            foreach (List<string> m in z)
            {
                m.Insert(0, b[0].firstObj);
                m.Insert(m.Count, b[0].lastObj);
            }
            foreach (List<string> x in z)
            {
                s += "{" + toJSONNode(x) + "," + toJSONEdge(x) + "},";
                //textBox3.Text += toJSONEdge(x) + "\n";
            }
            s = s.Remove((s.Length) - 1, 1);
            s += "";
            return s;


        }


        /// <summary>
        /// renames the string;
        /// </summary>
        /// <param name="URI"></param>
        /// <returns></returns>
        public static string namer(string URI)
        {
            string inp = "select ?x where{<" + URI + "> <http://www.w3.org/2000/01/rdf-schema#label> ?x}";
            QueryProcessor.startConnection();
            SparqlResultSet result= QueryProcessor.ExecuteQueryWithString(inp);
            QueryProcessor.closeConnection();
            string resString = "";

            //if the query got a result
            if (result.Count > 0)
            {
                SparqlResult res = result[0];
                resString = (res.Value("x")).ToString();
                //resString = res.ToString();
            }

            //if something bad happened
            else
            {
                int x = URI.LastIndexOf('/');
                resString= (URI.Substring(x + 1));

            }
            int index = resString.IndexOf("@");
            if (index != -1)
                resString = resString.Remove(resString.IndexOf("@"));
            return resString;
            //return r;
            
        }    // get the labels of URIs
        

        private static List<string> valuesOfResults(SparqlResult sq)
        {
            List<string> variableName = new List<string>();
            variableName = sq.Variables.ToList();
            List<string> output = new List<string>();
            foreach (string var in variableName)
            {
                output.Add(sq.Value(var).ToString());
            }
            return output;
        }
        
        
        // get the value of each variable
        private static List<List<string>> innerResultToURIs(List<innerResult> inner)
        {
            List<List<string>> a = new List<List<string>>();
            foreach (innerResult q in inner)
            {
                foreach (SparqlResult w in q.resultSets)
                {
                    a.Add(valuesOfResults(w));
                }
            }
            return a;
        }

        static int counter = 0;
        // get all the results of each variable
        private static string toJSONNode(List<string> query)
        {
            string a = "nodes:{";
            /*foreach (string s in query)
            { 
                a+="\""+namer(s)+"\":{\"uri\":\""+s+"\"},";
            }*/
            
            //
            a += "\"" + namer(query[0]) + "\":{\"uri\":\"" + query[0] + "\",\"shape\":\"rectangle\",\"color\":\"#956BC9\",\"label\":\"" + namer(query[0]) + "\"},";
            
            for (int i = 1; i < query.Count-1; i++)
            {
                if (i % 2 == 0)
                    a += "\"" + namer(query[i])  + "\":{\"uri\":\"" + query[i] + "\",\"shape\":\"rectangle\",\"color\":\"#9E9E9E\",\"label\":\"" + namer(query[i]) + "\"},";
                else
                    a += "\"" + namer(query[i])  + "\":{\"uri\":\"" + query[i] + "\",\"shape\":\"rectangle\",\"color\":\"#FF8800\",\"label\":\"" + namer(query[i]) + "\"},";
            }
            a += "\"" + namer(query[query.Count-1]) + "\":{\"uri\":\"" + query[query.Count-1] + "\",\"shape\":\"rectangle\",\"color\":\"#956BC9\",\"label\":\"" + namer(query[query.Count-1]) + "\"},";
            
            a = a.Remove((a.Length) - 1, 1);
            a += "}";
            return a;
        } 
        
        // get the nodes of each query
        private static string toJSONEdge(List<string> query)
        {
            string a = "edges:{";
            /*foreach (string s in query)
            {
                a += "\"" + namer(s) + "\":{\"" + s + "\":{}},";
            }*/
            for (int i = 0; i < query.Count - 1; i++)
            {
                a += "\"" + namer(query[i]) + "\":{\"" + namer(query[i + 1]) + "\":{}},";
            }
            a = a.Remove((a.Length) - 1, 1);
            a += "}";
            return a;
        }


    }
}