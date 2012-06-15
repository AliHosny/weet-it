using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;

using VDS.RDF;
using VDS.RDF.Query;
using Comparison_Part;
using ObjectsRelationFactory;

namespace CompPrev
{
    public partial class _Default : System.Web.UI.Page
    {
        List<string> uri_s = new List<string>();


        protected void Page_Load(object sender, EventArgs e)
        {

        }
        static string parse_internal_list(List<INode> x)
        {
            string y = null;
            foreach (INode z in x)
            {
                y += z.ToString() + "\n";
            }

            return y;
        }

       


        #region WebMethods

        ///// <summary>
        ///// returns the comparison table between two or many objects sent in the URIs String 
        ///// </summary>
        ///// <param name="URIs">URIs comma separated URIs to be compared</param>
        ///// <returns>string of an html table contains the comparison between sent objects</returns>
        //[WebMethod(EnableSession = false)]
        //public static string getComparisonTable(string URIs)
        //{
        //    string result;
        //    List<string> uri_s = (URIs.Split(',')).ToList<string>();

        //    Comparison comparison_object = new Comparison(uri_s);

        //    result = "<table width=\"1100px\" class=\"resultTable\" Cellpadding='0px' Cellmargin='0px'>";

        //    for (int i = 0; i < (comparison_object.CommonPredicate_SubjectLabel.Count); i++)
        //    {

        //        result += "<tr><td>" + comparison_object.CommonPredicate_SubjectLabel[i] + "</td>";
        //        foreach (ComparisonElement y in comparison_object.ComparisonElement)
        //        {
        //            foreach (string x in y.CommonPredicateObject_String[i])
        //            {
        //                result += ("<td>" + x + "</td>");
        //            }
        //            result += "</tr>";
        //        }
        //    }

        //    result += "<Table/>";

        //    return result;
        //}


        /// <summary>
        /// returns the comparison table between two or many objects sent in the URIs String 
        /// </summary>
        /// <param name="URIs">URIs comma separated URIs to be compared</param>
        /// <returns>string of an JSON table contains the comparison between sent objects</returns>
        [WebMethod(EnableSession = false)]
        public static string getJSONComparisonTable(string URIs)
        {
            string result;
            List<string> uri_s = (URIs.Split(',')).ToList<string>();

            Comparison comparison_object = new Comparison(uri_s);

            result = "{";

            result += "'__comparisonElements' : [";
            foreach (string i in comparison_object.CommonPredicate_SubjectLabel)
            {
                string ireplaced = i.Replace("'", @"\'");
                result += (i == comparison_object.CommonPredicate_SubjectLabel[comparison_object.CommonPredicate_SubjectLabel.Count - 1]) ? "'" + ireplaced + "'" : "'" + ireplaced + "',";
            }

            //foreach (string i in comparison_object.CommonPredicate_ObjectLabel)
            //{
            //    result += "'" + i + "',";

            //}

            result += "],";


            for (int i = 0; i < (comparison_object.CommonPredicate_SubjectLabel.Count); i++)
            {
                string labelReplaced = comparison_object.CommonPredicate_SubjectLabel[i].Replace("'", @"\'");
                result += "'" + labelReplaced + "':{";

                foreach (ComparisonElement y in comparison_object.ComparisonElement)
                {

                    result += "'" + y.ElementLabel + "':[";

                    foreach (string x in y.CommonPredicateObject_String[i])
                    {
                        string xreplaced = x ;
                        if (x.Contains("'"))
                         xreplaced = x.Replace("'",@"\'");
                        result += (x == y.CommonPredicateObject_String[i][y.CommonPredicateObject_String[i].Count - 1]) ? "'" + xreplaced + "'" : "'" + xreplaced + "',";   
                    }
                
                    result += ( y == comparison_object.ComparisonElement[comparison_object.ComparisonElement.Count-1] )? "]" :  "]," ; 
                }
                result += (i == comparison_object.CommonPredicate_SubjectLabel.Count -1 ) ? "}":"},";
            }

            //for (int i = 0; i < (comparison_object.CommonPredicate_Object.Count); i++)
            //{
            //    result += "'" + comparison_object.CommonPredicate_ObjectLabel[i] + "':{";

            //    foreach (ComparisonElement y in comparison_object.ComparisonElement)
            //    {

            //        result += "'" + y.ElementLabel + "':[";

            //        foreach (string x in y.CommonPredicateSubject_String[i])
            //        {
            //            result += ("'" + x + "',");
            //        }

            //        result = result.Remove(result.Length - 1);
            //        result += "],";
            //    }

            //    result += "},";
            //}
            //result = result.Remove(result.Length - 1);

            result += "}";

            return result;
        }

        /// <summary>
        /// returns the comparison table between two or many objects sent in the URIs String 
        /// </summary>
        /// <param name="URIs">URIs comma separated URIs to be compared</param>
        /// <returns>string of an JSON table contains the comparison between sent objects</returns>
        [WebMethod(EnableSession = false)]
        public static string getComparisonElements(string URIs)
        {
            string result;
            List<string> uri_s = (URIs.Split(',')).ToList<string>();

            Comparison comparison_object = new Comparison(uri_s);

            result = "[";

            foreach (string i in comparison_object.CommonPredicate_SubjectLabel)
            {
                result += "'" + i + "',";
            }
            result = result.Remove(result.Length);
            result += "]";

            return result;
        }

        static ObjectsRelationManager objectsRelationManager;

        /// <summary>
        /// takes the URIs as comma separated , initialize Object relation manager , returns JSON object with the 1st 5 relations 
        /// </summary>
        /// <param name="URIs">String object of URIs comma separated</param>
        /// <returns>returns a json object containing all the relations between the sent objects</returns>
        [WebMethod(EnableSession = false)]
        public static string getRelations(string URIs)
        {
            List<string> URIArray = (URIs.Split(',')).ToList<string>();
            objectsRelationManager = new ObjectsRelationManager();
            objectsRelationManager.startConnection();
            List<string> s = new List<string>();
            objectsRelationManager.generateQueries(URIArray[0], URIArray[1]);

            //just testing
            //string result = objectsRelationManager.getnext();
            string result = getNextRelation();
            while (result == "" && objectsRelationManager.IsEndOfResults != true)
            {
                result = getNextRelation();
            }

            return result;
        }

        /// <summary>
        /// returns a string of a JSON object that contains the next relation to be drawn
        /// </summary>
        /// <returns>a string of JSON object to be drawn</returns>
        [WebMethod(EnableSession = false)]
        public static string getNextRelation()
        {
            if (!objectsRelationManager.IsEndOfResults)
            {
                //jus testing
                string t = objectsRelationManager.getNextResult();
                return t;
                //return objectsRelationManager.getnext(); 
            }
            else
            {
                return "false";
            }
        }


        /// <summary>
        /// returns a string of a JSON object that contains the next relation to be drawn
        /// </summary>
        /// <param name="URIs">String object of comma separated URIs</param>
        /// <returns>returns a JSON object contains array of Results to be preview</returns>
        [WebMethod(EnableSession = false)]
        public static string previewObject(string URIs)
        {
            List<string> URIsArray = new List<string>();

            if (URIs.Contains(","))
            {
                URIsArray = URIs.Split(',').ToList<string>();
            }
            else
            {
                URIsArray.Add(URIs);
            }

            ObjectsPreviewManager objectsPreviewManager = new ObjectsPreviewManager();
            List<string> result = objectsPreviewManager.run(URIsArray[0]);

            string RelationsArray = "["; 
            
            RelationsArray+= string.Join(",", result.ToArray());

            RelationsArray += "]";

            return RelationsArray; 
        }

        #endregion

    }
}
