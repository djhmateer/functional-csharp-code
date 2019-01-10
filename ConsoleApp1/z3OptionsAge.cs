using System;
using LaYumba.Functional;
using NUnit.Framework;

namespace ConsoleApp1.Chapter3.OptionsAge
{
    using static F;
    // data object / custom type / anemic objects that can only represent a valid value for an age
    // structs are value types - he uses a struct here
    // classes are reference types
    public class Age
    {
        private int Value { get; }

        // smart constructor
        public static Option<Age> Of(int age)
            => IsValid(age) ? Some(new Age(age)) : None;
        // private ctor
        private Age(int value)
        {
            if (!IsValid(value))
                // Age can only be instantiated with a valid value
                throw new ArgumentException($"{value} is not a valid age"); 

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
            Console.WriteLine("hello!");
            //var result = CalculateRiskProfile(new Age(20));
            // this isn't going to work as we need to handle the None case in the Option<Age>
            //var result = CalculateRiskProfile(Age.Of(20));

            // regular variable pointing to a function
            // taking a string and returning an Option<Age>
            // using Bind!!!
            Func<string, Option<Age>> parseAge = s => Int.Parse(s).Bind(Age.Of);

            var a = parseAge("26"); // => Some(26)

            // how to work with Option<Age>?
            // Match is easiest
        }

        // honest function - it honours its signature ie you will always end up with a Risk
        // it will never blow up with a runtime error as Age has to be valid
        public static Risk CalculateRiskProfile(Age age)
            => (age < 60) ? Risk.Low : Risk.Medium;
    }

    public enum Risk { Low, Medium, High }
}
