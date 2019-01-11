using System;
using System.Linq;
using NUnit.Framework;
using static System.Math;
using static System.Console;

namespace ConsoleApp1.Chapter1.Triple
{
    public static class TripleThing
    {
        public static void Run()
        {
            Console.WriteLine("hello z1triple!");
            // 1. Functions as first class values
            // function assigned to variable triple takes one int parameter and returns an int 
            // lambda expression (executable code without a method name) returns whatever int value is passed in times 3

            // 1. the variable triple is assigned to a lambda expression 
            Func<int, int> triple = x => x * 3;
            var a = triple(4); // 12


            // passing the function around creating more concise code
            // and higher level of abstraction
            var g = Enumerable.Range(1, 100)
                .Select(x => triple(x));

            // more verbose putting the lambda expression inline
            var h = Enumerable.Range(1, 100)
                .Select(x => x * 3);
        }
    }

    public class AgeTests
    {
        //[TestCase(x20, ExpectedResult = Risk.Low)]
        //[TestCase(70, ExpectedResult = Risk.Medium)]
        //public Risk CalculateRiskProfile_Simple(Age number)
        //{
        //    var result = AgeThing.CalculateRiskProfile(number);
        //    return result;

        //    [Test]
        //    public void CalculateRiskProfile_SimpleMedium()
        //    {
        //        var result = AgeThing.CalculateRiskProfile(new Age(60));
        //        Assert.AreEqual(Risk.Medium, result);
        //    }
    }
}
