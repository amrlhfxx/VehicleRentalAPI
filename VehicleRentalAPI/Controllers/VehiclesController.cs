using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VehicleRentalAPI.Data;
using VehicleRentalAPI.Models;
using System.IO;

namespace VehicleRentalAPI.Controllers
{
    public class VehiclesController : Controller
    {
        private readonly VehicleRentalContext _context;

        public VehiclesController(VehicleRentalContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var vehicles = await _context.Vehicles.ToListAsync();
            return View(vehicles);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Vehicle vehicle, IFormFile? ImageFile)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Failed to add the vehicle. Please check the inputs.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                // Convert the uploaded image file to binary data
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await ImageFile.CopyToAsync(memoryStream);
                        vehicle.Image = memoryStream.ToArray();
                    }
                }

                _context.Add(vehicle);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Vehicle added successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error adding vehicle: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Vehicle vehicle, IFormFile? ImageFile)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Failed to update the vehicle. Please check the inputs.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await ImageFile.CopyToAsync(memoryStream);
                        vehicle.Image = memoryStream.ToArray();
                    }
                }
                else
                {
                    // Retain the existing image if no new image is uploaded
                    var existingVehicle = await _context.Vehicles.AsNoTracking().FirstOrDefaultAsync(v => v.VehicleID == vehicle.VehicleID);
                    if (existingVehicle != null)
                    {
                        vehicle.Image = existingVehicle.Image;
                    }
                }

                _context.Update(vehicle);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Vehicle updated successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error updating vehicle: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var vehicle = await _context.Vehicles.FindAsync(id);
                if (vehicle == null)
                {
                    TempData["ErrorMessage"] = "Vehicle not found.";
                    return RedirectToAction(nameof(Index));
                }

                _context.Vehicles.Remove(vehicle);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Vehicle deleted successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error deleting vehicle: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
