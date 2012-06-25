using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApplication11
{
    public class Entity
    {
        String uri, label, pic;
        public String URI
        {
            get { return uri; }
            set { uri = value; }
        }
        public String Label
        {
            get { return label; }
            set { label = value; }
        }
        public String Picture
        {
            get { return pic; }
            set { pic = value; }
        }
    }
}
