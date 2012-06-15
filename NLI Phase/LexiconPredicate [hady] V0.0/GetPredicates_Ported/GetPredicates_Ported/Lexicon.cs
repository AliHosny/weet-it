﻿using System;
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
            //The list of string that holds all permutations of the input string
            List<string> toReturn = new List<string>();

            //parsing the input string
            List<string> input = new List<string>();
            input = questionLeft.Split(' ').ToList<string>();

            string temp = ""; //holds the temporary constructed string

            //The core algorithm to generate permutations
            for (int j = 1; j <= input.Count; j++)//Size of word
            {
                for (int k = 0; k < (input.Count - (j - 1)); k++) //offset
                {
                    for (int l = k; l < (j + k); l++)
                    {
                        temp += input[l] + " ";
                    }
                    //Adding to output and clearing the temp
                    toReturn.Add(temp.Remove(temp.Length - 1));
                    temp = "";
                }
            }

            //Clearing the duplicates
            toReturn = toReturn.Distinct().ToList<string>();

            return toReturn;

        }

        /// <summary>
        /// get predicates is a method in lexicon class that get all predicates objects that match some words in the Question 
        /// </summary>
        /// <param name="question">question to get matched predicates of it </param>
        /// <param name="topN">the number of top matching results to be returned, default = 10</param>
        /// <param name="Limit">the limit of the number of returned results in the query, default = 20</param>
        /// <returns>list of top matching LexiconPredicates</returns>
        public List<LexiconPredicate> getPredicates(string question , int topN = 10 , int Limit = 20 )
        {
            List<LexiconPredicate> __predicateList = new List<LexiconPredicate>();
            
            //getting all permutation of words formed from the question string
            List<string> permutationList = getPermutations(question);

            //removing permutations that most propbably wont return results and will take time in querying 
            permutationList = trimPermutations(permutationList);


            QueryHandler.startConnection();
            SparqlResultSet resultSet;

            DateTime dt = DateTime.Now;  // capturing time for testing 

            // iterating over each permutation of Question left and Query them from virtuoso and return predicate list and add them 
            foreach (string questionleft in permutationList)
            {

                string Query = "SELECT  * WHERE { {" +
                                "?predicate <http://www.w3.org/2000/01/rdf-schema#label> ?label ." +
                                "?predicate <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <http://www.w3.org/1999/02/22-rdf-syntax-ns#Property> ." +
                                "?label bif:contains '" + questionleft + "'}"+
                                "} LIMIT "+Limit+"  ";


                //SparqlResultSet resultset = QueryHandler.ExecuteQueryWithString(Query); 
                SparqlRemoteEndpoint remoteEndPoint = new SparqlRemoteEndpoint(new Uri("http://localhost:8890/sparql"));
                
                try
                {
                    //executing the Query and finding results
                    resultSet = remoteEndPoint.QueryWithResultSet(Query);

                    //iterating over matched predicates in the resultset
                    foreach (SparqlResult result in resultSet)
                    {
                        INode predicateURI = result.Value("predicate");
                        INode predicateLabel = result.Value("label");
                        LexiconPredicate tmplexiconpredicate = new LexiconPredicate();

                        // check that the predicate doesn't exists in the predicateslist 
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
                        if (!exists)
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
            List<LexiconPredicate> predicateList = scorepredicates(__predicateList, topN);


            //now interating over the final predicate list and fill the rest of it's details <Domain , Range>

            //foreach (LexiconPredicate x in predicateList)
            //{
            //    string Query = "Select distinct ?domain ?range where { "+
            //        "?predicate <http://www.w3.org/2000/01/rdf-schema#domain> ?domain"+
            //        "?rangeType  <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> ?range"+




            //}


            return predicateList;
        }

        /// <summary>
        /// scoring predicates calculating the Distance between them and the matching words 
        /// then returning the n ones who have the least distance  
        /// </summary>
        /// <param name="results">list of predicates found</param>
        /// <param name="n">number of results needed to be returned</param>
        /// <returns> the top n ones who have the least distance </returns>
        private List<LexiconPredicate> scorepredicates(List<LexiconPredicate> results, int n)
        {

            DateTime dt = DateTime.Now;
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
            util.log("finished scoring predicates in " + DateTime.Now.Subtract(dt).TotalMilliseconds + " msec");


            dt = DateTime.Now;

            results.Sort(delegate(LexiconPredicate s1, LexiconPredicate s2) { return s1.score.CompareTo(s2.score); });
            util.log("finished sorting predicates in " + DateTime.Now.Subtract(dt).TotalMilliseconds + " msec");

            if (results.Count < n) { n = results.Count; };

            foreach (LexiconPredicate result in results.GetRange(0, n))
            {
                Console.WriteLine(result.URI + "\t" + result.label + "\t" + result.QuestionMatch + "\t" + result.score);
            }

            return results.GetRange(0, n);

        }



        /// <summary>
        /// 1- a list of lexicon predicates are used instead of using Hashtables // to be discussed later 
        /// 2- without ranking methods 
        /// 3- tirivial implementation
        /// </summary>
        /// <param name="question"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public List<LexiconPredicate> findTokens(string question, string query)
        {
            List<LexiconPredicate> predicateList = new List<LexiconPredicate>();


            return null;

        }




#region helper functions 

        private static List<string> trimPermutations(List<string> m)
        {
            foreach (string x in m.ToList<string>())
            {
                Match match = Regex.Match(x, @"(^the$)|(^and$)|(^of$)", RegexOptions.IgnoreCase);
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


