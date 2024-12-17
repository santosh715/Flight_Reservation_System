using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
 
namespace Flight_Reservation_System.Models
{
    public class Flight
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int FlightID { get; set; }
 
        [Required]
        [StringLength(20)]
        public required string FlightNumber { get; set; }
 
        [Required]
        [StringLength(100)]
        public required string FlightName { get; set; }
 
        [Required]
        [Column(TypeName = "date")] // FlightDate should be a Date type in the database
        public DateOnly FlightDate { get; set; }
 
        [Required]
        [Column(TypeName = "time(0)")]
        public TimeOnly DepartureTime { get; set; } // TimeSpan to store time only
 
        [Required]
        [Column(TypeName = "time(0)")]
        public TimeOnly ArrivalTime { get; set; }
 
        [Required]
        [StringLength(100)]
        public required string DepartureLocation { get; set; }
 
        [Required]
        [StringLength(100)]
        public required string ArrivalLocation { get; set; }
 
        [Required]
        public int TotalSeats { get; set; }
 
        [Required]
        public int AvailableSeats { get; set; }
 
        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal Price { get; set; }
 
        [Required]
        public bool isActive { get; set; }
    }

    
}