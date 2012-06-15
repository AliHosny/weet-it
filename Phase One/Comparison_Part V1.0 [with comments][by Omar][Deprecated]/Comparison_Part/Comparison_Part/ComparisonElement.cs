using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VDS.RDF;
using VDS.RDF.Parsing;

namespace Comparison_Part
{
    /// <summary>
    /// ComparisonElement type represents the entities involved in the comparison and holds its comparison data.
    /// </summary>
    class ComparisonElement
    {
        /// <summary>
        /// String of the URI of the comparison element.
        /// </summary>
        string elementURI;

        /// <summary>
        /// The graph node of the comparison element.
        /// </summary>
        INode elementNode;

        /// <summary>
        /// The graph of the comparison element that holds its triples in rdf format.
        /// </summary>
        Graph elementGraph;

        /// <summary>
        /// The leabel of the comparison element.
        /// </summary>
        string elementLabel;

        /// <summary>
        /// List of objects of triples that have common predicate between comparison elements and the element node is the subject of the triple.
        /// Its indexed in the same order of the common predicate list,where each index has a list of objects of that predicate.
        /// </summary>
        List<List<INode>> commonPredicateObject_Subject = new List<List<INode>>();

        /// <summary>
        /// List of subjects of triples that have common predicate between comparison elements and the element node is the object of the triple.
        /// Its indexed in the same order of the common predicate list,where each index has a list of subjects of that predicate.
        /// Its involved in "is predicate of" relation.
        /// </summary>
        List<List<INode>> commonPredicateSubject_Object = new List<List<INode>>();

        /// <summary>
        /// Constructor creates new comparison element.
        /// </summary>
        /// <param name="uri">URI of the element</param>
        /// <param name="node">Graph node of the element</param>
        /// <param name="graph">Graph of the element</param>
        public ComparisonElement(string uri, INode node, Graph graph)
        {
            elementURI = uri;
            elementNode = node;
            elementGraph = graph;
            INode label = graph.CreateUriNode(new Uri("http://www.w3.org/2000/01/rdf-schema#label"));
            List<Triple> t = new List<Triple>();
            t.AddRange(graph.GetTriplesWithSubjectPredicate(node,label));

            elementLabel = t[0].Object.ToString();
        }

        /// <summary>
        /// Returns string of comparison element URI.
        /// </summary>
        public string ElementURI
        {
            get { return elementURI; }
        }

        /// <summary>
        /// Returns node of comparison element's graph node;
        /// </summary>
        public INode ElementNode
        {
            get { return elementNode; }
        }

        /// <summary>
        /// Returns the graph of the comparison element which conatin its triples.
        /// </summary>
        public Graph ElementGraph
        {
            get { return elementGraph; }
        }

        /// <summary>
        /// Returns a string of comparison element's label.
        /// </summary>
        public string ElementLabel
        {
            get { return elementLabel; }
        }

        /// <summary>
        /// Returns list of objects of triples that have common predicate between comparison elements and the element node is the subject of the triple.
        /// or
        /// Sets list of objects of triples that have common predicate between comparison elements and the element node is the subject of the triple to a ready made list.
        /// </summary>
        public List<List<INode>> CommonPredicateObject_Subject
        {
            get { return commonPredicateObject_Subject; }
            set { commonPredicateObject_Subject = value; }
        }

        /// <summary>
        /// Returns list of subjects of triples that have common predicate between comparison elements and the element node is the object of the triple.
        /// or
        /// Sets list of subjects of triples that have common predicate between comparison elements and the element node is the object of the triple to a ready made list.
        /// </summary>
        public List<List<INode>> CommonPredicateSubject_Object
        {
            get { return commonPredicateSubject_Object; }
            set { commonPredicateSubject_Object = value; }
        }

    }
}
