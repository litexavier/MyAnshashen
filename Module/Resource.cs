using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ascension.Module
{
    class Resource
    {
        private ResourceType type;
        private uint num;

        public Resource(ResourceType type)
        {
            this.type = type;
            this.num = 0;
        }

        public Resource(ResourceType type, uint num )
        {
            this.type = type;
            this.num = num;
        }

        public ResourceType Type
        {
            get
            {
                return type;
            }
        }

        public uint Num
        {
            get
            {
                return num;
            }
            set
            {
                if ( value < 0 ) {
                    num = 0;
                }
                else if( value > 1000 ) {
                    num = 1000;
                }
                else
                {
                    num = value;
                }
            }
        }

        public override string ToString ()
        {
            return "<" + this.type + "," + this.num + ">";
        }
    }
}
