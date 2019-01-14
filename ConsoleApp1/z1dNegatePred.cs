using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApp1.Chapter1.Pred
{
public static class PredThing
{
    public static void Run()
    {
        // 1. Write a function that negates a given predicate: whenever the given predicate
        // evaluates to `true`, the resulting function evaluates to `false`, and vice versa.

        var numbers = new[] { 3, 5, 7, 9 };

        // function assigned to a variable takes an int and returns a bool
        Func<int, bool> isPrime = IsPrime;

        foreach (var prime in numbers.Where(isPrime.Negate()))
            Console.WriteLine(prime);
    }

    public static bool IsPrime(int number)
    {
        for (long i = 2; i < number; i++)
            if (number % i == 0)
                return false;
        return true;
    }

    // extension method on Func<t, bool>
    // returns a Func<T, bool>
    // negates a predicate
    public static Func<T, bool> Negate<T>(this Func<T, bool> pred)
    {
        return t => !pred(t);
    }
}
}
