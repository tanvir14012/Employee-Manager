using EmployeeManager.Models.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManager.Services
{
    public class EmployeeCrudDb: IEmployeeCrud
    {
        private readonly AppDbContext context;

        public EmployeeCrudDb(AppDbContext context) {
            this.context = context;
        }

        public Employee Add(Employee employee)
        {
            context.EmployeeList.Add(employee);
            context.SaveChanges();
            return employee;
        }

        public void Delete(int Id)
        {
            context.EmployeeList.Remove(new Employee { Id = Id});
            context.SaveChanges();
        }

        public IEnumerable<Employee> GetAll()
        {
            return context.EmployeeList.AsNoTracking().ToList();
        }

        public Employee GetEmployee(int Id)
        {
            return context.EmployeeList.AsNoTracking().Where(e => e.Id == Id).FirstOrDefault();
        }

        public Employee Update(Employee employee)
        {
            context.EmployeeList.Update(employee);
            context.SaveChanges();
            
            return employee;
        }
    }
}
