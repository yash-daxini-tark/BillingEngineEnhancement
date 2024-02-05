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
        [Index(4)]
        public DateTime UsedFrom { get; set; }
        [Index(5)]
        public DateTime UsedUntil { get; set; }
        [Name("Region")]
        public string Region { get; set; }
        [Name("OS")]
        public string OS { get; set; }
        [Ignore]
        public double totalCost { get; set; }
        [Ignore]
        public double totalDiscount { get; set; }
        [Ignore]
        public TimeSpan totalUsedTime { get; set; }
        public AWSResourceUsages() { }

        public AWSResourceUsages(string customerID, string eC2InstanceID, string eC2InstanceType, DateTime usedFrom, DateTime usedUntil, string region, string oS)
        {
            CustomerID = customerID;
            EC2InstanceID = eC2InstanceID;
            EC2InstanceType = eC2InstanceType;
            UsedFrom = usedFrom;
            UsedUntil = usedUntil;
            Region = region;
            totalCost = 0;
            totalDiscount = 0;
            OS = oS;
        }
        public List<AWSResourceUsages> splitDateBasedOnItsMonth()
        {
            List<AWSResourceUsages> updatedResources = new List<AWSResourceUsages>();
            var totalDifferenceInSeconds = (UsedUntil - UsedFrom).TotalSeconds;
            DateTime startDateOfService = UsedFrom;
            while (totalDifferenceInSeconds > 0)
            {
                DateTime endDateOfMonth = new DateTime(startDateOfService.Year, startDateOfService.Month, DateTime.DaysInMonth(startDateOfService.Year, startDateOfService.Month), 23, 59, 59);
                double minimumSeconds = Math.Min((endDateOfMonth - startDateOfService).TotalSeconds, totalDifferenceInSeconds);
                totalDifferenceInSeconds -= minimumSeconds + 1;
                updatedResources.Add(new AWSResourceUsages(CustomerID, EC2InstanceID, EC2InstanceType, startDateOfService, startDateOfService.AddSeconds(minimumSeconds), Region, OS));
                startDateOfService = startDateOfService.AddMonths(1);
            }
            return updatedResources;
        }
        public double calculateCost(string charge)
        {
            TimeSpan difference = UsedUntil - UsedFrom;
            totalCost += Math.Ceiling((UsedUntil - UsedFrom).TotalHours) * Convert.ToDouble(charge);
            totalUsedTime += difference;
            if (UsedFrom.Month == 6)
            {   
                Console.WriteLine(UsedFrom + " " + UsedUntil + " " + totalUsedTime + " " + CustomerID + " " + Region);
            }
            return totalCost;
        }
        public override string ToString()
        {
            return CustomerID + "," + EC2InstanceID + "," + EC2InstanceType + "," + UsedFrom + "," + UsedUntil + "," + Region + "," + OS + "," + totalCost;
        }
    }
}

