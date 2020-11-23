using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManager.Models
{
    public class EmployeeCrud : IEmployeeCrud
    {
        private List<Employee> _employees;

        public EmployeeCrud()
        {
            _employees = new List<Employee>() {
               new Employee {Id = 1, Name = "Jack Ali", Email = "jack@ali.com", Department = Dept.SALES},
               new Employee {Id = 2, Name = "Flock Plask", Email = "pl@ak.re", Department = Dept.ENGG},
               new Employee {Id = 3, Name = "Guerrio Gillepsi", Email = "apl@yahoo.com", Department = Dept.HR},
            };
        }

        public Employee Add(Employee employee)
        {
            int Id = _employees.Max(e => e.Id) + 1;
            employee.Id = Id;
            _employees.Add(employee);
            return employee;
        }

        public void Delete(int Id)
        {
            throw new NotImplementedException();
        }

        public Employee Get(int id)
        {
            return _employees.Find(e => e.Id == id);
        }

        public IEnumerable<Employee> GetAll()
        {
            return _employees.ToList();
        }

        public Employee GetEmployee(int Id)
        {
            return _employees.FirstOrDefault(e => e.Id == Id);
        }

        public Employee Update(Employee employee)
        {
            var currEmp = _employees.Find(e => e.Id == employee.Id);
            currEmp.Name = employee.Name;
            currEmp.Email = employee.Email;
            currEmp.Department = employee.Department;
            return currEmp;
        }
    }
}
