using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VDS.RDF.Query;
using VDS.RDF;
using System.Xml;
using System.IO;
using System.Xml.Linq;

namespace ConsoleApplication11
{
    public class ProfileConstructor
    {
        public enum choiceProfile {micro, mini, full };

        public static Profile ConstructProfile(String subjectURI,choiceProfile profile, int resultLimit=10)
        {
            if (profile == choiceProfile.micro)
            {
                MicroProfile micro = new MicroProfile();
                micro.Abstract = getAbstract(subjectURI);
                micro.Label = getLabel(subjectURI);
                micro.Picture = imageGrapper.get_fb_link(subjectURI, imageGrapper.E.small);
                micro.URI = subjectURI;
                return micro;
            }
            else if (profile == choiceProfile.mini)
            {
                MiniProfile mini = new MiniProfile();
                mini.Abstract = getAbstract(subjectURI);
                mini.Label = getLabel(subjectURI);
                mini.URI = subjectURI;
                mini.Details = setProfileContents("mini", subjectURI, resultLimit);
                mini.Picture = imageGrapper.get_fb_link(subjectURI, imageGrapper.E.small);
                return mini;
            }
            else if (profile == choiceProfile.full)
            {
                FullProfile full = new FullProfile();
                full.Abstract = getAbstract(subjectURI);
                full.Label = getLabel(subjectURI);
                full.URI = subjectURI;
                List<String> relations = RelationGenerator.getRelatedEntities(subjectURI);
                List<Entity> related = new List<Entity>();
                foreach (String rel in relations)
                {
                    Entity en = new Entity();
                    en.URI = rel;
                    en.Label = getLabel(rel);
                    en.Picture = imageGrapper.get_fb_link(rel,imageGrapper.E.small);
                    related.Add(en);
                }
                full.Related = related;
                full.Details = setProfileContents("full", subjectURI, resultLimit);
                full.Picture = imageGrapper.get_fb_link(subjectURI, imageGrapper.E.large);
                return full;
            }
            return null;
        }

        private static List<KeyValuePair<String, List<Entity>>> setProfileContents(String profileType, String subjectURI, int resultLimit)
        {
            XDocument XMLDoc = XDocument.Load("profile content.xml");
            SparqlRemoteEndpoint endpoint = new SparqlRemoteEndpoint(new Uri("http://weetit:8890/sparql"));
            String query = "select * where {<" + subjectURI + "> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> ?obj}";
            List<String> types = new List<String>();
            SparqlResultSet results = endpoint.QueryWithResultSet(query);
            List<KeyValuePair<String,List<Entity>>> profileContent= new List<KeyValuePair<String,List<Entity>>>();
            bool typeFound = false;
            foreach (SparqlResult result in results)
                types.Add(result.Value("obj").ToString());
            foreach (String type in types)
            {
                List<XElement> XMLList = XMLDoc.Root.Elements(profileType).Elements().ToList();
                foreach (XElement element in XMLList)
                {
                    if (element.Attribute("URI").Value == type)
                    {
                        var elements = element.Elements("predicate");
                        foreach (XElement elem in elements)
                        {
                            List<Entity> entities = new List<Entity>();
                            if (elem.Attribute("queryType").Value == "getObjects")
                                entities = getQueryResults("getObjects",subjectURI, (elem.Value), resultLimit);
                            else if (elem.Attribute("queryType").Value == "getSubjects")
                            {
                                entities = getQueryResults("getSubjects",subjectURI, (elem.Value), resultLimit);
                            }
                            KeyValuePair<String, List<Entity>> key = new KeyValuePair<String, List<Entity>>(elem.Value,entities);
                            profileContent.Add(key);
                        }
                        typeFound = true;
                        break;
                    }
                }
                if (typeFound == true)
                    break;
            }
            if (typeFound == false)
            {
                List<XElement> XMLListExecluded = XMLDoc.Root.Elements("execludedPredicates").Elements("predicate").ToList();
                String execluded = "";
                String x = profileType == "mini" ? "5" : "10";
                String queryExecluded = "select distinct ?predicate   where{" +
                "<" + subjectURI + ">" + "?predicate ?literal";
                foreach (XElement elementExecluded in XMLListExecluded)
                    execluded += "!(?predicate=<" + elementExecluded.Value + "> ) &&";
                if (execluded != "")
                {
                    execluded = execluded.Substring(0, execluded.Length - 2);
                    queryExecluded += ".FILTER (" + execluded + ")";
                }
                queryExecluded+="} limit  " + x;
                SparqlResultSet resultsExecluded = endpoint.QueryWithResultSet(queryExecluded);
                List<Entity> entitiesExecluded = new List<Entity>();
                List<String> predicateNames = new List<String>();
                foreach (SparqlResult result in resultsExecluded)
                    predicateNames.Add(result.Value("predicate").ToString());
                foreach (String predicateName in predicateNames)
                {
                    entitiesExecluded = getQueryResults("getObjects",subjectURI, predicateName, resultLimit);
                    KeyValuePair<String, List<Entity>> key = new KeyValuePair<String, List<Entity>>(predicateName, entitiesExecluded);
                    profileContent.Add(key);
                }
            }
            return profileContent;
        }

        private static String getLabel(String URI)
        {
            //at least best one for now
            URI = Uri.EscapeUriString(URI);
            SparqlRemoteEndpoint endpoint = new SparqlRemoteEndpoint(new Uri("http://weetit:8890/sparql"));
            string query = "select * where {<" + URI + "> <http://www.w3.org/2000/01/rdf-schema#label> ?obj}";
            SparqlResultSet results = endpoint.QueryWithResultSet(query);
            //if there's no results from the first query, we will try to get the name 
            if (results.Count < 1)
            {
                string name_query = "select * where {<" + URI + "> <http://xmlns.com/foaf/0.1/name> ?obj}";
                results = endpoint.QueryWithResultSet(name_query);

                //if there's no result from the second query
                //get the name after the /
                if (results.Count < 1)
                {
                    string toreturn = new string(URI.ToCharArray().Reverse().ToArray());//URI.Reverse().ToString();
                    toreturn = toreturn.Remove(toreturn.IndexOf("/"));
                    toreturn = new string(toreturn.ToCharArray().Reverse().ToArray());
                    toreturn = toreturn.Replace("_", " ");
                    //TODO : get back the encoding
                    toreturn = toreturn.Trim();
                    return toreturn;
                }
                else
                {
                    //returning
                    return ((LiteralNode)results[0].Value("obj")).Value;
                }
            }
            else
            {
                //returning it
                return ((LiteralNode)results[0].Value("obj")).Value;
            }
        }

        private static List<Entity> getQueryResults(String type,String SubjectURI, String predicateURI, int resultLimit)
        {
            SparqlRemoteEndpoint endpoint = new SparqlRemoteEndpoint(new Uri("http://weetit:8890/sparql"));
            String query = "";
            if(type=="getObjects")
                query+="select * where {<" + SubjectURI + "><" + predicateURI + "> ?obj} limit " + resultLimit;
            else if(type=="getSubjects")
                query+= "select * where{ ?obj <" + predicateURI + "> <"+SubjectURI+ ">} limit "+resultLimit;
            SparqlResultSet results = endpoint.QueryWithResultSet(query);
            List<Entity> entities = new List<Entity>();
            foreach (SparqlResult result in results)
            {
                Entity en = new Entity();
                if (((INode)result[0]).NodeType == NodeType.Uri)
                {
                    en.Label = getLabel(result.Value("obj").ToString());
                    en.URI = result.Value("obj").ToString();
                }
                else
                {
                    en.Label = ((LiteralNode)result.Value("obj")).Value;
                }
                entities.Add(en);
            }
            return entities;

        }

        private static String getAbstract(String SubjectURI)
        {
            SparqlRemoteEndpoint endpoint = new SparqlRemoteEndpoint(new Uri("http://weetit:8890/sparql"));
            String query = "select * where {<" + SubjectURI + "><http://dbpedia.org/ontology/abstract> ?obj}";
            SparqlResultSet results = endpoint.QueryWithResultSet(query);
            if (results != null)
            {
                SparqlResult result = results[0];
                return ((LiteralNode)result.Value("obj")).Value;
            }
            else return null;
        }
    }
}