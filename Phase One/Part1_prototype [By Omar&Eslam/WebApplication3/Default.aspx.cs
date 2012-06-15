using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Query;

namespace WebApplication3
{
    public partial class _Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            SparqlRemoteEndpoint endpoint = new SparqlRemoteEndpoint(new Uri("http://dbpedia.org/sparql"), "http://dbpedia.org");

            //Make a SELECT query against the Endpoinre
            SparqlResultSet results = endpoint.QueryWithResultSet(TextBox1.Text);
            resultTable.InnerHtml = "<table>";
            foreach (SparqlResult result in results)
            {
                int x=result.Count;
                
                resultTable.InnerHtml += "<tr>";
                for (int i = 0; i < x; i++)
                {   
                    resultTable.InnerHtml += "<td>" + result[i].ToString() + "</td>";
                }

                resultTable.InnerHtml += "</tr>";
                           
            }

            resultTable.InnerHtml += "</table>";
        
        }
    }
}
