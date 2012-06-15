using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Query;

using ObjectsRelationManager;

namespace Comparison_Part
{
    /// <summary>
    /// Contains all comparison elements and handles all operations to do the comparison and store each comparison element result in it.
    /// </summary>
    class Comparison
    {



        //Variables

        #region
        
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
        /// List of labels of common predicates where the subject is the comparison element
        /// </summary>
        List<string> commonPredicate_SubjectLabel = new List<string>();
        
        /// <summary>
        /// List of common properties where the object is the comparison element (relation of "is predicate of")
        /// </summary>
        List<INode> commonPredicate_Object = new List<INode>();

        /// <summary>
        /// List of labels of common properties where the object is the comparison element (relation of "is predicate of")
        /// </summary>
        List<string> commonPredicate_ObjectLabel = new List<string>();
        #endregion





        //Functions

        #region
   

       /// <summary>
        /// Creates new comprison between elements where comparison elements are represented in strings.
       /// </summary>
        /// <param name="elementURI">List of comparison element uri</param>
        public Comparison(List<string> elementURI)
        {
            comparisonElementUri = elementURI;
            CreateComparisonNodes();
            SetComparisonElementList();
            SetCommonPredicate();
            SetCommonPredicateLabel();
            SetObjectSubject_String();
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
        /// Creates list of comparison elements of type ComparisonElement given its uri,node and graph.
        /// </summary>
        void SetComparisonElementList()
        {
            Graph tempGraph;

            for (int i = 0; i < comparisonElementUri.Count; i++)
            {
                tempGraph = QueryProcessor.getGraphFromURIWithlocalHost(comparisonElementUri[i]); 
                comparisonElement.Add(new ComparisonElement(comparisonElementUri[i], comparisonNode[i], tempGraph));
            }
        }

        /// <summary>
        /// Returns list of comparison elements nodes.
        /// </summary>
        public List<INode> ComparisonNode
        {
            get { return comparisonNode; }
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
        /// Creates list of labels of common predicates between comparison elements where where the element is the subject and another list where the element is the object.
        /// </summary>
        void SetCommonPredicateLabel()
        {
            foreach (INode node in commonPredicate_Subject)
            {
                commonPredicate_SubjectLabel.Add(getLabel(node));
            }
            foreach (INode node in commonPredicate_Object)
            {
                commonPredicate_ObjectLabel.Add(getLabel(node));
            }
        }

        /// <summary>
        /// Gets label of given node.
        /// </summary>
        /// <param name="node">Node</param>
        /// <returns>String of the node label</returns>
        string getLabel(INode node)
        {
            if (node.NodeType.ToString() == "Uri")
            {
                QueryProcessor.startConnection();
                SparqlResultSet results =QueryProcessor.ExecuteQueryWithString("select ?x where {<" + node.ToString() + "> <http://www.w3.org/2000/01/rdf-schema#label> ?x FILTER (langMatches(lang(?x), \"EN\"))}");
                QueryProcessor.closeConnection();
                if (results.Count != 0)
                    return results[0].Value("x").ToString();
                else
                    return node.ToString();
            }
            else
                return node.ToString();
        }

        /// <summary>
        /// Sets the label of the values(objects or subjects) of the common predicates for each comparison element.
        /// </summary>
        void SetObjectSubject_String()
        {
            foreach (ComparisonElement element in comparisonElement)
            {
                for (int i = 0; i < commonPredicate_Subject.Count; i++)
                {
                    element.CommonPredicateObject_Node.Add(new List<INode>());
                    element.CommonPredicateObject_String.Add(new List<string>());

                    foreach (Triple t in element.ElementGraph.Triples.WithSubjectPredicate(element.ElementNode, commonPredicate_Subject[i]))
                    {
                        if (t.Predicate.Equals(commonPredicate_Subject[i]))
                        {
                            element.CommonPredicateObject_Node[i].Add(t.Object);
                        
                            element.CommonPredicateObject_String[i].Add(getLabel(t.Object));
                        }
                    }
                }

                for (int i = 0; i < commonPredicate_Object.Count; i++)
                {
                    element.CommonPredicateSubject_Node.Add(new List<INode>());
                    element.CommonPredicateSubject_String.Add(new List<string>());

                    foreach (Triple t in element.ElementGraph.Triples.WithPredicateObject(commonPredicate_Object[i], element.ElementNode))
                    {
                        if (t.Predicate.Equals(commonPredicate_Object[i]))
                        {
                            element.CommonPredicateSubject_Node[i].Add(t.Subject);
                            element.CommonPredicateSubject_String[i].Add(getLabel(t.Subject));
                        }
                    }
                }
            }
        }


        #endregion




        //setters and getters

        #region
       

        /// <summary>
        /// Returns list of comparison elements of type ComparisonElement.
        /// </summary>
        internal List<ComparisonElement> ComparisonElement
        {
            get { return comparisonElement; }
        }

      

        /// <summary>
        /// Returns list of common predicates between comparison elements where where the element is the subject.
        /// </summary>
        public List<INode> CommonPredicate_Subject
        {
            get { return commonPredicate_Subject; }
        }

        /// <summary>
        /// Returns list of labels of common predicates between comparison elements where where the element is the subject.
        /// </summary>
        public List<string> CommonPredicate_SubjectLabel
        {
            get { return commonPredicate_SubjectLabel; }
        }

        /// <summary>
        /// Returns list of common predicates between comparison elements where where the element is the object ("is predicate of").
        /// </summary>
        public List<INode> CommonPredicate_Object
        {
            get { return commonPredicate_Object; }
        }

        /// <summary>
        /// Returns list of labels of common predicates between comparison elements where where the element is the object ("is predicate of").
        /// </summary>
        public List<string> CommonPredicate_ObjectLabel
        {
            get { return commonPredicate_ObjectLabel; }
        }

        #endregion


    }
}


