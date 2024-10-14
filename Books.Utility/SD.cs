using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Books.Utility
{
    public static class SD
    {
        public const string Role_Customer = "Customer";
        public const string Role_Company = "Company";
        public const string Role_Admin = "Admin";
        public const string Role_Employee = "Employee";

        public const string OrderStatus_Pending = "Pending";
        public const string OrderStatus_Approved = "Approved";
        public const string OrderStatus_InProcess = "Processing";
        public const string OrderStatus_Shipped = "Shipped";
        public const string OrderStatus_Cancelled = "Cancelled";
        public const string OrderStatus_Refunded = "Refunded";

        public const string PaymentStatus_Pending = "Pending";
        public const string PaymentStatus_Approved = "Approved";
        public const string PaymentStatus_DelayedPayment = "ApprovedForDelayPayment";
        public const string PaymentStatus_Rejected = "Rejected";
    }
}
