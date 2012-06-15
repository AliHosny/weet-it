using System;
using System.Collections.Generic;
using System.Text;
using VDS.RDF;
using VDS.RDF.Query;
using System.IO;

namespace NLI
{
    class questionAnswer
    {

        public int answerCount {get; set;}

        public List<LexiconLiteral> subjectList;          // literals used to get this Answer Literals in the Sparql Query 
        public List<LexiconPredicate> predicateList;
        public List<INode> objectNodetList;           

        public List<string> subjectStringList;          // literals used to get this Answer Literals in the Sparql Query 
        public List<string> predicateStringList;      // predicates used to get this Answer Predicates used in the sparql Query 
        public List<string> objectStringList;           // literals that is contained in the answer of the predicate 
        
        public SparqlResultSet resultSet  {get; set ;}    //the base result set if u want to use it note:u'll have to implement the DotNetRDF libraries
        public util.questionTypes questiontype ;
        
        public questionAnswer()
        {
            subjectList  = new List<LexiconLiteral>();
            predicateList = new List<LexiconPredicate>();
            objectNodetList = new List<INode>();

            subjectStringList = new List<string>();
            predicateStringList = new List<string>();
            objectStringList = new List<string>();
        }

        public questionAnswer(util.questionTypes type)
        {
            subjectList = new List<LexiconLiteral>();
            predicateList = new List<LexiconPredicate>();
            objectNodetList = new List<INode>();

            subjectStringList = new List<string>();
            predicateStringList = new List<string>();
            objectStringList = new List<string>();

            questiontype = type;
        }

        

    }
}
