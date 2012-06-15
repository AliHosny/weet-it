using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace QuestionAnswering
{
    class QueryBucket
    {
        private string questionLeft;    // this one contains all words not yet consumed by the tokens already in the bucket
        private Dictionary<object/*LexiconToken consumedToken*/, object/*string of words used to match them*/> tokens;   // a dictionary containing all the consumed LexiconTokens and the words used to match them
        private List<string> uriToDo;   // List containing all URIs (Strings) that are not yet resolved
        private Dictionary<object,object> uriUsed; // hashtable containing all URIs (Strings) used to fill the bucket
        private int[] rating = new int[1]; //a rating for the answer being the one searched for, the rating is being updated when getQuery() is executed (the array is for making it possible to pass by reference)

        //Functions
        #region

        public QueryBucket(string question)
        {
            tokens = new Dictionary<object, object>();
            questionLeft = question;
            uriUsed = new Dictionary<object, object>();
            uriToDo = new List<string>();
            //rating[0] = 0;  //to be discussed
        }

        public QueryBucket(QueryBucket bucket)
        {
            this.tokens = new Dictionary<object,object>(bucket.tokens);
            this.questionLeft = bucket.questionLeft;
            this.uriUsed = new Dictionary<object,object>(bucket.tokens);
            this.uriToDo = new List<string>(bucket.uriToDo);
            //this.rating[0] = bucket.getRating();    //to be discussed
        }

        /*Checks if there is a token already exists in the consumed tokens list that connects the same URIs (has same range and domain)*/
        private bool ContainsTokenWithSameURIs(object/*LexiconToken*/ token)
        {
            foreach(List<object> value in tokens.Values)
            {
                object /*LexiconToken*/ tmpToken = /*(LexiconToken casting)*/value[0];
                if (tmpToken.hasURIs(token))/*LexiconToken method*/
                    return true;
            }

            return false;
        }

        /*Checks if there is a token already exists in the consumed tokens list that has same identifier*/
        public bool ContainsTokenWithSameIdentifier(object/*LexiconToken*/ token)
        {
            foreach (List<object> value in tokens.Values)
            {
                object /*LexiconToken*/ tmpToken = /*(LexiconToken casting)*/value[0];
                if (tmpToken.GetIdentifier() == token.GetIdentifier())/*LexiconToken method*/
                    return true;
            }

            return false;
        }

        public bool ContainsQueryPart(string queryPart)
        {
            foreach (List<object> value in tokens.Values)
            {
                object /*LexiconToken*/ tmpToken = /*(LexiconToken casting)*/value[0];
                string tmpWordsUsed = (string)value[1]; 
                if (tmpToken.BuildQueryPart(tmpWordsUsed).equals(queryPart))/*LexiconToken method*/
                    return true;
            }
            return false;
        }

        public bool Add(object/*LexiconToken*/ token,string wordsUsed)
        {
            List<object> uriList =token.GetURIs()/*LexiconToken method*/;       //this will get the URIs of the domain and range of the token (if available)
            string tmpURI;      //Temp variable

            /*If this token has domain and range(is a connector token or predicate) i.e. uriList size > 1, 
             *check all URIs of that token are already present in any token in the tokens hashtable
             *If already present, dont add (return false)*/
            if((uriList.Count > 1) && (this.ContainsSameURIs(token)))
                return false;

            /*
		 * check whether any URI of token is contained in the uriToDo vector 
		 * but only if there is already something in the uriToDo vector or in the uriUsed hashtable
		 */ 
            bool inToDo;
            bool inURIUsed;
            if ((uriToDo.Count > 0) || (uriUsed.Count > 0)) /////////////////To be checked again
            {
                inToDo = false;
                for (int i = 0; i < uriList.Count; i++)
                {
                    tmpURI = (string)uriList[i];
                    if (uriToDo.Contains(tmpURI))
                        inToDo = true;
                }
                inURIUsed = false;
                for (int i = 0; i < uriList.Count; i++)
                {
                    tmpURI = (string)uriList[i];
                    if (uriUsed.ContainsKey(tmpURI))    //Note: ContainsKey or ContainsValue
                        inURIUsed = true;
                }
                if (!inToDo && !inURIUsed) 
                    return false;
            }

            //consume the wordsUsed from the questionLeft String
            string[] tmpWords = wordsUsed.Split(' ');
            Regex regex;
            int firstIndex, lastIndex;
            foreach (string word in tmpWords)
            {                
                if (questionLeft.Contains(word))
                {
                    firstIndex = questionLeft.IndexOf(word);
                    lastIndex = questionLeft.LastIndexOf(word);

                    questionLeft = questionLeft.Remove(firstIndex, lastIndex-firstIndex);
                }
                else{
                    regex = new Regex("[a-z]*" + word + "[a-z]*");

                    questionLeft=regex.Replace(questionLeft, "");
                }

                questionLeft = questionLeft.Replace("  ", " ");
                questionLeft = questionLeft.Replace("__", "_");
                questionLeft = questionLeft.Trim();
            }

            questionLeft = questionLeft.Trim();
            List<object> tmpTokenInfo= new List<object>();
            tmpTokenInfo.Add(token);
		    tmpTokenInfo.Add(wordsUsed);
            tokens.Add(token.ToString()+token.getType()/*LexiconToken method*/,tmpTokenInfo);

            //if this token has an uri in it that is part of the uriToDo Vector,
            //remove it from the vector an move it into the uriUsed Hashtable,
            //elseif the uri is already contained in the uriUsed Hashtable do nothing
            //else put it into the uriToDo Vector

            for (int i = 0; i < uriList.Count; i++)
            {
                tmpURI = (string)uriList[i];
                if (uriToDo.Contains(tmpURI))
                {
                    uriToDo.Remove(tmpURI);
                    uriUsed.Add(tmpURI, tmpURI);
                }
                else if (!uriUsed.ContainsKey(tmpURI)) uriToDo.Add(tmpURI);
            }

            return true;
        }

        /// <summary>
        /// Builds the sparql query for this bucket
        /// </summary>
        /// <returns>string of this bucket's query</returns>
        public string GetQuery()
        {

            string literalQuery = "";
            string predicateQuery = "";
            string query;

            
           /*
            *!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! postponed till discussing rating part
            * 
            //reset rating
            if (tokens.Count == 0) 
                rating[0] = 0; //set low rating for empty bucket
            else 
                rating[0] = 10000; //most of the rating changes decrease the rating.
            */

            foreach (List<object> value in tokens.Values)
            {
                LexiconToken tmpToken = (LexiconToken)value[0];
                //string tmpWordsUsed = (string)value[1];

                if (tmpToken is LexiconLiteral)/*check if type LexiconLiteral*/
                {
				    if ((literalQuery.Length > 0) && (literalQuery != null)) 
                        literalQuery = literalQuery + " . ";
				    literalQuery = literalQuery + tmpToken.BuildQueryPart();/*LexconToken method*/		
			    } 
                else 
                {
                    if (predicateQuery.Length > 0)
                        predicateQuery = predicateQuery + " . ";
				    predicateQuery = predicateQuery + tmpToken.BuildQueryPart();	/*LexconToken method*/
			    }
                /*   
                 * Postponed till discussing rating
                 * 
                 * 
                double r = rating[0] * 0.9;
			    rating[0] = new Long(Math.round(r)).intValue();
                 * */
            }

            query = predicateQuery;

	        if ( (literalQuery.Length > 0) && (predicateQuery.Length > 0)) 
                query = query + " . ";
	        query = query + literalQuery;

            query = "select distinct *" + 
	    	" WHERE { " +
				query + 
				" }";
		    return query;
        }

        public bool EqualSolution(QueryBucket bucket)
        {
            if (this.tokens.Count != bucket.tokens.Count)
                return false;

            foreach (List<object> value in tokens.Values)
            {
                object /*LexiconToken*/ tmpToken = /*(LexiconToken casting)*/value[0];
                string tmpWordsUsed = (string)value[1];
                if (!bucket.ContainsQueryPart(tmpToken.BuildQueryPart(tmpWordsUsed)/*LexiconToken method*/))
                    return false;
            }

            return true;
        }


        #endregion

        //Setters and Getters
        #region


        public string QuestionLeft
        {
            get { return questionLeft; }
            set { questionLeft = value; }
        }

        public Dictionary<object, object> Tokens
        {
            get { return tokens; }
            set { tokens = value; }
        }
        #endregion
    }
}
