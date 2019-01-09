using System;
using NUnit.Framework;
using static System.Math;

namespace ConsoleApp1.Chapter2.Bmi
{
    // 1. Write a console app that calculates a user's Body-Mass Index:
    //   - prompt the user for her height in metres and weight in kg
    //   - calculate the BMI as weight/height^2
    //   - output a message: underweight(bmi<18.5), overweight(bmi>=25) or healthy weight
    // 2. Structure your code so that structure it so that pure and impure parts are separate
    // 3. Unit test the pure parts
    // 4. Unit test the impure parts using the HOF-based approach

    // see the Exercises project for a fully commented version

    static class Bmi
    {
        public static void Run()
        {
            // impure parts?
            Console.WriteLine("Height in metres");
            var height = Console.ReadLine();

            Console.WriteLine("Weight in kg");
            var weight = Console.ReadLine();
        }

        // pure function
        public static double GetBMI(double height, double weight)
        {
            return weight / Pow(height,2);
        }
    }

    public class Tests
    {
        [TestCase(1.80, 77, ExpectedResult = 23.77)]
        [TestCase(1.60, 77, ExpectedResult = 30.08)]
        public double GetBMI_Simple(double height, double weight)
        {
            var result = Bmi.GetBMI(height, weight);
            return result;
        }
    }
}
