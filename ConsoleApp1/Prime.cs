using System;
using NUnit.Framework;
using static System.Math;
using static System.Console;

namespace ConsoleApp1.Chapter2.Prime
{
    // 1. Write a console app that calculates a user's Body-Mass Index:
    //   - prompt the user for a number
    //   - calculate if it is prime
    //   - output to the console

    // 2. Structure your code so that structure it so that pure and impure parts are separate
    // 3. Unit test the pure parts
    // 4. Unit test the impure parts using the HOF-based approach

    // see the Exercises project for a fully commented version

    static class Prime
    {
        public static void Run()
        {
            // injecting functions as dependencies (so we are able to test the Run method)
            // passing impure functions into the Run HOF
            Run(Read, Write);
        }

        // HOF returns void, read is a function which takes a string a returns a double,
        // write function that takes a bool and returns void 
        internal static void Run(Func<int> read, Action<bool> write)
        {
            // input
            // using the injected function to do a Console.Read and Parse to do an int
            int number = read();

            // computation
            // static function easy to test
            bool isNumberPrime = IsPrime(number); 

            // output
            // using injected function to Console.WriteLine
            write(isNumberPrime);
        }

        internal static bool IsPrime(int number)
        {
            for (long i = 2; i < number; i++)
                if (number % i == 0)
                    return false;
            return true;
        }

        // Impure function (will not test)
        // I/O always considered a side effect (as what happens in the outside world will effect the int returned)
        private static int Read()
        {
            WriteLine($"Please enter an int");
            return int.Parse(ReadLine());
        }

        // Impure function (will not test)
        private static void Write(bool isPrime)
            => WriteLine($"isPrime: {isPrime}");
    }

    public class PrimeTests
    {
        [TestCase(3, ExpectedResult = true)]
        [TestCase(4, ExpectedResult = false)]
        [TestCase(5, ExpectedResult = true)]
        [TestCase(6, ExpectedResult = false)]
        public bool IsPrime_Simple(int number)
        {
            var result = Prime.IsPrime(number);
            return result;
        }
    }
}
