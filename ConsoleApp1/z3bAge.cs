using System;
using NUnit.Framework;
using static System.Console;

namespace ConsoleApp1.Chapter3.Ageb
{
    // Data object / custom type / anemic objects that can only represent a valid value for an age
    public class Age
    {
        private int Value { get; }
        public Age(int value)
        {
            if (!IsValid(value))
                // Age can only be instantiated with a valid value - but Dishonest (can not return Age as it can throw)
                throw new ArgumentException($"{value} is not a valid age"); // see below for Option improvement

            Value = value;
        }

        private static bool IsValid(int age)
            => 0 <= age && age < 120;

        // logic for comparing an Age with another Age
        public static bool operator <(Age l, Age r) => l.Value < r.Value;
        public static bool operator >(Age l, Age r) => l.Value > r.Value;

        // for readability make it possible to compare an Age with an int.
        // the int will first be converted to an Age
        public static bool operator <(Age l, int r) => l < new Age(r);
        public static bool operator >(Age l, int r) => l > new Age(r);

        public override string ToString() => Value.ToString();
    }

    public static class AgeThing
    {
        public static void Run()
        {
            WriteLine("hello!");
            var result = CalculateRiskProfile(new Age(20));
        }

        // Honest function - it honours its signature ie you will always end up with a Risk
        // it will never blow up with a runtime error as Age has to be valid
        public static Risk CalculateRiskProfile(Age age)
            => (age < 60) ? Risk.Low : Risk.Medium;

        // Dishonest function - it will not always abide by its signatured
        // ie will sometimes throw an exception
        public static Risk CalculateRiskProfileDishonest(int age)
        {
            if (age < 0 || 120 <= age)
                throw new ArgumentException($"{age} is not a valid age");

            return (age < 60) ? Risk.Low : Risk.Medium;
        }
    }

    public enum Risk { Low, Medium, High }

    public class AgeTests
    {
        [Test]
        public void CalculateRiskProfile_Simple()
        {
            var result = AgeThing.CalculateRiskProfile(new Age(20));
            Assert.AreEqual(Risk.Low, result);
        }
        [Test]
        public void CalculateRiskProfile_SimpleMedium()
        {
            var result = AgeThing.CalculateRiskProfile(new Age(60));
            Assert.AreEqual(Risk.Medium, result);
        }
    }
}
