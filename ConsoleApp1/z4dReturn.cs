using LaYumba.Functional;

namespace ConsoleApp1.Chapter4.D
{
    using System.Collections.Generic;
    using static F;

    class PetsInNeighbourhood
    {
        internal static void Run()
        {
            // List is a function which returns an immutable list for the monad IEnumerable
            IEnumerable<string> empty = List<string>();

            var singleton = List("Dave");

            // nice shorthand syntax for initialising immutable lists
            var many = List("Dave", "Bob", "Alice");

        var a = ToNatural("2"); // Some(2)
        var b = ToNatural("-2"); // None
        var c = ToNatural("hello"); // None
    }

    // Parse is a function that does the int parse and returns Some or None
    static Option<int> ToNatural(string s) => Int.Parse(s).Where(IsNatural);

    static bool IsNatural(int i) => i >= 0;
    }
}
