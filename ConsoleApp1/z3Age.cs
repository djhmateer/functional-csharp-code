﻿using System;
using NUnit.Framework;
using static System.Math;
using static System.Console;

namespace ConsoleApp1.Chapter3.Age
{
    public class Age
    {
        private int Value { get; }
        private Age(int value)
        {
            if (!IsValid(value))
                throw new ArgumentException($"{value} is not a valid age");

            Value = value;
        }

        private static bool IsValid(int age)
            => 0 <= age && age < 120;

        public static bool operator <(Age l, Age r) => l.Value < r.Value;
        public static bool operator >(Age l, Age r) => l.Value > r.Value;

        public static bool operator <(Age l, int r) => l < new Age(r);
        public static bool operator >(Age l, int r) => l > new Age(r);

        public override string ToString() => Value.ToString();
    }

    public static class AgeThing
    {
        public static void Run()
        {
            Console.WriteLine("hello!");
        }

        public static Risk CalculateRiskProfile(Age age)
            => (age < 60) ? Risk.Low : Risk.Medium;
    }

    public enum Risk { Low, Medium, High }

    public class PrimeTests
    {
        [TestCase(20, ExpectedResult = Risk.Low)]
        [TestCase(70, ExpectedResult = Risk.Medium)]
        public Risk CalculateRiskProfile_Simple(Age number)
        {
            var result = AgeThing.CalculateRiskProfile(number);
            return result;
        }
    }


}
