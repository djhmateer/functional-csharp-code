using System;
using NUnit.Framework;

namespace Exercises.Chapter2.Solutions
{
    // 1. Write a console app that calculates a user's Body-Mass Index:
    //   - prompt the user for her height in metres and weight in kg
    //   - calculate the BMI as weight/height^2
    //   - output a message: underweight(bmi<18.5), overweight(bmi>=25) or healthy weight
    // 2. Structure your code so that structure it so that pure and impure parts are separate
    // 3. Unit test the pure parts
    // 4. Unit test the impure parts using the HOF-based approach

    using static Console;
    using static Math;

    public enum BmiRange { Underweight, Healthy, Overweight }

    public static class Bmi
    {
        public static void Run()
        {
            // injecting functions as dependencies (so we are able to test the Run method)
            // passing impure functions into the Run HOF
            Run(Read, Write);
        }

        // HOF returns void, read is a function which takes a string a returns a double, write function that takes a BmiRange and returns void 
        internal static void Run(Func<string, double> read, Action<BmiRange> write)
        {
            // input
            // multiple declarators C#3
            // using the injected function to do a Console.Read and Parse to do a double
            double weight = read("weight")
                 , height = read("height");

            // computation
            // static function and extension method on double easy to test as both pure functions
            var bmiRange = CalculateBmi(height, weight).ToBmiRange();

            // output
            // using injected function to Console.WriteLine
            write(bmiRange);
        }
        
        // Isolated the pure computational functions below from impure I/O
        internal static double CalculateBmi(double height, double weight)
           => Round(weight / Pow(height, 2), 2);

        internal static BmiRange ToBmiRange(this double bmi)
           => bmi < 18.5 ? BmiRange.Underweight
              : 25 <= bmi ? BmiRange.Overweight
              : BmiRange.Healthy;

        // Impure function (will not test)
        // I/O always considered a side effect (as what happens in the outside world will effect the double returned)
        private static double Read(string field)
        {
            WriteLine($"Please enter your {field}");
            return double.Parse(ReadLine());
        }

        // Impure function (will not test)
        private static void Write(BmiRange bmiRange)
           => WriteLine($"Based on your BMI, you are {bmiRange}");
    }

    public class BmiTests
    {
        // Easy to test the pure computational functions!
        [TestCase(1.80, 77, ExpectedResult = 23.77)]
        [TestCase(1.60, 77, ExpectedResult = 30.08)]
        public double CalculateBmi(double height, double weight)
           => Bmi.CalculateBmi(height, weight);

        // testing ToBmiRange
        [TestCase(23.77, ExpectedResult = BmiRange.Healthy)]
        [TestCase(30.08, ExpectedResult = BmiRange.Overweight)]
        public BmiRange ToBmiRange(double bmi) => bmi.ToBmiRange();

        // testing Run
        // this is good as testing the actual output of the program (and not just units)
        // just not testing the impure functions (faking them)
        [TestCase(1.80, 77, ExpectedResult = BmiRange.Healthy)]
        [TestCase(1.60, 77, ExpectedResult = BmiRange.Overweight)]
        public BmiRange ReadBmi(double height, double weight)
        {
            var result = default(BmiRange);
            // defining two pure fake functions to pass into the HOF
            // takes a string as input (the field name) and returns a double
            // we don't need to double.Parse as we control the test data
            Func<string, double> read = s => s == "height" ? height : weight;
            // takes a BmiRange and returns void 
            // uses a local variable (result) to hold the value of BmiRange passed into the function, which the test returns
            Action<BmiRange> write = r => result = r;

            Bmi.Run(read, write);
            return result;
        }
    }
}
