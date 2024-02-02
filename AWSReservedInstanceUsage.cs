using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BillingEngine;
using CsvHelper.Configuration.Attributes;

namespace BillingEngineEnhancement
{
    internal class AWSReservedInstanceUsage : AWSResourceUsages
    {
        [Name("Start Date")]
        public DateTime StartDate { get; set; }
        [Name("End Date")]
        public DateTime EndDate { get; set; }

        [Ignore]
        public double totalCost { get; set; }

        public AWSReservedInstanceUsage() { }
        public AWSReservedInstanceUsage(string customerID, string eC2InstanceID, string eC2InstanceType, DateTime usedFrom, DateTime usedUntil) : base(customerID, eC2InstanceID, eC2InstanceType)
        {
            CustomerID = customerID;
            EC2InstanceID = eC2InstanceID;
            EC2InstanceType = eC2InstanceType;
            StartDate = usedFrom;
            EndDate = usedUntil;
            totalCost = 0;
        }
        public AWSReservedInstanceUsage(AWSReservedInstanceUsage aWSResourceUsage)
        {
            CustomerID = aWSResourceUsage.CustomerID;
            EC2InstanceID = aWSResourceUsage.EC2InstanceID;
            EC2InstanceType = aWSResourceUsage.EC2InstanceType;
            StartDate = aWSResourceUsage.StartDate;
            EndDate = aWSResourceUsage.EndDate;
            totalCost = 0;
        }
        //public List<AWSReservedInstanceUsage> splitDateBasedOnItsMonth()
        //{
        //List<AWSOnDemandResourceUsage> updatedResources = new List<AWSOnDemandResourceUsage>();
        //var totalDifferenceInSeconds = (UsedUntil - UsedFrom).TotalSeconds;
        //DateTime startDateOfService = UsedFrom;
        //while (totalDifferenceInSeconds > 0)
        //{
        //    DateTime endDateOfMonth = new DateTime(startDateOfService.Year, startDateOfService.Month, DateTime.DaysInMonth(startDateOfService.Year, startDateOfService.Month), 23, 59, 59);
        //    double minimumSeconds = Math.Min((endDateOfMonth - startDateOfService).TotalSeconds, totalDifferenceInSeconds);
        //    totalDifferenceInSeconds -= minimumSeconds + 1;
        //    updatedResources.Add(new AWSOnDemandResourceUsage(CustomerID, EC2InstanceID, EC2InstanceType, startDateOfService, startDateOfService.AddSeconds(minimumSeconds)));
        //    startDateOfService += new TimeSpan(0, 0, 0, (int)Math.Ceiling(minimumSeconds + 1));
        //}
        //return updatedResources;
        //}
        public double calculateCost(double charge)
        {
            totalCost = Math.Ceiling((EndDate - StartDate).TotalHours) * charge;
            return totalCost;
        }
        override
        public string ToString()
        {
            return CustomerID + " " + EC2InstanceID + " " + EC2InstanceType + " " + StartDate.ToString() + " " + EndDate + " " + totalCost;
        }
    }
}
