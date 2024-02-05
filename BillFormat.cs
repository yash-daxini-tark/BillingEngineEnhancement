using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BillingEngine
{
    internal class BillFormat
    {
        public BillFormat(string CustomerName, string TimeInfo, string TotalAmount, string TotalDiscount, string ActualAmount, string TableRow, List<BillResourceInformation> resourceUsage)
        {
            this.CustomerName = CustomerName;
            this.TimeInfo = TimeInfo;
            this.TotalAmount = TotalAmount;
            this.TableRow = TableRow;
            this.TotalDiscount = TotalDiscount;
            this.ActualAmount = ActualAmount;
            this.resourceUsage = new List<BillResourceInformation>(resourceUsage);
        }
        public string CustomerName { get; set; }
        public string TimeInfo { get; set; }
        public string TotalAmount { get; set; }
        public string TotalDiscount { get; set; }
        public string ActualAmount { get; set; }
        public string TableRow { get; set; }

        List<BillResourceInformation> resourceUsage { get; set; }

        public override string ToString()
        {
            StringBuilder bill = new StringBuilder();
            bill.AppendLine(CustomerName);
            bill.AppendLine(TimeInfo);
            bill.AppendLine(TotalAmount);
            bill.AppendLine(TotalDiscount);
            bill.AppendLine(ActualAmount);
            bill.AppendLine(TableRow);
            foreach (BillResourceInformation resource in resourceUsage)
            {
                bill.AppendLine(resource.ToString());
            }
            return bill.ToString();
        }
    }
}
