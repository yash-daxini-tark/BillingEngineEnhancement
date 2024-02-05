using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper.Configuration.Attributes;

namespace BillingEngine
{
    internal class AWSResourceTypes
    {
        [Name("Instance Type")]
        public string instanceType { get; set; }
        [Name("Charge/Hour(OnDemand)")]
        //[System.ComponentModel.TypeConverter(typeof(DoubleConverter))]
        public string chargeOnDemand { get; set; }
        [Name("Charge/Hour(Reserved)")]
        //[System.ComponentModel.TypeConverter(typeof(DoubleConverter))]
        public string chargeReserved { get; set; }
        [Name("Region")]
        public string region { get; set; }
        public AWSResourceTypes(string instanceType, string chargeOnDemand, string chargeReserved, string region)
        {
            this.instanceType = instanceType;
            this.chargeOnDemand = chargeOnDemand;
            this.chargeReserved = chargeReserved;
            this.region = region;
        }
        public AWSResourceTypes() { }
        override
        public string ToString()
        {
            return instanceType + " " + chargeOnDemand + " " + chargeReserved + " " + region;
        }
    }
}
