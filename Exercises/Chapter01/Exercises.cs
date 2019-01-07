using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Exercises.Chapter1
{
    public static class Exercises
    {
        // 1. Write a function that negates a given predicate: whenever the given predicate
        // evaluates to `true`, the resulting function evaluates to `false`, and vice versa.
        public static void PrimeTest()
        {
            var numbers = new[] { 3, 5, 7, 9 };

            // function assigned to a variable takes an int and returns a bool
            Func<int, bool> isPrime = IsPrime;
            foreach (var prime in numbers.Where(isPrime.Negate()))
                Console.WriteLine(prime);
        }

        // extension method on Func<t, bool>
        // returns a Func<T, bool>
        // negates a predicate
        public static Func<T, bool> Negate<T>(this Func<T, bool> pred)
        {
            return t => !pred(t);
        }

        public static bool IsPrime(int number)
        {
            for (long i = 2; i < number; i++)
                if (number % i == 0)
                    return false;
            return true;
        }

        // 2. Write a method that uses quicksort to sort a `List<int>` (return a new list,
        // rather than sorting it in place).

        public static void QuickSortTest()
        {
            var numbers = new List<int> { 1, 4, 2, 8, 3 };
            var result = numbers.QuickSort();
            result.ForEach(x => Console.WriteLine(x));
        }

        static List<int> QuickSort(this List<int> list)
        {
            if (list.Count == 0) return new List<int>();

            var pivot = list[0];
            var rest = list.Skip(1);

            var small = rest.Where(x => x <= pivot);
            var large = rest.Where(x => pivot < x);

            return QuickSort(small.ToList())
                .Append(pivot)
                .Concat(QuickSort(large.ToList()))
                .ToList();
        }



        // 3. Generalize your implementation to take a `List<T>`, and additionally a 
        // `Comparison<T>` delegate.

        // 4. In this chapter, you've seen a `Using` function that takes an `IDisposable`
        // and a function of type `Func<TDisp, R>`. Write an overload of `Using` that
        // takes a `Func<IDisposable>` as first
        // parameter, instead of the `IDisposable`. (This can be used to fix warnings
        // given by some code analysis tools about instantiating an `IDisposable` and
        // not disposing it.)
    }
}
