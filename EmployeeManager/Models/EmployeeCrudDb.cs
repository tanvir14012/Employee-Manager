using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManager.Models
{
    public class EmployeeCrudDb: IEmployeeCrud
    {
        private readonly AppDbContext context;

        public EmployeeCrudDb(AppDbContext context) {
            this.context = context;
        }

        public Employee Add(Employee employee)
        {
            context.EmployeesTwo.Add(employee);
            context.SaveChanges();
            return employee;
        }

        public void Delete(int Id)
        {
            context.EmployeesTwo.Remove(new Employee { Id = Id});
            context.SaveChanges();
        }

        public IEnumerable<Employee> GetAll()
        {
            return context.EmployeesTwo.AsNoTracking().ToList();
        }

        public Employee GetEmployee(int Id)
        {
            return context.EmployeesTwo.AsNoTracking().Where(e => e.Id == Id).FirstOrDefault();
        }

        public Employee Update(Employee employee)
        {
            context.EmployeesTwo.Update(employee);
            context.SaveChanges();
            
            return employee;
        }
    }
}
