using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace VehicleRentalAPI.Models
{
    public class ReportsViewModel
    {
        // Total Statistics
        public int TotalRentals { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TotalVehicles { get; set; }
        public int TotalCustomers { get; set; }

        // Vehicle Usage Report
        public List<VehicleUsageReport> VehicleUsage { get; set; }

        // Revenue Chart Data
        public List<string> RevenueLabels { get; set; }
        public List<decimal> RevenueData { get; set; }
    }

    public class VehicleUsageReport
    {
        public string VehicleModel { get; set; }
        public int RentalsCount { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    public class CustomerListReport
    {
        public string CustomerName { get; set; }
        public int TotalRentals { get; set; }
        public decimal TotalSpending { get; set; }
    }

    public class RevenueReport
    {
        public decimal TotalRevenue { get; set; }
    }
}
