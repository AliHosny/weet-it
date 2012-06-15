using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/*This class should do the following:
         * 1-Receive the questionLeft from the other class
         * 2-generate all the possible combinations of the questionLeft with the upper and lower cases
         * 3-generate the necessary query for each case
         * 4-Do the query for each case
         * 5-add each query result to the list<LexiconLiterals> //doesn't matter which query returned which result
         * 6-Return the List<LexiconLiterals>
         */

//The structure should be like this:
//FindLiterals->getPermutations->generateQueriesFromPermutations->doQueriesWithLexiconLiterals


//public List<LexiconLiteral> FindLiterals(string questionLeft)
//public List<string> getPermutations(string questionLeft)
//public List<string> generateQueriesFromPermutations(list<string> permutationsString)
//public List<LexiconLiteral> doQueriesWithLexiconLiterals(list<string> queries)

/*TODO:
 *[DONE] TEST Permutations
 *[] Each word capitalizations and manipulations // no need for that 
 * trim the question 
 * removing extra spaces
 */

/*Recommendations:
 * 1-The permutations needs capitalization combinations, this could be too long... or REGEX or ???
 */
namespace LexiconLiteral
{
    class LexiconLiteral
    {
        #region Members
        protected String matchString; //the string which matches when searching for the predicate
        protected String sparqlString; //the actual sparql string to use when building the query
        protected int type;
        protected String identifier; //a string identifiying this token
        #endregion


        /// <summary>
        /// The only method to be used inside this class
        /// </summary>
        /// <param name="questionLeft">The input string of the questionLeft</param>
        /// <returns>The output list of literal objects</returns>
        public List<LexiconLiteral> FindLiterals(string questionLeft)
        {
            List<LexiconLiteral> toReturn = new List<LexiconLiteral>();

            toReturn = doQueriesWithLexiconLiterals(generateQueriesFromPermutations(getPermutations(questionLeft)));
            return toReturn;
        }


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
            input=questionLeft.Split(' ').ToList<string>();

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
            toReturn=toReturn.Distinct().ToList<string>();

            return toReturn;
 
        }

        /// <summary>
        /// this method generates the query of each string in the permutationsString List
        /// </summary>
        /// <param name="permutationsString">List of all permutations list</param>
        /// <returns>The list of Ready String List</returns>
        public List<string> generateQueriesFromPermutations(List<string> permutationsString)
        {
            List<string> toReturn = new List<string>();
            
            return toReturn;
        }


        /// <summary>
        /// This method is responsible of Actually querying the queries and forming the list of lexicon literals
        /// </summary>
        /// <param name="queries">Queries do be queried</param>
        /// <returns>The final list of Lexicon Literals</returns>
        public List<LexiconLiteral> doQueriesWithLexiconLiterals(List<string> queries)
        {
            List<LexiconLiteral> toReturn = new List<LexiconLiteral>();
            return toReturn;
        }

    }
}
