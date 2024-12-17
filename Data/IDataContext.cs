using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Flight_Reservation_System.Models;

namespace Flight_Reservation_System.Data
{
   public class IDataContext : IdentityDbContext<IdentityUser>
   {
      public IDataContext(DbContextOptions<IDataContext> options) :base(options)
      {
         
      }

      public required DbSet<Flight> Flights {get; set;}

      // public required DbSet<User> Users {get; set;}


   }
}