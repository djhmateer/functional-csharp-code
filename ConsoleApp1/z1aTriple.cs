using System;
using System.Linq;
using static System.Console;
using static System.Linq.Enumerable;

namespace ConsoleApp1.Chapter1.Triple
{
    public static class TripleThing
    {
        public static void Run()
        {
            WriteLine("hello z1triple!");
            // 1. Functions as first class values
            // function assigned to variable triple takes one int parameter and returns an int 
            // lambda expression (executable code without a method name) returns whatever int value is passed in times 3

            // The variable triple is assigned to a lambda expression function 
            Func<int, int> triple = x => x * 3;
            var a = triple(4); // 12

            // Passing the function around creating more concise code
            // and higher level of abstraction
            var b = Range(1, 100)
                    .Select(x => triple(x)); // 3, 6, 9..

            // 2. Avoid State Mutation
            // Create and populate an array
            int[] nums = { 1, 2, 3 };
            // Updates the first value of the array
            nums[0] = 7;
            var c = nums; // => [7, 2, 3]

            // Function (predicate signature) accepts an int and returns a bool
            Func<int, bool> isOdd = x => x % 2 == 1;
            var d = isOdd(3); // true

            int[] original = { 7, 6, 1 };
            var sorted = original.OrderBy(x => x); // [1, 6, 7]
            var filtered = original.Where(isOdd); // [7, 1]
            var e = original; // [7, 6, 1]
        }
    }
}
