using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApplication11
{
    public class MiniProfile: MicroProfile
    {
        List<Entity> related = new List<Entity>();
        List<KeyValuePair<String, List<Entity>>> details = new List<KeyValuePair<String, List<Entity>>>();
        public List<KeyValuePair<String, List<Entity>>> Details
        {
            get { return details; }
            set { details = value; }
        }
    }
}