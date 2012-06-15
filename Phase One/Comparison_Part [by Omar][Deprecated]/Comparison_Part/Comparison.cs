using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VDS.RDF;
using VDS.RDF.Parsing;

namespace Comparison_Part
{
    /// <summary>
    /// Contains all comparison elements and handles all operations to do the comparison and store each comparison element result in it.
    /// </summary>
    class Comparison
    {
        /// <summary>
        /// Input list of string uri of comparison elements
        /// </summary>
        List<string> comparisonElementUri;

        /// <summary>
        /// Input list of type INode has method to set it from existing uri string
        /// </summary>
        List<INode> comparisonNode = new List<INode>();

        /// <summary>
        /// List of comparison elements
        /// </summary>
        List<ComparisonElement> comparisonElement = new List<ComparisonElement>();

        /// <summary>
        /// List of common predicates where the subject is the comparison element
        /// </summary>
        List<INode> commonPredicate_Subject = new List<INode>();
        
        /// <summary>
        /// List of common properties where the object is the comparison element (relation of "is predicate of")
        /// </summary>
        List<INode> commonPredicate_Object = new List<INode>();

        /// <summary>
        /// Creates new comprison between elements.
        /// </summary>
        /// <param name="elementURI">List of comparison element uri</param>
        public Comparison(List<string> elementURI)
        {
            comparisonElementUri = elementURI;
            CreateComparisonNodes();
            SetComparisonElementList();
            SetCommonPredicate();
            SetObjectSubject();
        }

        /// <summary>
        /// Returns list of comparison elements nodes.
        /// </summary>
        public List<INode> ComparisonNode
        {
            get { return comparisonNode; }
        }

        /// <summary>
        /// Create list of comparison elements nodes from the list of comparison elements uri.
        /// </summary>
        void CreateComparisonNodes()
        {
            Graph g = new Graph();
            IUriNode select;

            foreach (string s in comparisonElementUri)
            {
                select = g.CreateUriNode(new Uri(s));
                comparisonNode.Add(select);
            }
        }


        /// <summary>
        /// Returns list of comparison elements of type ComparisonElement.
        /// </summary>
        internal List<ComparisonElement> ComparisonElement
        {
            get { return comparisonElement; }
        }

        /// <summary>
        /// Creates list of comparison elements of type ComparisonElement given its uri,node and graph.
        /// </summary>
        void SetComparisonElementList()
        {
            Graph tempGraph;

            for (int i = 0; i < comparisonElementUri.Count; i++)
            {
                tempGraph = new Graph();
                UriLoader.Load(tempGraph, new Uri(comparisonElementUri[i]));
                comparisonElement.Add(new ComparisonElement(comparisonElementUri[i], comparisonNode[i], tempGraph));
            }
        }

        /// <summary>
        /// Returns list of common predicates between comparison elements where where the element is the subject.
        /// </summary>
        public List<INode> CommonPredicate_Subject
        {
            get { return commonPredicate_Subject; }
        }

        /// <summary>
        /// Returns list of common predicates between comparison elements where where the element is the object ("is predicate of").
        /// </summary>
        public List<INode> CommonPredicate_Object
        {
            get { return commonPredicate_Object; }
        }

        /// <summary>
        /// Creates list of common predicates between comparison elements where where the element is the subject and another list where the element is the object.
        /// </summary>
        void SetCommonPredicate()
        {
            bool commonComparingFlag = false;
            bool notExist = true;

            foreach (Triple firstTriple in comparisonElement[0].ElementGraph.Triples)
            {

                for (int i = 1; i < comparisonElement.Count; i++)
                {
                    foreach (Triple anotherTriple in comparisonElement[i].ElementGraph.Triples)
                    {
                        if (anotherTriple.Predicate.Equals(firstTriple.Predicate))
                        {
                            commonComparingFlag = true;
                            break;
                        }
                    }

                    if (!commonComparingFlag)
                        break;
                }


                if (firstTriple.Subject.Equals(comparisonNode[0]))
                {
                    foreach (INode node in commonPredicate_Subject)
                    {
                        if (node.Equals(firstTriple.Predicate))
                            notExist = false;
                    }

                    if (commonComparingFlag && notExist)
                        commonPredicate_Subject.Add(firstTriple.Predicate);
                }
                else if (firstTriple.Object.Equals(comparisonNode[0]))
                {
                    foreach (INode node in commonPredicate_Object)
                    {
                        if (node.Equals(firstTriple.Predicate))
                            notExist = false;
                    }

                    if (commonComparingFlag && notExist)
                        commonPredicate_Object.Add(firstTriple.Predicate);
                }

                notExist = true;
                commonComparingFlag = false;
            }
        }

        /// <summary>
        /// Sets the values(objects or subjects) of the common predicates for each comparison element.
        /// </summary>
        void SetObjectSubject()
        {
            foreach (ComparisonElement element in comparisonElement)
            {
                for (int i=0;i<commonPredicate_Subject.Count;i++)
                {
                    element.CommonPredicateObject_Subject.Add(new List<INode>());

                    foreach(Triple t in element.ElementGraph.Triples.WithSubjectPredicate(element.ElementNode,commonPredicate_Subject[i]))
                    {
                        if (t.Predicate.Equals(commonPredicate_Subject[i]))
                            element.CommonPredicateObject_Subject[i].Add(t.Object);
                    }
                }

                for (int i = 0; i < commonPredicate_Object.Count; i++)
                {
                    element.CommonPredicateSubject_Object.Add(new List<INode>());

                    foreach (Triple t in element.ElementGraph.Triples.WithPredicateObject(commonPredicate_Object[i], element.ElementNode))
                    {
                        if (t.Predicate.Equals(commonPredicate_Object[i]))
                            element.CommonPredicateSubject_Object[i].Add(t.Subject);
                    }
                }
            }
        }
    }
}


