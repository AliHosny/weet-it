using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Data;
using VDS.RDF;
using VDS.RDF.Query;

namespace ObjectsRelationManager
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

        public static string ToJsonObj(List<innerResult> b)
        {
            if (b.Count == 0)
                return "";
            z = innerResultToURIs(b);
            string s = "[";
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
            s += "]";
            return s;


        }


        private static string namer(string URI)
        {

            int x = URI.LastIndexOf('/');
            return (URI.Substring(x + 1));
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
        
        
        // get all the results of each variable
        private static string toJSONNode(List<string> query)
        {
            string a = "nodes:{";
            /*foreach (string s in query)
            { 
                a+="\""+namer(s)+"\":{\"uri\":\""+s+"\"},";
            }*/
            for (int i = 0; i < query.Count; i++)
            {
                if (i % 2 == 0)
                    a += "\"" + namer(query[i]) + "\":{\"uri\":\"" + query[i] + "\",\"shape\":\"dot\",\"color\":\"blue\",\"label\":\"" + namer(query[i]) + "\"},";
                else
                    a += "\"" + namer(query[i]) + "\":{\"uri\":\"" + query[i] + "\",\"shape\":\"rectangle\",\"color\":\"blue\",\"label\":\"" + namer(query[i]) + "\"},";
            }
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