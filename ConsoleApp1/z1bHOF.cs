using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApp1.Chapter1.HOF
{
    public static class HOFThing
    {
        public static void Run()
        {
            Console.WriteLine("hello z1bHOF!");
            // 4.1 HOF
            var numbers = new[] { 3, 5, 7, 9 };
            foreach (var prime in numbers.Find(IsPrime))
                Console.WriteLine(prime);
        }
        // 4.1 Higher Order Function - second parameter Func is another function which takes an int parameter
        // returns a bool 
        public static IEnumerable<int> Find(this IEnumerable<int> values, Func<int, bool> predicate)
        {
            foreach (var number in values)
                if (predicate(number))
                    yield return number;
        }
        public static bool IsPrime(int num)
        {
            for (long i = 2; i < num; i++)
                if (num % i == 0)
                    return false;
            return true;
        }
    }
}
