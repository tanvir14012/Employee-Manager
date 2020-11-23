using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmployeeManager.Models.DTO.Invoice;
using Microsoft.AspNetCore.Identity;

namespace EmployeeManager.Models.Entity
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
                      
            });
        }
        public DbSet<Employee> EmployeeList { get; set; }
        public DbSet<Address> Addresses { get; set; }

    }
}
