using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Query;
using ObjectsRelationFactory;

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
        /// List of comparison elements
        /// </summary>
        List<ComparisonElement> comparisonElement = new List<ComparisonElement>();

        /// <summary>
        /// List of uri of common predicates where the subject is the comparison element
        /// </summary>
        List<string> commonPredicate_Subject = new List<string>();

        /// <summary>
        /// List of labels of common predicates where the subject is the comparison element
        /// </summary>
        List<string> commonPredicate_SubjectLabel = new List<string>();

        /// <summary>
        /// List of uri of common properties where the object is the comparison element (relation of "is predicate of")
        /// </summary>
        List<string> commonPredicate_Object = new List<string>();

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
            SetComparisonElementList();
            SetCommonPredicate();
            SetObjectSubject_String();
        }

        /// <summary>
        /// Creates list of comparison elements of type ComparisonElement given its uri,node and graph.
        /// </summary>
        void SetComparisonElementList()
        {
            for (int i = 0; i < comparisonElementUri.Count; i++)
            {
                comparisonElement.Add(new ComparisonElement(comparisonElementUri[i]));
            }
        }



        /// <summary>
        /// Creates list of common predicates between comparison elements where where the element is the subject and another list where the element is the object.
        /// </summary>
        void SetCommonPredicate()
        {
            bool commonComparingFlag = false;

            List<SparqlResultSet> predicateWithSubjectList = new List<SparqlResultSet>();
            List<SparqlResultSet> predicateWithObjectList = new List<SparqlResultSet>();

            QueryProcessor.startConnection();
            for (int i = 0; i < comparisonElementUri.Count; i++)
            {
                ////////////
                predicateWithSubjectList.Add(QueryProcessor.ExecuteQueryWithString("select distinct ?x where{<" + comparisonElementUri[i] + "> ?x ?y. filter (?x != <http://dbpedia.org/ontology/wikiPageWikiLink>)}"));
                predicateWithObjectList.Add(QueryProcessor.ExecuteQueryWithString("select distinct ?x where{?y ?x <" + comparisonElementUri[i] + ">. filter (?x != <http://dbpedia.org/ontology/wikiPageWikiLink>) }"));
            }
            QueryProcessor.closeConnection();

            foreach (SparqlResult predicate in predicateWithSubjectList[0])
            {

                for (int i = 1; i < comparisonElement.Count; i++)
                {
                    foreach (SparqlResult anotherPredicate in predicateWithSubjectList[i])
                    {
                        if (predicate.Equals(anotherPredicate))
                        {
                            commonComparingFlag = true;
                            break;
                        }
                    }

                    if (!commonComparingFlag)
                        break;
                }


                    if (commonComparingFlag)
                    {
                        commonPredicate_Subject.Add(predicate.Value("x").ToString());
                        commonPredicate_SubjectLabel.Add(QueryProcessor.getLabelIfAny(predicate.Value("x").ToString()));
                    }

                    commonComparingFlag = false;
            }

            foreach (SparqlResult predicate in predicateWithObjectList[0])
            {

                for (int i = 1; i < comparisonElement.Count; i++)
                {
                    foreach (SparqlResult anotherPredicate in predicateWithObjectList[i])
                    {
                        if (predicate.Equals(anotherPredicate))
                        {
                            commonComparingFlag = true;
                            break;
                        }
                    }

                    if (!commonComparingFlag)
                        break;
                }


                if (commonComparingFlag)
                {
                    commonPredicate_Object.Add(predicate.Value("x").ToString());
                    commonPredicate_ObjectLabel.Add(QueryProcessor.getLabelIfAny(predicate.Value("x").ToString()));
                }

                commonComparingFlag = false;
            }

            commonPredicate_Subject = commonPredicate_Subject.Distinct().ToList();
            commonPredicate_SubjectLabel = commonPredicate_SubjectLabel.Distinct().ToList();

            commonPredicate_Object = commonPredicate_Object.Distinct().ToList();
            commonPredicate_ObjectLabel = commonPredicate_ObjectLabel.Distinct().ToList();

            
        }


        


        /// <summary>
        /// Sets the label of the values(objects or subjects) of the common predicates for each comparison element.
        /// </summary>
        void SetObjectSubject_String()
        {
            SparqlResultSet temp;

            QueryProcessor.startConnection();

            for (int x = 0; x < comparisonElement.Count;x++)
            {
                for (int i = 0; i < commonPredicate_SubjectLabel.Count; i++)
                {
                    comparisonElement[x].CommonPredicateObject_String.Add(new List<string>());

                    temp = QueryProcessor.ExecuteQueryWithString("select distinct ?x where{<" + comparisonElement[x].ElementURI + "> <" + commonPredicate_Subject[i] + "> ?x}");

                    for (int j = 0; j < temp.Count; j++)
                    {
                        comparisonElement[x].CommonPredicateObject_String[i].Add(QueryProcessor.getLabelIfAny(temp[j].Value("x").ToString()));
                    }
                }

                for (int i = 0; i < commonPredicate_ObjectLabel.Count; i++)
                {
                    comparisonElement[x].CommonPredicateSubject_String.Add(new List<string>());

                    temp = QueryProcessor.ExecuteQueryWithString("select distinct ?x where{?x <" + commonPredicate_Object[i] + "> <" + comparisonElement[x].ElementURI + ">}");

                    for (int j = 0; j < temp.Count; j++)
                    {
                        comparisonElement[x].CommonPredicateSubject_String[i].Add(QueryProcessor.getLabelIfAny(temp[j].Value("x").ToString()));
                    }
                }
            }

            QueryProcessor.closeConnection();
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


        public List<string> CommonPredicate_Subject
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

        public List<string> CommonPredicate_Object
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


