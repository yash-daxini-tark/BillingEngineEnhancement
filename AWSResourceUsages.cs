using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper.Configuration.Attributes;

namespace BillingEngineEnhancement
{
    internal class AWSResourceUsages
    {
        [Name("Customer ID")]
        public string CustomerID { get; set; }
        [Name("EC2 Instance ID")]
        public string EC2InstanceID { get; set; }
        [Name("EC2 Instance Type")]
        public string EC2InstanceType { get; set; }
        [Name("Region")]
        public string Region { get; set; }
        [Name("OS")]
        public string OS { get; set; }

        public AWSResourceUsages() { }

        public AWSResourceUsages(string customerID, string eC2InstanceID, string eC2InstanceType, string region, string oS)
        {
            CustomerID = customerID;
            EC2InstanceID = eC2InstanceID;
            EC2InstanceType = eC2InstanceType;
            Region = region;
            OS = oS;
        }
    }
}
