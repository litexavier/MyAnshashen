using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AscensionModule
{
    class Picture
    {
        private string path;

        public string Path
        {
            get
            {
                return path;
            }
            set
            {
                path = value;
            }
        }

        public override string ToString()
        {
            return this.path;
        }
    }
}
