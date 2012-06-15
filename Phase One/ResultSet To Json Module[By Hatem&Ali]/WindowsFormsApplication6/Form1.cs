using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using VDS.RDF;
using VDS.RDF.Query;

namespace WindowsFormsApplication6
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            ResSetToJSON.innerResult inner = new ResSetToJSON.innerResult();
            inner.firstObj = "http://dbpedia.org/ontology/purpose";
            inner.lastObj = "http://dbpedia.org/ontology/supplementalDraftRound";
            inner.resultSets = doQuery();
            List<ResSetToJSON.innerResult> b = new List<ResSetToJSON.innerResult>();
            b.Add(inner);
            string s = "";
            s=ResSetToJSON.ToJsonObj(b);
            textBox3.Text = s;
        }
        public SparqlResultSet doQuery()
        {
            string query = "select * where {<http://dbpedia.org/ontology/purpose> ?x ?y}";
            SparqlRemoteEndpoint endpoint = new SparqlRemoteEndpoint(new Uri("http://dbpedia.org/sparql"));
            SparqlResultSet set = endpoint.QueryWithResultSet(query);
            List<SparqlResultSet> listset = new List<SparqlResultSet>();
            listset.Add(set);
            //SparqlResultSet set2 = endpoint.QueryWithResultSet("select * where {<http://dbpedia.org/ontology/supplementalDraftRound> ?x ?y}");
            //listset.Add(set2);
            return listset[0];
        }
        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }     // get the edges of each query
        
    
    }
}
