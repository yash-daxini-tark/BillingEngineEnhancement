using System.Globalization;
using System.Linq;
using System.Text;
using BillingEngineEnhancement;

namespace BillingEngine;
class Program
{
    #region Split Resource Usage Base on Month
    public static List<AWSOnDemandResourceUsage> splitResourceUsageByMonth(List<AWSOnDemandResourceUsage> resourceUsages)
    {
        List<AWSOnDemandResourceUsage> updatedResources = new List<AWSOnDemandResourceUsage>();
        foreach (AWSOnDemandResourceUsage resource in resourceUsages)
        {
            //updatedResources = updatedResources.Concat(resource.splitDateBasedOnItsMonth()).ToList();
        }
        return updatedResources;
    }
    #endregion

    #region Calculate Cost
    public static void calculateCostForEachResource(ref List<AWSOnDemandResourceUsage> updatedResources, Dictionary<string, string> changesForEachInstanceType)
    {
        foreach (AWSOnDemandResourceUsage resource in updatedResources)
        {
            double cost = Convert.ToDouble(changesForEachInstanceType[resource.EC2InstanceType].Substring(1));
            resource.calculateCost(cost);
        }
    }
    #endregion

    //#region Generate bill

    //public static void generateBill(List<AWSResourceTypes> awsResourceTypes, List<AWSOnDemandResourceUsage> awsResourceUsage, List<Customer> customers)
    //{
    //    List<AWSOnDemandResourceUsage> updatedResources = splitResourceUsageByMonth(awsResourceUsage);

    //    Dictionary<string, string> changesForEachInstanceType = awsResourceTypes.ToDictionary(type => type.InstanceType, type => type.Charge);
    //    Dictionary<string, string> customerNames = customers.ToDictionary(customer => customer.CustomerID.Replace("-", ""), customer => customer.CustomerName);

    //    calculateCostForEachResource(ref updatedResources, changesForEachInstanceType);

    //    var groupedResources = updatedResources.GroupBy(resource => new { resource.CustomerID, resource.UsedFrom.Month, resource.UsedFrom.Year })
    //                                       .Select(resource => new { Key = resource.Key, list = resource.Select(resource => resource).ToList() });

    //    foreach (var resource in groupedResources)
    //    {
    //        var g = resource.list.GroupBy(resource => new { resource.EC2InstanceType });
    //        double totalAmount = Math.Round(resource.list.Sum(res => res.totalCost), 4);
    //        string monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(resource.Key.Month);
    //        string fileName = resource.Key.CustomerID.Substring(0, 4) + "-" + resource.Key.CustomerID.Substring(4) + "_" + monthName.Substring(0, 3).ToUpper() + "-" + resource.Key.Year;
    //        if (totalAmount == 0) continue;
    //        List<BillResourceInformation> resourceInformation = new List<BillResourceInformation>();
    //        foreach (var item5 in g)
    //        {
    //            string instanceType = item5.Key.EC2InstanceType;
    //            int totalInstances = item5.Select(item => item.EC2InstanceID).Distinct().Count();
    //            double totalCost = Math.Round(item5.Sum(item => item.totalCost), 4);
    //            var sumOfDates = item5.Select(item => item.UsedUntil - item.UsedFrom).Aggregate((date1, date2) => date1.Add(date2));
    //            if (sumOfDates.Seconds == 59) sumOfDates = sumOfDates.Add(new TimeSpan(0, 0, 0, 1));
    //            resourceInformation.Add(new BillResourceInformation(instanceType, totalInstances, (int)Math.Floor(sumOfDates.TotalHours) + ":" + sumOfDates.Minutes + ":" + sumOfDates.Seconds
    //                                            , (int)Math.Ceiling(sumOfDates.TotalHours) + ":00:00"
    //                                            , changesForEachInstanceType[instanceType]
    //                                            , totalCost));
    //        }
    //        BillFormat bill = new BillFormat(customerNames[resource.Key.CustomerID], "Bill for month of " + monthName + " " + resource.Key.Year
    //                                            , "Total Amount: " + totalAmount, "Resource Type,Total Resources,Total Used Time (HH:mm:ss),Total Billed Time (HH:mm:ss),Rate (per hour),Total Amount"
    //                                            , resourceInformation);
    //        writeFile(fileName, bill.ToString());
    //    }
    //}
    //#endregion

    #region Take Input

    public static void takeInput(ref List<Customer> customers, ref List<AWSOnDemandResourceUsage> awsOnDemandResourceUsage, ref List<AWSReservedInstanceUsage> awsReservedInstanceUsage, ref List<AWSResourceTypes> awsResourceTypes, ref List<Region> region
        , string pathOfCustomer, string pathOfAWSOnDemandResourceUsage, string pathOfAWSReservedInstanceUsage, string pathOfAWSResourceTypes, string pathOfRegion)
    {
        customers = ReadCSV<Customer>.LoadDataFromCsv(pathOfCustomer);
        awsResourceTypes = ReadCSV<AWSResourceTypes>.LoadDataFromCsv(pathOfAWSResourceTypes);
        awsOnDemandResourceUsage = ReadCSV<AWSOnDemandResourceUsage>.LoadDataFromCsv(pathOfAWSOnDemandResourceUsage);
        awsReservedInstanceUsage = ReadCSV<AWSReservedInstanceUsage>.LoadDataFromCsv(pathOfAWSReservedInstanceUsage);
        region = ReadCSV<Region>.LoadDataFromCsv(pathOfRegion);
    }

    #endregion

    #region Write Into File

    public static void writeFile(string fileName, string content)
    {
        string directory = "../../../Output/" + "Testcase";

        if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

        File.WriteAllText(directory + "/" + fileName + ".csv", content);
    }
    #endregion

    public static void Main(string[] args)
    {
        #region Get Input

        string pathOfAWSOnDemandResourceUsage = "../../../TestCases/Input/AWSOnDemandResourceUsage.csv";
        string pathOfAWSReservedInstanceUsage = "../../../TestCases/Input/AWSReservedInstanceUsage.csv";
        string pathOfAWSResourceTypes = "../../../TestCases/Input/AWSResourceTypes.csv";
        string pathOfCustomer = "../../../TestCases/Input/Customer.csv";
        string pathOfRegion = "../../../TestCases/Input/Region.csv";

        List<Customer> customers = new List<Customer>();
        List<AWSOnDemandResourceUsage> awsOnDemandResourceUsage = new List<AWSOnDemandResourceUsage>();
        List<AWSReservedInstanceUsage> awsReservedInstanceUsage = new List<AWSReservedInstanceUsage>();
        List<AWSResourceTypes> awsResourceTypes = new List<AWSResourceTypes>();
        List<Region> regions = new List<Region>();

        takeInput(ref customers, ref awsOnDemandResourceUsage, ref awsReservedInstanceUsage, ref awsResourceTypes, ref regions, pathOfCustomer, pathOfAWSOnDemandResourceUsage, pathOfAWSReservedInstanceUsage, pathOfAWSResourceTypes, pathOfRegion);
        //generateBill(awsResourceTypes, awsResourceUsage, customers, 1);

        customers.ForEach(Console.WriteLine);
        awsOnDemandResourceUsage.ForEach(Console.WriteLine);
        awsReservedInstanceUsage.ForEach(Console.WriteLine);
        awsResourceTypes.ForEach(Console.WriteLine);
        regions.ForEach(Console.WriteLine);


        #endregion
    }
}