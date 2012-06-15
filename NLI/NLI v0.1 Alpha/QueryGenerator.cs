﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace NLI
{
    class QueryGenerator
    {
        //  private QueryStorage sparqlQueries;
        private string parsedQuestion;
        private string question;
        private int questionType;
        private Lexicon lexicon;
        // todo : object of kind Queries holder

        public QueryGenerator(string question)
        {
            lexicon = new Lexicon();

            this.question = question.ToLower();
            parsedQuestion = this.question;

            // preprocessing of parsedQuestion
            QuestionPreprocessing();

            // compute the sparql query
            buildQueries();
        }

        public void QuestionPreprocessing()
        {
            Regex tempRegex;
            string substr;
            bool tempBool;

            //remove possible question marks
            parsedQuestion = parsedQuestion.Replace("?", "");

            //remove possible dots
            parsedQuestion = parsedQuestion.Replace(".", "");

            //remove [ and ]
            parsedQuestion = parsedQuestion.Replace("[", "");
            parsedQuestion = parsedQuestion.Replace("]", "");

            //replace commas
            parsedQuestion = parsedQuestion.Replace(",", " ");

            //replace two spaces with one
            parsedQuestion = parsedQuestion.Replace("  ", " ");


            //starting and ending quotes
            while ((parsedQuestion.StartsWith("'") && parsedQuestion.EndsWith("'")) || (parsedQuestion.StartsWith("\"") && parsedQuestion.EndsWith("\"")))
                parsedQuestion = parsedQuestion.Substring(1, parsedQuestion.Length - 1);

            //mark names (words quoted), removes quotes and if the name contains spaces it replace it with underscores '_'
            tempRegex = new Regex(".*'(.*)'.*");
            tempBool = tempRegex.IsMatch(parsedQuestion);

            while (tempBool)
            {
                tempBool = false;
                substr = tempRegex.Match(parsedQuestion).Groups[1].ToString();
                parsedQuestion = parsedQuestion.Replace("'" + substr + "'", substr.Replace(" ", "_"));
                tempBool = tempRegex.IsMatch(parsedQuestion);
            }

            //remove more than one spaces
            while (parsedQuestion.Contains("  "))
                parsedQuestion = parsedQuestion.Replace("  ", " ");

            //////////Ambiguous part
            //tempRegex = new Regex("\\^'(.*)'\\$");
            //parsedQuestion = tempRegex.Replace(parsedQuestion, "$1");  //parsedQuestion = parsedQuestion.replaceAll("\\^'(.*)'\\$","$1");

            //todo :
            //try to find out the question type  
            //int[] tmpType = new int[1];
            //parsedQuestion = QuestionType.getQuestionType(parsedQuestion, tmpType);
            //questionType = tmpType[0];

            //todo : 
            //look for names and mark them with underscores instead of spaces
            //string testString = "";
            //string[] words = parsedQuestion.Split(' ');
            //for (int i = words.Length; i > 1; i--)
            //{
            //    for (int j = 0; j < (words.Length - i + 1); j++)
            //    {
            //        testString = "";
            //        for (int k = 0; k < i; k++)
            //        {
            //            if (testString.Length > 0) testString = testString + " ";
            //            testString = testString + words[k + j];
            //        }
            //        if (lexicon.isName(testString)) parsedQuestion = parsedQuestion.Replace(testString, testString.Replace(" ", "_")); /////isName need to be implemented (Lexicon method)
            //    }
            //}

            //todo : 
            //try to replace matchstrings with classes with the classname itself
            //parsedQuestion = lexicon.replaceMatchstringWithClassname(parsedQuestion);

            //todo : 
            //remove occurences of 'the' 
            //tempRegex = new Regex("([\\s|^])the([\\s|$])");
            //parsedQuestion = tempRegex.Replace(parsedQuestion, "");

            parsedQuestion = parsedQuestion.Trim();
        }

        private void buildQueries()
        {

            List<LexiconPredicate> predicateList = lexicon.getPredicates(parsedQuestion); //find all matching predicates
            List<LexiconLiteral> literalList = new List<LexiconLiteral>();
            List<QueryBucket> queryBuckets = new List<QueryBucket>();
            QueryBucket tmpBucket = new QueryBucket(parsedQuestion);

            queryBuckets.Add(tmpBucket);

            // first of all create an ArrayList containing as much QueryBuckets as there are predicates in our predicateList
            foreach (LexiconPredicate predicate in predicateList)
            {
                //create a new QueryBucket object to be added in the end of the for loop 
                tmpBucket = new QueryBucket(parsedQuestion);

                string[] tmpWords = parsedQuestion.Split(' ');

                // if any of the words in the Question matches the domain of the predicate , adding it to the Question match 
                foreach (string word in tmpWords)
                {
                    if (util.match(word, util.URIToSimpleString(predicate.domains)) && !predicate.QuestionMatch.Contains(word))
                    {
                        if (predicate.QuestionMatch.Length > 0) predicate.QuestionMatch += " ";
                        predicate.QuestionMatch += word;
                    }
                }


                // if any of the words in the Question matches the range of the predicate , adding it to the Question match 
                foreach (string word in tmpWords)
                {
                    if (util.match(word, util.URIToSimpleString(predicate.ranges)) && !predicate.QuestionMatch.Contains(word))
                    {
                        if (predicate.QuestionMatch.Length > 0) predicate.QuestionMatch += " ";
                        predicate.QuestionMatch += word;
                    }
                }

                tmpBucket.add(predicate);
                //now add the bucket to the queryBuckets ArrayList
                queryBuckets.Add(tmpBucket);
            }



            /* 
             * and now for each QueryBucket search for all the 
             * combination possibilities among the predicates
            */

            util.log("search for combination possibilities among predicates");
            util.log("loop over " + queryBuckets.Count);
            
            foreach (QueryBucket bucket in queryBuckets.ToList())
            {
                bool somethingHappened = false;


                if (bucket.questionLeft.Length > 0)
                {
                    // getting all the new predicates for the Question left 

                    foreach (LexiconPredicate predicate in predicateList.ToList())
                    {

                        //only go further if the predicate is not already in tmpBucket
                        if (!bucket.ContainsTokenWithSameIdentifier(predicate))
                        {

                            string[] tmpWords = bucket.questionLeft.Split(' ');

                            foreach (string word in tmpWords)
                            {
                                if ((util.match(word, util.URIToSimpleString(predicate.domains)) || util.match(word, util.URIToSimpleString(predicate.ranges))) && !predicate.QuestionMatch.Contains(word))
                                {
                                    if (predicate.QuestionMatch.Length > 0) predicate.QuestionMatch += " ";
                                    predicate.QuestionMatch += word;
                                }
                            }

                            QueryBucket newBucket = new QueryBucket(bucket);
                            somethingHappened = newBucket.add(predicate);
                            if (somethingHappened) queryBuckets.Add(newBucket);

                        }

                    }
                }
            }//end for


            //we have now query buckets that has the matched predicates and the part of the question left 
            //if we have a question: Who is the father of barack obama
            //after parsing the question, it would be: father of barack obama
            //after the .getPredicates(), it would be: Question left:barack obama, becasue fatehrof is a predicate
            //find combinations using literals , which could match any of the already made QueryBuckets 

            util.log(" search for combinations using literals among " + queryBuckets.Count + " Querybuckets ");

            foreach (QueryBucket bucket in queryBuckets.ToList())
            {
                bool somethingHappened = false;

                //Literals 
                if (bucket.questionLeft.Length > 0)
                {
                    literalList = lexicon.getLiterals(bucket.questionLeft); //find all matching literals

                    foreach (LexiconLiteral literal in literalList)
                    {

                        //if we only matched because of one single character ("a" for example) jump over this..
                        // useless in our example because we don't match 
                        if (literal.QuestionMatch.Length > 1)
                        { //jump over

                            //add TypeOfOwner to matchstring

                            string[] tmpWords = bucket.questionLeft.Split(' ');

                            //adding to the type of owner and the predicate used \
                            // not used in our case 
                            //foreach (string word in tmpWords)
                            //{
                            //    // some literals don't have type of owner .. it's set to ""  and all don't have a predicate . because we match " label " 
                            //    if ((util.match(util.URIToSimpleString(literal.typeOfOwner), word) || Util.match(tmpLit.getSimplePredicate(), word)) && !literal.QuestionMatch.Contains(word))
                            //    {
                            //        if (literal.QuestionMatch.Length > 0)
                            //            literal.QuestionMatch += " ";
                            //        literal.QuestionMatch +=  word;
                            //    }
                            //}


                            //add additional matchStrings 
                            // not used as well in our case 
                            //List<string> matchStrings = lexicon.getMatchStringsForLiteral(tmpLit);

                            //if (matchStrings != null)
                            //    foreach (string word in tmpWords)
                            //    {
                            //        foreach (string matchStringWord in matchStrings)
                            //        {
                            //            if (Util.match(matchStringWord, word) && !wordsUsed.Contains(word))
                            //            {
                            //                if (wordsUsed.Length > 0)
                            //                    wordsUsed = wordsUsed + " ";
                            //                wordsUsed = wordsUsed + word;
                            //            }
                            //        }
                            //    }

                            QueryBucket newBucket = new QueryBucket(bucket);
                            somethingHappened = newBucket.add(literal);
                            if (somethingHappened) queryBuckets.Add(newBucket);

                        }
                    }
                }
            }


            // till now query buckets generated containing predicates combined with other predicates  combined with literals 	
            //delete all occurences of ignoreStrings out of questionLeft string
            foreach (QueryBucket bucket in queryBuckets)
            {
                string questionLeft = bucket.questionLeft;
                List<string> ignoreStrings = util.getIgnoreStrings();
                foreach (string ignoreString in ignoreStrings)
                {
                    while (Regex.Match(questionLeft, ".*(\\s|^)" + ignoreString + "(\\s|$).*", RegexOptions.IgnoreCase).Length > 0)
                    {
                        questionLeft = Regex.Replace(questionLeft, "(\\s|^)" + ignoreString + "(\\s|$)", " ");
                        questionLeft = questionLeft.Replace("  ", " "); //delete double spaces
                        questionLeft = questionLeft.Trim();
                    }
                }

                #region check exceptions
                //    bucket.QuestionLeft = questionLeft.Trim();

                //    if ((bucket.QuestionLeft.Trim().Length > 0) || (bucket.countToDo()/*Replaced by getter count*/ > 0))
                //    {
                //        //ok now check for our exceptions
                //        //exception: words left and those are contained in the string of the unresolved URIs
                //        bool isException = false;
                //        if ((bucket.QuestionLeft.Trim().Length > 0) && (bucket.countToDo()/*Replaced by getter count*/ > 0))
                //        {
                //            string tmpString = Util.stem(bucket.QuestionLeft.Trim().ToLower());
                //            string[] tmpWords = tmpString.Split(' '); allContained = true;
                //            foreach (string word in tmpWords)
                //            {
                //                if (!Util.stem(bucket.getToDoAsSimpleString().replaceAll("([A-Z])", " $1").toLowerCase()).contains(word))        //////////////////////////to be handled later
                //                    allContained = false;
                //            }
                //            if (allContained)
                //                isException = true;
                //        }

                //        //exception: unresolved URIs and the strings of those uri are contained in the parsedQuestion string
                //        if ((bucket.QuestionLeft.Trim().Length == 0) && (bucket.countToDo()/*Replaced by getter count*/ > 0))
                //        {
                //            string tmpString = bucket.getToDoAsSimpleString().replaceAll("([A-Z])", " $1").toLowerCase();    //////////////////////////to be handled later
                //            string[] tmpWords = tmpString.Trim().Split(' ');
                //            allContained = true;
                //            foreach (string word in tmpWords)
                //            {
                //                if (!Util.stem(parsedQuestion).Contains(Util.stem(word)))
                //                    allContained = false;
                //            }
                //            if (allContained)
                //                isException = true;

                //        }

                //        if (!isException)
                //        {
                //            queryBuckets.Remove(bucket);
                //            /*
                //            queryBuckets.Remove(i);
                //            i--;
                //             * */
                //        }

                //    }
                #endregion
            }



            queryBuckets = cleanBucket(queryBuckets);


            // remove duplicates ie. if for any solution another one has the same content in the bucket remove that
            foreach (QueryBucket bucket1 in queryBuckets)
            {
                foreach (QueryBucket bucket2 in queryBuckets)
                {
                    if (!bucket1.Equals(bucket2))
                    {
                        // not to comparing to itself
                        if (bucket1.EqualSolution(bucket2))
                        {
                            queryBuckets.Remove(bucket1);
                            break;
                        }
                    }
                }
            }


            //and now build queries out of the buckets :-)
            foreach (QueryBucket bucket in queryBuckets)
            {
                if (bucket.questionLeft.Length > 4)
                    continue;
                util.log ("-----------------------");
                util.log("QUESTION LEFT: " + bucket.questionLeft);
                foreach (var item in bucket.tokens)
                {
                    //if (item.score ==0)
                    {
                        //util.log("\n\nSCORE:" + item.score.ToString());
                        //util.log("\n\nQUESTION MATCH: " + item.QuestionMatch + "\t" + "URI USED: " + item.URI+"\tSCORE:" + item.score.ToString() 
                           // + "\tLABEL: " + item.label);
                    }
                } 
                   string Query = bucket.GetQuery();
                   util.log(Query);
                   //sparqlQueries.addQuery(Query, bucket.getScore())
            }
            Console.WriteLine("DONE");
        }


        /// <summary>
        /// removing the non used predicates domains and the literals type of owners 
        /// </summary>
        /// <param name="tokens">list </param>of tokens
        /// <returns>cleaned list of tokens </returns>
        private List<QueryBucket> cleanBucket(List<QueryBucket> queryBuckets)
        {

            #region removing Buckets which still have question left  >1

            foreach (QueryBucket querybucket in queryBuckets.ToList())
            {
                if (querybucket.questionLeft.Length > 0)
                {
                    queryBuckets.Remove(querybucket);
                }
            }

            #endregion 



           #region remove Predicates domains and type of owners
            foreach (QueryBucket querybucket in queryBuckets)
            {
                List<LexiconLiteral> literalsList = new List<LexiconLiteral>();
                List<LexiconPredicate> predicateList = new List<LexiconPredicate>();

                List<LexiconToken> tokens = querybucket.tokens;
                foreach (LexiconToken token in tokens.ToList())
                {
                    if (token is LexiconLiteral)
                    {
                        literalsList.Add(token as LexiconLiteral);
                    }
                    else if (token is LexiconPredicate)
                    {
                        predicateList.Add(token as LexiconPredicate);
                    }
                }


                //removing predicates that doesn't match any of the literalstype of owners
                foreach (LexiconPredicate predicate in predicateList.ToList())
                {
                    foreach (string domain in predicate.domains.ToList())
                    {
                        bool exists = false;

                        foreach (LexiconLiteral literal in literalsList.ToList())
                        {
                            if (literal.typeOfOwner.Contains(domain))
                            {
                                exists = true;
                                break;
                            }
                        }

                        if (!exists)
                        {
                            predicate.domains.Remove(domain);

                            if (predicate.domains.Count == 0)
                            {
                                predicateList.Remove(predicate);
                            }
                        }
                    }
                }

                //removing types of owners that doesn't match any of the predicates domains
                foreach (LexiconLiteral literal in literalsList.ToList())
                {
                    foreach (string typeOfOwner in literal.typeOfOwner.ToList())
                    {
                        bool exists = false;

                        foreach (LexiconPredicate predicate in predicateList)
                        {
                            if (literal.typeOfOwner.Contains(typeOfOwner))
                            {
                                exists = true;
                                break;
                            }
                        }

                        if (!exists)
                        {
                            literal.typeOfOwner.Remove(typeOfOwner);
                            
                            if (literal.typeOfOwner.Count == 0)
                            {
                                literalsList.Remove(literal);
                            }
                        }
                    }
                }



                List<LexiconToken> newtokens = new List<LexiconToken>();
                newtokens.Concat<LexiconToken>(predicateList);
                newtokens.Concat<LexiconToken>(literalsList);


                querybucket.tokens = newtokens;

            }
           #endregion
            return queryBuckets; 
        }
    }
}