using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApplication11
{
    public class FullProfile : MiniProfile
    {
        List<Entity> related = new List<Entity>();
        public List<Entity> Related
        {
            get { return related; }
            set { related = value; }
        }
    }
}
