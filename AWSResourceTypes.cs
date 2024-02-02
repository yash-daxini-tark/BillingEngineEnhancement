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
        public string InstanceType { get; set; }
        [Name("Charge/Hour(OnDemand)")]
        //[System.ComponentModel.TypeConverter(typeof(DoubleConverter))]
        public string ChargeOnDemand { get; set; }
        [Name("Charge/Hour(Reserved)")]
        //[System.ComponentModel.TypeConverter(typeof(DoubleConverter))]
        public string ChargeReserved { get; set; }
        [Name("Region")]
        public string Region { get; set; }
        public AWSResourceTypes(string instanceType, string chargeOnDemand, string chargeReserved, string region)
        {
            InstanceType = instanceType;
            ChargeOnDemand = chargeOnDemand;
            ChargeReserved = chargeReserved;
            Region = region;
        }
        public AWSResourceTypes() { }
        override
        public string ToString()
        {
            return InstanceType + " " + ChargeOnDemand + " "+ ChargeReserved + " " +Region;
        }
    }
}
