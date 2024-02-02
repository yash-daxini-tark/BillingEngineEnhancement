using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper.Configuration.Attributes;

namespace BillingEngineEnhancement
{
    internal class Region
    {
        [Name("Region")]
        public string region { get; set; }
        [Name("Free Tier Eligible")]
        public string freeTierEligible { get; set; }
        public override string ToString()
        {
            return region + " " + freeTierEligible;
        }
    }
}
