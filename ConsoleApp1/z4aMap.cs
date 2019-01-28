using System;
using LaYumba.Functional;
using static System.Console;
using String = LaYumba.Functional.String;

namespace ConsoleApp1.Chapter4.A
{
    using System.Collections.Generic;
    using System.Linq;
    using static F;

    public class Examples
    {
        public static void List_Map()
        {
            Func<int, int> plus3 = x => x + 3;

            var a = new[] { 2, 4, 6 };
            // => [2, 4, 6]

            var b = a.Map(plus3);
            // => [5, 7, 9]
        }

        public static void List_ForEach()
        {
            Enumerable.Range(1, 5).ForEach(Console.Write);
        }

        internal static void Run()
        {
            Option<string> name = Some("Enrico");

            name
               .Map(String.ToUpper) // Maps works on Option
               .ForEach(WriteLine); // Foreach similar to Map, but takes an Action rather than a function, so its used to perform side effects

            IEnumerable<string> names = new[] { "Constance", "Brunhilde" };

            names
               .Map(String.ToUpper) // Map works on IEnumerable (so Option an IEnumerable are specialised containers)
               .ForEach(WriteLine);
        }
    }
}
