using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VehicleRentalAPI.Data;
using VehicleRentalAPI.Models;

namespace VehicleRentalAPI.Controllers
{
    public class RentalsController : Controller
    {
        private readonly VehicleRentalContext _context;

        public RentalsController(VehicleRentalContext context)
        {
            _context = context;
        }

        // Index method to display all rentals with related customer and vehicle details
        public async Task<IActionResult> Index()
        {
            var rentals = await _context.Rentals
                .Include(r => r.Vehicle)
                .Include(r => r.Customer)
                .ToListAsync();

            // Pass available customers and vehicles to the view
            ViewBag.Customers = await _context.Customers.ToListAsync(); // Assuming all customers are available
            ViewBag.Vehicles = await _context.Vehicles.Where(v => v.Availability == "Available").ToListAsync(); // Only available vehicles

            return View(rentals);
        }


        // Create method to add a new rental
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Rental rental)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Failed to create rental. Please check the inputs.";
                return RedirectToAction(nameof(Index));
            }

            // Validate ReturnDate is not earlier than RentalDate
            if (rental.ReturnDate.HasValue && rental.ReturnDate.Value < rental.RentalDate)
            {
                TempData["ErrorMessage"] = "Return date cannot be earlier than rental date.";
                return RedirectToAction(nameof(Index));
            }

            // Validate Customer and Vehicle existence
            if (!_context.Customers.Any(c => c.CustomerID == rental.CustomerID))
            {
                TempData["ErrorMessage"] = "Invalid Customer selected.";
                return RedirectToAction(nameof(Index));
            }

            var vehicle = await _context.Vehicles.FirstOrDefaultAsync(v => v.VehicleID == rental.VehicleID);
            if (vehicle == null)
            {
                TempData["ErrorMessage"] = "Invalid Vehicle selected.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                rental.RentalDate = rental.RentalDate.Date; // Normalize date
                if (rental.ReturnDate.HasValue)
                {
                    rental.ReturnDate = rental.ReturnDate.Value.Date; // Normalize date
                    rental.TotalAmount = CalculateTotalAmount(rental.RentalDate, rental.ReturnDate.Value, vehicle.PricePerDay); // Get price from the vehicle table
                }
                else
                {
                    rental.TotalAmount = 0;
                }

                _context.Add(rental);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Rental created successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error creating rental: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Rentals/Delete/5
        public IActionResult Delete(int id)
        {
            var rental = _context.Rentals
                                 .Include(r => r.Customer)  // Include customer data if needed
                                 .Include(r => r.Vehicle)   // Include vehicle data if needed
                                 .FirstOrDefault(r => r.RentalID == id);

            if (rental == null)
            {
                return NotFound();
            }

            return View(rental);
        }


        // POST: Rentals/Delete/5
        // POST: Rentals/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var rental = _context.Rentals.Find(id);
            if (rental != null)
            {
                _context.Rentals.Remove(rental);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Rental deleted successfully.";
            }

            return RedirectToAction(nameof(Index));  // Redirect back to the rentals list after deletion
        }



        // Utility method to calculate the total amount
        private decimal CalculateTotalAmount(DateTime rentalDate, DateTime returnDate, decimal pricePerDay)
        {
            int totalDays = (returnDate - rentalDate).Days + 1; // Inclusive of both days
            return totalDays * pricePerDay;
        }


        // Helper method to check if the Rental exists in the database
        private bool RentalExists(int id)
        {
            return _context.Rentals.Any(r => r.RentalID == id);
        }
    }
}

