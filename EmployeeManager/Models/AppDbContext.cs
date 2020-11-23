using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmployeeManager.Models.Practice;

namespace EmployeeManager.Models
{
    public class AppDbContext: IdentityDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options): base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Address>(entity =>
            {
                entity.Property(e => e.Id).IsRequired().UseSqlServerIdentityColumn();
                entity.HasOne(e => e.Order)
                      .WithOne(e => e.Address)
                      .HasForeignKey<Address>(e => e.OrderId)
                      .OnDelete(DeleteBehavior.Cascade);
                      
            });
        }
        public DbSet<Employee> EmployeesTwo { get; set; }

        public DbSet<Order> Orders { get; set; }
        public DbSet<Address> Addresses { get; set; }

    }
}
