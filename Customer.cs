using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper.Configuration.Attributes;

namespace BillingEngine
{
    internal class Customer
    {
        [Name("Customer ID")]
        public string CustomerID { get; set; }
        [Name("Customer Name")]
        public string CustomerName { get; set; }
        [Ignore]
        public int[] freeInstanceForWindows = new int[12];
        [Ignore]
        public int[] freeInstanceForLinux = new int[12];
        [Ignore]
        public DateTime freeInstanceDueDate { get; set; }
        [Ignore]
        public bool isDueDateSet { get; set; }

        public Customer(string customerID, string customerName)
        {
            CustomerID = customerID;
            CustomerName = customerName;
            //freeInstanceForLinux = new int[12];
            //freeInstanceForWindows = new int[12];
            Array.Fill(freeInstanceForLinux, 750);
            Array.Fill(freeInstanceForWindows, 750);
            isDueDateSet = false;
        }
        public Customer() { }
        override
        public string ToString()
        {
            return CustomerID + " " + CustomerName;
        }
    }
}

