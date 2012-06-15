using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using VDS.RDF;
using VDS.RDF.Query;


namespace GetPredicates_Ported
{
    class Lexicon
    {

        /// <summary>
        /// get predicates is a method in lexicon class that get all predicates objects that match some words in the Question 
        /// </summary>
        /// <param name="question">question to get matched predicates of it </param>
        /// <param name="topN">the number of top matching results to be returned, default = 10</param>
        /// <param name="Limit">the limit of the number of returned results in the query, default = 20</param>
        /// <returns>list of top matching LexiconPredicates</returns>
        public List<LexiconPredicate> getPredicates(string question, int topN, int Limit)
        {
            DateTime dt = DateTime.Now;  // capturing time for testing 

            List<LexiconPredicate> __predicateList = new List<LexiconPredicate>();

            //getting all permutation of words formed from the question string
            List<string> permutationList = getPermutations(question);

            //removing permutations that most propbably wont return results and will take time in querying 
            permutationList = trimPermutations(permutationList);



            // iterating over each permutation of Question left and Query them from virtuoso and return predicate list and add them 
            foreach (string questionleft in permutationList)
            {

                string Query = "SELECT  * WHERE { { " +
                                "?predicate <http://www.w3.org/2000/01/rdf-schema#label> ?label ." +
                                "?predicate <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <http://www.w3.org/2002/07/owl#DatatypeProperty>." +
                                "?label bif:contains '\"" + questionleft + "\"'}" +
                                "union {" +
                                "?predicate <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <http://www.w3.org/2002/07/owl#ObjectProperty> ." +
                                "?predicate <http://www.w3.org/2000/01/rdf-schema#label> ?label ." +
                               "?label bif:contains '\"" + questionleft + "\"'}" +

                                "union {" +
                                "?predicate <http://www.w3.org/1999/02/22-rdf-syntax-ns#type>  <http://www.w3.org/1999/02/22-rdf-syntax-ns#Property>  ." +
                                "?predicate <http://www.w3.org/2000/01/rdf-schema#label> ?label ." +
                               "?label bif:contains '\"" + questionleft + "\"'}" +

                                "} limit " + Limit;

                //string Query1 = "SELECT  * WHERE { { " +
                //                "?predicate <http://www.w3.org/2000/01/rdf-schema#label> ?label ." +
                //                "?predicate <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <http://www.w3.org/2002/07/owl#DatatypeProperty>." +
                //                "?label bif:contains '\"" + questionleft + "\"'} " +
                //                "} limit " + Limit;

                //string Query2 = "SELECT  * WHERE { { " +
                //                "?predicate <http://www.w3.org/2000/01/rdf-schema#label> ?label ." +
                //                "?predicate <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <http://www.w3.org/2002/07/owl#ObjectProperty>." +
                //                "?label bif:contains '\"" + questionleft + "\"'} " +
                //                "} limit " + Limit;

                //string Query3 = "SELECT  * WHERE { { " +
                //                "?predicate <http://www.w3.org/2000/01/rdf-schema#label> ?label ." +
                //                "?predicate <http://www.w3.org/1999/02/22-rdf-syntax-ns#type>  <http://www.w3.org/1999/02/22-rdf-syntax-ns#Property>." +
                //                "?label bif:contains '\"" + questionleft + "\"'} " +
                //                "} limit " + Limit;

                //SparqlResultSet resultset = QueryHandler.ExecuteQueryWithString(Query); 

                SparqlRemoteEndpoint remoteEndPoint = new SparqlRemoteEndpoint(new Uri("http://localhost:8890/sparql"));

                try
                {
                    //executing the Query and finding results
                    SparqlResultSet resultSet = remoteEndPoint.QueryWithResultSet(Query);
                    //resultSet.Concat(remoteEndPoint.QueryWithResultSet(Query2));
                    //resultSet.Concat(remoteEndPoint.QueryWithResultSet(Query3));

                    //iterating over matched predicates in the resultset
                    foreach (SparqlResult result in resultSet)
                    {
                        INode predicateURI = result.Value("predicate");
                        INode predicateLabel = result.Value("label");
                        LexiconPredicate tmplexiconpredicate = new LexiconPredicate();

                        // check that the property is used .. not a non-used property 
                        bool hasResuts = false;
                        string checkQuery = "select distinct * where { ?x <" + predicateURI + "> ?y } limit 1 ";
                        QueryHandler.startConnection();
                        SparqlResultSet checkResults = QueryHandler.ExecuteQueryWithString(checkQuery);
                        QueryHandler.closeConnection();

                        if (checkResults.Count != 0)
                        {
                            hasResuts = true;
                        }

                        // check that the predicate doesn't exists in the predicateslist before 
                        bool exists = false;
                        foreach (LexiconPredicate x in __predicateList)
                        {
                            if (x.URI == predicateURI.ToString())
                            {
                                exists = true;
                                break;
                            }
                        }

                        // adding the new predicate to the __predicatelist 
                        if (!exists && hasResuts)
                        {
                            tmplexiconpredicate.URI = predicateURI.ToString();
                            tmplexiconpredicate.QuestionMatch = questionleft;
                            tmplexiconpredicate.label = predicateLabel.ToString();
                            __predicateList.Add(tmplexiconpredicate);
                        }
                    }


                }

                // skipping results that raised timeout exceptions
                catch
                {
                    util.log("skipped : " + questionleft + " ---- due to time out ");
                }
            }

            util.log(" finished getting " + __predicateList.Count + " predicates " + " Time taken : " + DateTime.Now.Subtract(dt).TotalMilliseconds + " msec");

            // now done of collecting predicates scoring them down and get the best n ones 
            List<LexiconPredicate> predicateList = scorePredicates(__predicateList, topN);

            predicateList = addDomainAndRange(predicateList);

            util.log("total time taken :" + DateTime.Now.Subtract(dt).TotalMilliseconds.ToString() + " msecs ");

            return predicateList;
        }

        /// <summary>
        /// get predicates is a method in lexicon class that get all LexiconLiterals objects that match some words in the Question 
        /// </summary>
        /// <param name="question">question to get matched predicates of it </param>
        /// <param name="topN">the number of top matching results to be returned, default = 10</param>
        /// <param name="Limit">the limit of the number of returned results in the query, default = 20</param>
        /// <returns>list of top matching LexiconLiterals with it's type of owner and predicate </returns>
        public List<LexiconLiteral> getLiterals(string question, int topN = 10, int Limit = 20)
        {
            DateTime dt = DateTime.Now;  // capturing time for testing 
            List<LexiconLiteral> __literalList = new List<LexiconLiteral>();

            //getting all permutation of words formed from the question string
            List<string> permutationList = getPermutations(question);

            //removing permutations that most propbably wont return results and will take time in querying 
            permutationList = trimPermutations(permutationList);


            // iterating over each permutation of Question left and Query them from virtuoso and return predicate list and add them 
            foreach (string questionleft in permutationList)
            {

                // Query 1 is suitable for Keywords like : United states which match the most popular resource 
                string Query1 = "SELECT distinct ?subject ?literal ?typeOfOwner " +
                               "where {              " +
                                "        ?subject <http://www.w3.org/2000/01/rdf-schema#label> ?literal . " +
                                " optional { ?subject <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> ?typeOfOwner ." +
                                "        ?literal bif:contains '\"" + questionleft + "\"' OPTION (score ?sc) .  " +
                                "FILTER (" +
                                "!(?typeOfOwner  = <http://www.w3.org/2002/07/owl#Thing> " +
                                "|| ?typeOfOwner = <http://www.w3.org/2004/02/skos/core#Concept> " +
                                "|| ?typeOfOwner = <http://www.w3.org/2002/07/owl#ObjectProperty> " +
                                "|| ?typeOfOwner = <http://www.w3.org/1999/02/22-rdf-syntax-ns#Property> " +
                                "|| ?typeOfOwner = <http://www.w3.org/2002/07/owl#DatatypeProperty>) " +
                                ")" +
                                "} limit " + Limit;

                // Query2 is suitable for Keywords like : USA  , which match the redirections 
                string Query2 = "SELECT distinct ?subject ?literal ?typeOfOwner " +
                                "WHERE { " +
                                "      ?subject2 <http://www.w3.org/2000/01/rdf-schema#label> ?literal . " +
                                "      ?subject2 <http://dbpedia.org/ontology/wikiPageRedirects> ?subject ." +
                                "      ?subject  <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> ?typeOfOwner ." +
                                "      ?literal bif:contains '\"" + questionleft + "\"' OPTION (score ?sc) ." +
                                "FILTER (" +
                                "!(?typeOfOwner  = <http://www.w3.org/2002/07/owl#Thing> " +
                                "|| ?typeOfOwner = <http://www.w3.org/2004/02/skos/core#Concept> " +
                                "|| ?typeOfOwner = <http://www.w3.org/2002/07/owl#ObjectProperty> " +
                                "|| ?typeOfOwner = <http://www.w3.org/1999/02/22-rdf-syntax-ns#Property> " +
                                "|| ?typeOfOwner = <http://www.w3.org/2002/07/owl#DatatypeProperty>) " +
                                ")" +
                                "} limit " + Limit;

                SparqlRemoteEndpoint remoteEndPoint = new SparqlRemoteEndpoint(new Uri("http://localhost:8890/sparql"));
                SparqlResultSet resultSet1 = new SparqlResultSet();
                SparqlResultSet resultSet2 = new SparqlResultSet();
                List<SparqlResult> resultSet = new List<SparqlResult>();
                try
                {
                    //executing the Query and finding results
                    resultSet1 = remoteEndPoint.QueryWithResultSet(Query1);
                }
                // skipping results that raised timeout exceptions
                catch
                {
                    util.log("skipped Query1 : " + questionleft + " ---- due to time out ");
                }
                try
                {
                    resultSet2 = remoteEndPoint.QueryWithResultSet(Query2);
                }
                // skipping results that raised timeout exceptions
                catch
                {
                    util.log("skipped  Query2: " + questionleft + " ---- due to time out ");
                }

                resultSet = (resultSet1.Count != 0) ? resultSet1.ToList<SparqlResult>() : resultSet;
                resultSet = (resultSet2.Count != 0) ? resultSet.Concat<SparqlResult>(resultSet2.ToList<SparqlResult>()).ToList() : resultSet;



                //iterating over matched Literals in the resultset
                foreach (SparqlResult result in resultSet)
                {
                    INode resourceURI = result.Value("subject");
                    INode literalLabel = result.Value("literal");
                    INode literalTypeOfOwner = result.Value("typeOfOwner");

                    // check that the predicate doesn't exists in the predicateslist before 
                    bool exists = false;          // URI + Label only Exists
                    bool totallyExists = false;   // URI + Label + TypeofOwner exists in the literal list 
                    foreach (LexiconLiteral x in __literalList)
                    {
                        if (x.URI == resourceURI.ToString() && x.label == literalLabel.ToString())
                        {
                            exists = true;
                            if (x.typeOfOwner.Contains(literalTypeOfOwner.ToString()))
                            {
                                totallyExists = true;
                                break;
                            }

                        }
                    }

                    // adding the new literals to the literallist . 
                    if (!exists)
                    {
                        LexiconLiteral tmpLexiconLiteral = new LexiconLiteral(resourceURI.ToString(), literalLabel.ToString(), questionleft, literalTypeOfOwner.ToString());
                        __literalList.Add(tmpLexiconLiteral);
                    }

                    if (!totallyExists && exists)
                    {
                        foreach (LexiconLiteral let in __literalList)
                        {
                            if (let.URI == resourceURI.ToString() && let.label == literalLabel.ToString())
                            {
                                let.typeOfOwner.Add(literalTypeOfOwner.ToString());
                            }
                        }
                    }
                }



            }


            //scoring literals . trimming duplicates , 
            __literalList = scoreLiterals(__literalList, topN);

            util.log("total time taken :" + DateTime.Now.Subtract(dt).TotalMilliseconds.ToString() + " msecs ");
            return __literalList;


        }



        /// <summary>
        /// scoring predicates calculating the Distance between them and the matching words 
        /// then returning the n ones who have the least distance  
        /// </summary>
        /// <param name="results">list of predicates found</param>
        /// <param name="n">number of results needed to be returned</param>
        /// <returns> the top n ones who have the least distance </returns>
        private List<LexiconPredicate> scorePredicates(List<LexiconPredicate> results, int n)
        {

            foreach (LexiconPredicate predicate in results)
            {
                // adding a levenshtein score to each one of them where predicates of high score will make a bad match
                // removing the @en in the end of each label 
                string tmplabel;

                //use match instead regex 
                if (predicate.label.EndsWith("@en"))
                {
                    tmplabel = predicate.label.Remove(predicate.label.Length - 3);
                }
                else
                {
                    tmplabel = predicate.label;
                }

                // sending Questionmatch and label to find the levenshtein distance between them 
                predicate.score = util.computeLevenshteinDistance(predicate.QuestionMatch, tmplabel);
            }

            results.Sort(delegate(LexiconPredicate s1, LexiconPredicate s2) { return s1.score.CompareTo(s2.score); });

            if (results.Count < n) { n = results.Count; };

            foreach (LexiconPredicate result in results.GetRange(0, n))
            {
                Console.WriteLine(result.URI + "\t" + result.label + "\t" + result.QuestionMatch + "\t" + result.score);
            }

            return results.GetRange(0, n);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="results"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        private List<LexiconLiteral> scoreLiterals(List<LexiconLiteral> results, int n)
        {
            // adding scores to all literals 
            foreach (LexiconLiteral x in results)
            {
                string tmplabel;
                if (x.label.EndsWith("@en"))
                {
                    tmplabel = x.label.Remove(x.label.Length - 3);
                }
                else
                {

                    tmplabel = x.label;
                }

                x.score = util.computeLevenshteinDistance(x.QuestionMatch, tmplabel);
            }

            //removing duplicates that have the same resource
            foreach (LexiconLiteral x in results.ToList())
            {
                foreach (LexiconLiteral y in results.ToList())
                {
                    // if the URI exists before 
                    if (x.URI == y.URI && x.label != y.label)
                    {
                        // removing the one of the larger distance 
                        results.Remove((x.score >= y.score) ? y : x);
                    }
                }
            }

            // results.Distinct(delegate(LexiconLiteral l1, LexiconLiteral l2) { return (l1.score >= l2.score) ? l2 : l1 }); 


            // sorting the literals depending on the score 
            results.Sort(delegate(LexiconLiteral s1, LexiconLiteral s2) { return s1.score.CompareTo(s2.score); });

            if (results.Count < n)
            {
                n = results.Count;
            }




            return results.GetRange(0, n);

        }


        /// <summary>
        /// takes the list of Predicates and add tothem the domain and Range data needed  in two steps 
        /// 1st : searching for direct Domain and ranges 
        /// 2nd : searching for predicates who don't have domain and ranges and select the all predicates and objects 
        /// get their types and add them to the domain and range field 
        /// </summary>
        /// <param name="predicateList">the predicate list without the domain and range data</param>
        /// <returns>the predicate list with the predicates have domain and ranges</returns>
        private List<LexiconPredicate> addDomainAndRange(List<LexiconPredicate> predicateList)
        {
            //now interating over the final predicate list and fill the rest of it's details <Domain , Range>  
            // step 1 : the Direct Way 
            foreach (LexiconPredicate x in predicateList)
            {
                string Query = "Select distinct ?domain ?range where { {" +

                    "<" + x.URI + ">" + "<http://www.w3.org/2000/01/rdf-schema#domain> ?domain.}" +
                    "union { <" + x.URI + ">" + " <http://www.w3.org/2000/01/rdf-schema#range> ?range ." +
                    "}}";

                QueryHandler.startConnection();
                SparqlResultSet resulSet = QueryHandler.ExecuteQueryWithString(Query);
                QueryHandler.closeConnection();

                if (resulSet.Count() != 0)
                {
                    foreach (SparqlResult result in resulSet)
                    {
                        // check that the domain field is not empty  // and check that this domain wasn't added before 
                        if (result.Value("domain") != null)
                            if (!x.domains.Contains(result.Value("domain").ToString()))
                            {
                                x.domains.Add(result.Value("domain").ToString());
                            }

                        // check that the range field is not empty  // and check that this range wasn't added before 
                        if (result.Value("range") != null)
                            if (!x.ranges.Contains(result.Value("range").ToString()) && result.Value("range") != null)
                            {
                                x.ranges.Add(result.Value("range").ToString());
                            }
                    }
                }
            }


            //step2 : the inDirect Way -- for predicates that didn't have a Domain or range selected before 
            foreach (LexiconPredicate x in predicateList)
            {

                bool hasDomain = (x.domains.Count == 0) ? false : true;
                bool hasRange = (x.ranges.Count == 0) ? false : true;

                if (!hasDomain && !hasRange)
                {
                    continue;
                }

                string Query = "Select distinct ";
                Query += (hasDomain) ? "" : "?domain";
                Query += (hasRange) ? "" : "?range";
                Query += " where {{";

                if (!hasDomain)
                {
                    Query += "{ ?X <" + x.URI + "> ?Y ." +
                             "?X a ?domain . " +
                             "filter(!REGEX(STR(?domain) ,'http://www.w3.org/2002/07/owl#Thing','i'))" +
                             "}";
                }

                Query += "}";

                if (!hasRange)
                {
                    Query += "union { ?X <" + x.URI + "> ?Y ." +
                             "?Y a ?range . " +
                             "filter(!REGEX(STR(?range) ,'http://www.w3.org/2002/07/owl#Thing','i'))" +
                             "}";
                }

                Query += "}";

                Query += "limit 20 ";




                QueryHandler.startConnection();
                SparqlResultSet resulSet = QueryHandler.ExecuteQueryWithString(Query);
                QueryHandler.closeConnection();

                if (resulSet.Count() != 0)
                {
                    foreach (SparqlResult result in resulSet)
                    {
                        // check that the domain field is not empty  // and check that this domain wasn't added before 
                        if (!hasDomain)
                        {
                            if (result.Value("domain") != null)
                                if (!x.domains.Contains(result.Value("domain").ToString()))
                                {
                                    x.domains.Add(result.Value("domain").ToString());
                                }
                        }
                        // check that the range field is not empty  // and check that this range wasn't added before 
                        if (!hasRange)
                        {
                            if (result.Value("range") != null)
                                if (!x.ranges.Contains(result.Value("range").ToString()) && result.Value("range") != null)
                                {
                                    x.ranges.Add(result.Value("range").ToString());
                                }
                        }
                    }
                }
            }

            return predicateList;

        }



        #region helper functions
        /// <summary>
        /// This function returns all permutations of a string
        /// ex: father of barack obama
        /// 1st iteration: father, of, barack, obama
        /// 2nd iteration: father of, of barack, barack obama
        /// 3rd iteration: father of barack, of barack obama
        /// 4th iteration: father of barack obama
        /// </summary>
        /// <param name="questionLeft">The certain string to enter</param>
        /// <returns></returns>
        public List<string> getPermutations(string questionLeft)
        {
            //Trimming the string 
            questionLeft = questionLeft.Trim();
            //Removing Extra spaces
            while (questionLeft.Contains("  "))
                questionLeft = questionLeft.Replace("  ", " ");


            //The list of string that holds all permutations of the input string
            List<string> toReturn = new List<string>();

            //parsing the input string
            List<string> input = new List<string>();
            input = questionLeft.Split(' ').ToList<string>();

            string temp = ""; //holds the temporary constructed string
            string temponeWord = "";
            string tempUScore = "";
            //The core algorithm to generate permutations
            for (int j = 1; j <= input.Count; j++)//Size of word
            {
                for (int k = 0; k < (input.Count - (j - 1)); k++) //offset
                {
                    for (int l = k; l < (j + k); l++)
                    {
                        temp += input[l] + " ";
                        temponeWord += input[l];
                        tempUScore += input[l] + "_";

                    }
                    //Adding to output and clearing the temp
                    toReturn.Add(temp.Remove(temp.Length - 1));
                    toReturn.Add(temponeWord);
                    toReturn.Add(tempUScore.Remove(temp.Length - 1));
                    temp = "";
                    tempUScore = "";
                    temponeWord = "";
                }
            }

            //Clearing the duplicates
            toReturn = toReturn.Distinct().ToList<string>();

            return toReturn;

        }
        /// <summary>
        /// after getting all permutations this function is to remove 
        /// permutations that may take time during the Query and return no results
        /// </summary>
        /// <param name="m">List of permutations</param>
        /// <returns>List of permuations after trimming </returns>
        private static List<string> trimPermutations(List<string> m)
        {
            foreach (string x in m.ToList<string>())
            {
                Match match = Regex.Match(x, @"(^the$)|(^and$)|(^of$)|(^that$)", RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    m.Remove(x);
                }

            }
            return m;

        }
        #endregion

    }

}


