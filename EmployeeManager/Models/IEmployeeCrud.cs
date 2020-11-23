using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManager.Models
{
    public interface IEmployeeCrud
    {
        Employee GetEmployee(int Id);
        IEnumerable<Employee> GetAll();
        Employee Add(Employee employee);
        Employee Update(Employee employee);
        void Delete(int Id);
    }
}
