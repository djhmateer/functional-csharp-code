using System;
using System.Collections.Generic;
using System.Linq;
using LaYumba.Functional;

namespace ConsoleApp1.Chapter4.E
{
    using System.Collections.Generic;
    using static F;

    class PetsInNeighbourhood
    {
        internal static void Run()
        {
            var employees = new Dictionary<string, Employee>();
            employees.Add("dave", new Employee{Id = "dave", WorkPermit = 
                Some(new WorkPermit{Number = "123", Expiry = DateTime.Now.AddDays(-1)})});

            var a = GetWorkPermit(employees, "dave");

            var b = GetValidWorkPermit(employees, "dave");
        }

        static Option<WorkPermit> GetWorkPermit(Dictionary<string, Employee> employees, string employeeId)
            // Lookup is a function which looks up the dictionary key returning an Option
            // Bind is like Map (Select) except it returns a flattened list. Monad.
            => employees.Lookup(employeeId).Bind(e => e.WorkPermit);

        static Option<WorkPermit> GetValidWorkPermit(Dictionary<string, Employee> employees, string employeeId)
            => employees
                .Lookup(employeeId)
                .Bind(e => e.WorkPermit)
                .Where(HasExpired.Negate());

        static Func<WorkPermit, bool> HasExpired => permit => permit.Expiry < DateTime.Now.Date;


        // 4 Use Bind to implement AverageYearsWorkedAtTheCompany, shown below (only
        // employees who have left should be included).
        static double AverageYearsWorkedAtTheCompany(List<Employee> employees)
            => employees
                .Bind(e => e.LeftOn.Map(leftOn => YearsBetween(e.JoinedOn, leftOn)))
                .Average();

        static double YearsBetween(DateTime start, DateTime end)
            => (end - start).Days / 365d;
    }

    public struct WorkPermit
    {
        public string Number { get; set; }
        public DateTime Expiry { get; set; }
    }

    public class Employee
    {
        public string Id { get; set; }
        public Option<WorkPermit> WorkPermit { get; set; }

        public DateTime JoinedOn { get; }
        public Option<DateTime> LeftOn { get; }
    }
}
