using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BillingEngine
{
    internal class BillResourceInformation
    {
        public BillResourceInformation(string region, string resourceType, int numberOfResources, string usedTime, string billedTime, string totalAmount, string discount, string actualAmount)
        {
            this.region = region;
            this.resourceType = resourceType;
            this.numberOfResources = numberOfResources;
            this.usedTime = usedTime;
            this.billedTime = billedTime;
            this.totalAmount = totalAmount;
            this.discount = discount;
            this.actualAmount = actualAmount;
        }
        public string region { get; set; }
        public string resourceType { get; set; }
        public int numberOfResources { get; set; }
        public string usedTime { get; set; }
        public string billedTime { get; set; }
        public string totalAmount { get; set; }
        public string discount { get; set; }
        public string actualAmount { get; set; }


        public override string ToString()
        {
            return string.Format("{0}, {1}, {2}, {3}, {4}, ${5}, ${6}, ${7}", region, resourceType, numberOfResources, usedTime, billedTime, totalAmount, discount, actualAmount);
        }

    }
}
