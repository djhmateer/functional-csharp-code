using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using LaYumba.Functional;
//using Enum = System.Enum;
using Enum = LaYumba.Functional.Enum;

namespace ConsoleApp1.Chapter3.ZExamples
{
    using static F;
    public static class ExamplesThing
    {
        public static void Run()
        {
            // 1 Write a generic function that takes a string and parses it as a value of an enum. It
            // should be usable as follows:

            Option<DayOfWeek> a = Enum.Parse<DayOfWeek>("Friday"); // => Some(DayOfWeek.Friday)
                                                                   // Enum.Parse<DayOfWeek>("Freeday") // => None

            // 2 Write a Lookup function that will take an IEnumerable and a predicate, and
            // return the first element in the IEnumerable that matches the predicate, or None
            // if no matching element is found. Write its signature in arrow notation:

            bool isOdd(int i) => i % 2 == 1;
            var b = new List<int>().Lookup(isOdd); // => None
            var c = new List<int> { 1 }.Lookup(isOdd); // => Some(1)

            // 3 email thing
            // Email is a type using a smart constructor to return a new Email(string) if the string is valid
            Option<Email> email = Email.Create("davemateer@gmail.com");

            var x = Thing.Create();
        }

        public class Thing
        {
            public static Thing Create()
            {
                return new Thing();
            }
        }

        // Lookup : IEnumerable<T> -> (T -> bool) -> Option<T>
        public static Option<T> Lookup<T>(this IEnumerable<T> ts, Func<T, bool> pred)
        {
            foreach (T t in ts)
                // return the first element that matches the predicate
                if (pred(t)) return Some(t);
            return None;
        }

    }

    // 3 Write a type Email that wraps an underlying string, enforcing that it’s in a valid
    // format. Ensure that you include the following:
    // - A smart constructor
    // - Implicit conversion to string, so that it can easily be used with the typical API
    // for sending emails

    public class Email
    {
        static readonly Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");

        private string Value { get; }

        private Email(string value) => Value = value;

        public static Option<Email> Create(string s)
            => regex.IsMatch(s)
                ? Some(new Email(s))
                : None;

        // implicit conversion to string
        public static implicit operator string(Email e)
            => e.Value;
    }

    enum DayOfWeek
    {
        Monday,
        Tuesday,
        Wednesday,
        Thursday,
        Friday,
        Saturday,
        Sunday
    }
}
