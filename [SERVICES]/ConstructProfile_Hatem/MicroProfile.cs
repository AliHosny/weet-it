using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApplication11
{
    public class MicroProfile :Profile
    {
        Entity MyEntity = new Entity();
        public String URI
        {
            get { return MyEntity.URI; }
            set { MyEntity.URI = value; }
        }
        public String Label
        {
            get { return MyEntity.Label; }
            set { MyEntity.Label = value; }
        }
        public String Picture
        {
            get { return MyEntity.Picture; }
            set { MyEntity.Picture = value; }
        }
        String ABSTRACT;
        public String Abstract
        {
            get { return ABSTRACT; }
            set { ABSTRACT = value; }
        }
    }
}
