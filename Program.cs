using System.Collections;
using System.Globalization;
using System.Linq;
using System.Text;
using BillingEngineEnhancement;

namespace BillingEngine;
class Program
{
    #region Split Resource Usage Base on Month for on demand
    public static List<AWSResourceUsages> splitResourceUsageByMonth(List<AWSResourceUsages> resourceUsages, ref List<Customer> customers)
    {
        List<AWSResourceUsages> updatedResources = new List<AWSResourceUsages>();
        foreach (AWSResourceUsages resource in resourceUsages)
        {
            updatedResources = updatedResources.Concat(resource.splitDateBasedOnItsMonth()).ToList();
        }
        return updatedResources;
    }
    #endregion

    #region Split Resource Usage Base on Month on reserved
    public static List<AWSResourceUsages> splitResourceUsageByMonthForOnReserved(List<AWSResourceUsages> resourceUsages, ref List<Customer> customers)
    {
        List<AWSResourceUsages> updatedResources = new List<AWSResourceUsages>();
        foreach (AWSResourceUsages resource in resourceUsages)
        {
            resource.UsedUntil = resource.UsedUntil.Add(new TimeSpan(23, 59, 59));
            updatedResources = updatedResources.Concat(resource.splitDateBasedOnItsMonth()).ToList();
        }
        return updatedResources;
    }
    #endregion

    #region Calculate Cost For on demand
    public static void calculateCostForOnDemandResource(ref List<AWSResourceUsages> updatedResources, ref List<Customer> customers, Dictionary<string, string> changesForEachInstanceType, Dictionary<string, string> freeInstanceForEachRegion)
    {
        foreach (AWSResourceUsages resource in updatedResources)
        {
            Customer customer = customers.Where(customer => customer.CustomerID.Replace("-", "").Equals(resource.CustomerID)).FirstOrDefault();
            string cost = changesForEachInstanceType[resource.EC2InstanceType + "-" + resource.Region].Split("-")[0].Substring(1);
            var differenceOfDates = customer.freeInstanceDueDate - resource.UsedFrom;
            if (customer.freeInstanceDueDate < resource.UsedFrom)
            {
                resource.calculateCost(cost);
            }
            else if (!freeInstanceForEachRegion[resource.Region].Equals(resource.EC2InstanceType))
            {
                resource.calculateCost(cost);
            }
            else
            {
                TimeSpan difference = resource.UsedUntil - resource.UsedFrom;
                Console.WriteLine(difference);
                int totalHours = (int)Math.Ceiling((difference).TotalHours);
                int month = resource.UsedFrom.Month;
                if (resource.OS.Equals("Windows"))
                {
                    int min = Math.Min(totalHours, customer.freeInstanceForWindows[month - 1]);
                    customer.freeInstanceForWindows[month - 1] -= min;
                    int rem = totalHours - min;
                    resource.totalCost += rem * Convert.ToDouble(cost);
                    resource.totalUsedTime += difference;
                    resource.totalDiscount += min * Convert.ToDouble(cost);
                }
                else if (resource.OS.Equals("Linux"))
                {
                    int min = Math.Min(totalHours, customer.freeInstanceForLinux[month - 1]);
                    customer.freeInstanceForLinux[month - 1] -= min;
                    int rem = totalHours - min;
                    resource.totalCost += rem * Convert.ToDouble(cost);
                    resource.totalUsedTime += difference;
                    resource.totalDiscount += min * Convert.ToDouble(cost);
                }
            }
        }
    }
    #endregion

    #region calculate cost for reserved 
    public static void calculateCostForOnReservedInstance(ref List<AWSResourceUsages> AWSResourceUsagess, Dictionary<string, string> chargeOfEachInstance)
    {
        foreach (var resource in AWSResourceUsagess)
        {
            var cost = chargeOfEachInstance[resource.EC2InstanceType + "-" + resource.Region].Split("-")[1];
            resource.calculateCost(cost);
        }
    }
    #endregion

    #region Generate bill

    public static void generateBill(ref List<Customer> customers, ref List<AWSResourceUsages> onDemandAWSResourceUsages, ref List<AWSResourceUsages> reservedAWSResourceUsages, ref List<AWSResourceTypes> awsResourceTypes, ref List<Region> regions)
    {
        List<AWSResourceUsages> updatedAWSResourceUsagesD = splitResourceUsageByMonth(onDemandAWSResourceUsages, ref customers);

        List<AWSResourceUsages> updatedAWSResourceUsagesR = splitResourceUsageByMonthForOnReserved(reservedAWSResourceUsages, ref customers);

        Dictionary<string, string> customerNames = customers.ToDictionary(customer => customer.CustomerID.Replace("-", ""), customer => customer.CustomerName);
        Dictionary<string, string> freeInstanceForEachRegion = regions.ToDictionary(region => region.region, region => region.freeTierEligible);
        Dictionary<string, string> changesForEachInstanceType = new Dictionary<string, string>();

        foreach (var type in awsResourceTypes)
        {
            changesForEachInstanceType.Add(type.instanceType + "-" + type.region, type.chargeOnDemand + "-" + type.chargeReserved);
        }

        List<AWSResourceUsages> finalList = updatedAWSResourceUsagesD.Concat(updatedAWSResourceUsagesR).ToList();
        foreach (var item in finalList)
        {
            var customerObj = customers.Where(customer => customer.CustomerID.Replace("-", "").Equals(item.CustomerID)).Select(customer => customer).FirstOrDefault();
            customerObj.freeInstanceDueDate = finalList.Where(res => res.CustomerID.Equals(item.CustomerID)).Select(res => res.UsedFrom).Min(res => res).AddMonths(11);
        }

        calculateCostForOnDemandResource(ref updatedAWSResourceUsagesD, ref customers, changesForEachInstanceType, freeInstanceForEachRegion);

        calculateCostForOnReservedInstance(ref updatedAWSResourceUsagesR, changesForEachInstanceType);

        var groupedResources = finalList.GroupBy(resource => new { resource.CustomerID, resource.UsedFrom.Month, resource.UsedFrom.Year })
                                           .Select(resource => new { Key = resource.Key, list = resource.Select(resource => resource).ToList() });

        foreach (var resource in groupedResources)
        {
            var g = resource.list.GroupBy(resource => new { resource.EC2InstanceType, resource.Region });
            double totalAmount = Math.Round(resource.list.Sum(res => res.totalCost), 4);
            string monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(resource.Key.Month);
            string fileName = resource.Key.CustomerID.Substring(0, 4) + "-" + resource.Key.CustomerID.Substring(4) + "_" + monthName.Substring(0, 3).ToUpper() + "-" + resource.Key.Year;
            double totalDiscount = Math.Round(resource.list.Sum(res => res.totalDiscount), 4);
            List<BillResourceInformation> resourceInformation = new List<BillResourceInformation>();
            foreach (var item5 in g)
            {
                string instanceType = item5.Key.EC2InstanceType;
                string region = item5.Key.Region;
                int totalInstances = item5.Select(item => item.EC2InstanceID).Distinct().Count();
                double actualAmount = Math.Round(item5.Sum(item => item.totalCost), 4);
                double discount = Math.Round(item5.Sum(item => item.totalDiscount), 4);
                double finalAmount = actualAmount + discount;
                var sumOfDates = item5.Select(item => item.totalUsedTime).Aggregate((date1, date2) => date1.Add(date2));
                sumOfDates = sumOfDates.Add(new TimeSpan(0, 0, 0, totalInstances - 1));
                if (sumOfDates.Seconds == 59) sumOfDates = sumOfDates.Add(new TimeSpan(0, 0, 0, 1));
                resourceInformation.Add(new BillResourceInformation(region, instanceType, totalInstances, (int)(sumOfDates.TotalHours) + ":" + sumOfDates.Minutes + ":" + sumOfDates.Seconds
                                                , (int)Math.Ceiling(sumOfDates.TotalHours) + ":0:0"
                                                , finalAmount.ToString("F4"), discount.ToString("F4")
                                                , actualAmount.ToString("F4")));
            }
            BillFormat bill = new BillFormat(customerNames[resource.Key.CustomerID], "Bill for month of " + monthName + " " + resource.Key.Year
                                                , "Total Amount: $" + (totalAmount + totalDiscount).ToString("F4"), "Discount: $" + totalDiscount.ToString("F4"), "Actual Amount: $" + totalAmount.ToString("F4"), "Resource Type, Total Resouorces, Total Used Time (HH:mm:ss), Total Billed Time (HH:mm:ss), Total Amount, Discount, Actual Amount"
                                                , resourceInformation);
            writeFile(fileName, bill.ToString());
        }
    }
    #endregion

    #region Take Input

    public static void takeInput(ref List<Customer> customers, ref List<AWSResourceUsages> onDemandAWSResourceUsages, ref List<AWSResourceUsages> reservedAWSResourceUsages, ref List<AWSResourceTypes> awsResourceTypes, ref List<Region> region
        , string pathOfCustomer, string pathOfOnDemadAWSResourceUsages, string pathOfReservedAWSResourceUsages, string pathOfAWSResourceTypes, string pathOfRegion)
    {
        customers = ReadCSV<Customer>.LoadDataFromCsv(pathOfCustomer);
        awsResourceTypes = ReadCSV<AWSResourceTypes>.LoadDataFromCsv(pathOfAWSResourceTypes);
        onDemandAWSResourceUsages = ReadCSV<AWSResourceUsages>.LoadDataFromCsv(pathOfOnDemadAWSResourceUsages);
        reservedAWSResourceUsages = ReadCSV<AWSResourceUsages>.LoadDataFromCsv(pathOfReservedAWSResourceUsages);
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

        string pathOfOnDemandAWSResourceUsages = "../../../TestCases/Input/AWSOnDemandResourceUsage.csv";
        string pathOfReservedAWSResourceUsages = "../../../TestCases/Input/AWSReservedInstanceUsage.csv";
        string pathOfAWSResourceTypes = "../../../TestCases/Input/AWSResourceTypes.csv";
        string pathOfCustomer = "../../../TestCases/Input/Customer.csv";
        string pathOfRegion = "../../../TestCases/Input/Region.csv";

        List<Customer> customers = new List<Customer>();
        List<AWSResourceUsages> onDemandAWSResourceUsages = new List<AWSResourceUsages>();
        List<AWSResourceUsages> reserAWSResourceUsages = new List<AWSResourceUsages>();
        List<AWSResourceTypes> awsResourceTypes = new List<AWSResourceTypes>();
        List<Region> regions = new List<Region>();

        takeInput(ref customers, ref onDemandAWSResourceUsages, ref reserAWSResourceUsages, ref awsResourceTypes, ref regions, pathOfCustomer, pathOfOnDemandAWSResourceUsages, pathOfReservedAWSResourceUsages, pathOfAWSResourceTypes, pathOfRegion);

        foreach (var customer in customers)
        {
            Array.Fill(customer.freeInstanceForLinux, 750);
            Array.Fill(customer.freeInstanceForWindows, 750);
        }

        generateBill(ref customers, ref onDemandAWSResourceUsages, ref reserAWSResourceUsages, ref awsResourceTypes, ref regions);

        //customers.ForEach(Console.WriteLine);
        //AWSResourceUsages.ForEach(Console.WriteLine);
        //AWSResourceUsages.ForEach(Console.WriteLine);
        //awsResourceTypes.ForEach(Console.WriteLine);
        //regions.ForEach(Console.WriteLine);


        #endregion
    }
}