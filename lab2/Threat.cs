using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab2
{
    public class Threat
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Source { get; set; }
        public string ImpactObject { get; set; }
        public bool isBrokenConfidentiality { get; set; }
        public bool isBrokenIntegrity { get; set; }
        public bool isBrokenAvailobility { get; set; }

        

        public override string ToString()
        {
            return Id + " " + Name;
        }

        

        
    }
}
