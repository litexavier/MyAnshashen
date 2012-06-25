using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;

namespace Ascension.Controller
{
    using Ascension.Module;
    class ModuleController
    {
        public void Initialize()
        {
            using (XmlReader reader = XmlReader.Create(new FileStream(Definitions.AreaDefinitionXML, FileMode.Open)))
            {

            }
        }
    }
}