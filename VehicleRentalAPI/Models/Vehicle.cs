using System.ComponentModel.DataAnnotations;

namespace VehicleRentalAPI.Models
{
    public class Vehicle
    {
        [Key]
        public int VehicleID { get; set; }
        [Required]
        public string Model { get; set; }
        [Required]
        public string Type { get; set; }
        [Required]
        public decimal PricePerDay { get; set; }
        [Required]
        public string Availability { get; set; }
        //public string ImageURL { get; set; }

        public byte[]? Image { get; set; }
    }
}
