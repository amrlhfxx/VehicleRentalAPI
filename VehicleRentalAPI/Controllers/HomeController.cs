using Microsoft.AspNetCore.Mvc;
using VehicleRentalAPI.Data;
using VehicleRentalAPI.Models;
using System.Linq;

namespace VehicleRentalAPI.Controllers
{
    public class HomeController : Controller
    {
        private readonly VehicleRentalContext _context;

        public HomeController(VehicleRentalContext context)
        {
            _context = context;
        }

        // GET: Home/Index
        public IActionResult Index()
        {
            return View();  // Simply returns the Home/Index view
        }



        // GET: Home/Reports
        public IActionResult Reports()
        {
            // Fetch total statistics
            var totalVehicles = _context.Vehicles.Count();
            var totalCustomers = _context.Customers.Count();
            var totalRentals = _context.Rentals.Count();
            var totalRevenue = _context.Rentals.Sum(r => r.TotalAmount);

            // Fetch the Vehicle Usage Report data dynamically
            var vehicleUsage = _context.Vehicles
                .Select(v => new
                {
                    VehicleModel = v.Model,
                    RentalsCount = _context.Rentals.Count(r => r.VehicleID == v.VehicleID), // Calculate RentalsCount dynamically
                    TotalRevenue = _context.Rentals.Where(r => r.VehicleID == v.VehicleID).Sum(r => r.TotalAmount)
                })
                .ToList()
                .Select(v => new VehicleUsageReport
                {
                    VehicleModel = v.VehicleModel,
                    RentalsCount = v.RentalsCount,  // Use dynamically calculated RentalsCount
                    TotalRevenue = v.TotalRevenue
                })
                .ToList();

            // **Fetch real revenue data grouped by month** for the graph
            var revenueData = _context.Rentals
                .GroupBy(r => new { r.RentalDate.Year, r.RentalDate.Month })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    TotalRevenue = g.Sum(r => r.TotalAmount)
                })
                .OrderBy(g => g.Year)
                .ThenBy(g => g.Month)
                .ToList();

            // Prepare data for the chart
            var revenueLabels = revenueData.Select(r => $"{r.Month}/{r.Year}").ToList(); // Format as "Month/Year"
            var revenueAmounts = revenueData.Select(r => r.TotalRevenue).ToList(); // Total revenue per month

            // Prepare ViewModel to pass to the view
            var reportModel = new ReportsViewModel
            {
                TotalRentals = totalRentals,
                TotalRevenue = totalRevenue,
                TotalVehicles = totalVehicles,
                TotalCustomers = totalCustomers,
                VehicleUsage = vehicleUsage,
                RevenueLabels = revenueLabels,
                RevenueData = revenueAmounts
            };

            return View(reportModel);
        }


        // New action for About Us
        public IActionResult About()
        {
            return View(); // Returns the About.cshtml view
        }

        // New action for FAQ
        public IActionResult FAQ()
        {
            return View(); // Returns the FAQ.cshtml view
        }
    }
}