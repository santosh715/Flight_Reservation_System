using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
 
public class CreateFlightDto
{
    [Required]
    [StringLength(20)]
    public required string FlightNumber { get; set; }
 
    [Required]
    [StringLength(100)]
    public required string FlightName { get; set; }
 
    [Required]
    public DateOnly FlightDate { get; set; }
 
    [Required]
    public TimeOnly DepartureTime { get; set; }
 
    [Required]
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