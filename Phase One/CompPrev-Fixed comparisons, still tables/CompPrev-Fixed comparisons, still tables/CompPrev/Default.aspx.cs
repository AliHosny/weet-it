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

        //protected void PreviewButton_Click(object sender, EventArgs e)
        //{
        //    uri_s = (TextBox1.Text.Split(',')).ToList<string>();
        //}

        //protected void CompareButton_Click(object sender, EventArgs e)
        //{
        //    uri_s = (TextBox1.Text.Split(',')).ToList<string>();
        //    Comparison comparison_object = new Comparison(uri_s);

        //    resultbox.InnerHtml = "<table width=\"1100px\" class=\"resultTable\" Cellpadding='0px' Cellmargin='0px'>";

        //    for (int i = 0; i < (comparison_object.CommonPredicate_Subject.Count); i++)
        //    {
        //        resultbox.InnerHtml += "<tr><td>" + comparison_object.CommonPredicate_Subject[i].ToString() + "</td>";
        //        foreach (ComparisonElement y in comparison_object.ComparisonElement)
        //            resultbox.InnerHtml += ("<td>" + parse_internal_list(y.CommonPredicateObject_Subject[i]) + "</td>");

        //        resultbox.InnerHtml += "</tr>";
        //    }

        //    resultbox.InnerHtml += "<Table/>";
        // }

        static string parse_internal_list(List<INode> x)
        {
            string y = null;
            foreach (INode z in x)
            {
                y += z.ToString() + "\n";
            }

            return y;
        }

        protected void View_previewButton_Click(object sender, EventArgs e)
        {

        }

        protected void View_compareButton_Click(object sender, EventArgs e)
        {

        }

        protected void View_expandButton_Click(object sender, EventArgs e)
        {

        }

        /*protected string get_label(INode node,string endpoint1)
        {
               List<string> result_string=new List<string>();
            string uri = null;
            SparqlRemoteEndpoint endpoint = new SparqlRemoteEndpoint(new Uri(endpoint1));
           // SparqlResultSet results = endpoint.QueryWithResultSet( );
            
            //return results[0].ToString();
            return uri;

        }*/


        #region WebMethods
         [WebMethod(EnableSession = false)]
        public static string TrimString(string URIs)
        {
            URIs=URIs.Trim();
            return URIs;

        }

        /// <summary>
        /// returns the comparison table between two or many objects sent in the URIs String 
        /// </summary>
        /// <param name="URIs">URIs comma separated URIs to be compared</param>
        /// <returns>string of an html table contains the comparison between sent objects</returns>
        [WebMethod(EnableSession = false)]

        public static string getComparisonTable(string URIs)
        {
            URIs = URIs.Trim();
            string result;
            List<string> uri_s = (URIs.Split(',')).ToList<string>();
           
            Comparison comparison_object = new Comparison(uri_s);

            result = "<table width=\"1100px\" class=\"resultTable\" Cellpadding='0px' Cellmargin='0px'>";

            for (int i = 0; i < (comparison_object.CommonPredicate_SubjectLabel.Count); i++)
            {

                result += "<tr><td>" + comparison_object.CommonPredicate_SubjectLabel[i] + "</td>";
                foreach (ComparisonElement y in comparison_object.ComparisonElement)
                {
                    foreach (string x in y.CommonPredicateObject_String[i])
                    {
                        result += ("<td>" + x + "</td>");
                    }
                    result += "</tr>";
                }
            }

            result += "<Table/>";

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
            List < string > s  = new List<string > () ; 
            s.Add("http://dbpedia.org/ontology/wikiPageWikiLink");
            s.Add("http://dbpedia.org/ontology/wikiPageRedirects");
            s.Add("http://dbpedia.org/ontology/wikiPageDisambiguates");
           objectsRelationManager.generateQueries(URIArray[0], URIArray[1]);
           string result = objectsRelationManager.getNextResult();

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
                return objectsRelationManager.getNextResult(); 
            }
            else
            {
                return "false"; 
            }
        }


        #endregion

    }
}
