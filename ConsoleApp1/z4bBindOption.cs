using LaYumba.Functional;
using System;
using static System.Console;

namespace ConsoleApp1.Chapter4.B
{
    using System.Collections.Generic;
    using System.Linq;
    using static F;

    public static class AskForValidAgeAndPrintFlatteringMessage
    {
        // Reads users age from console
        // prints out related message
        // Error handling - the age should be valid!
        // Notice lack of if..else statments. Operating on higher level of abstraction with Option so handled

        public static void Run()
            => WriteLine($"Only {ReadAge()}! That's young!");

        static Age ReadAge()
            => ParseAge(Prompt("Please enter your age")).Match(
             () => ReadAge(), // if ParseAge come back as None prompt again
             (age) => age);

        static string Prompt(string prompt)
        {
            WriteLine(prompt);
            return ReadLine();
        }

        static Option<Age> ParseAge(string s)
        {
            // Apply the Age.Of function to each element of the optI (single, could be None)
            // 2 Option types combined give us this problem to work with
            //Option<int> optI = Int.Parse(s);
            //Option<Option<Age>> ageOpt = optI.Map(x => Age.Of(x));

            // Using Bind to chain two functions that return Option so we get a flattened Option<age>
            return Int.Parse(s)
                  .Bind(x => Age.Of(x));
        }
    }

    public class Age
    {
        private int Value { get; }
        // private ctor - enforcing that Value can only be set on instantiation
        private Age(int value) => Value = value;

        private static bool IsValid(int age)
            => 0 <= age && age < 120;

        // smart constructor that creates an Age instance from the given int (returns an Option)
        public static Option<Age> Of(int age)
            => IsValid(age) ? Some(new Age(age)) : None;

        public override string ToString() => Value.ToString();
    }


    class SurveyOptionalAge
    {
        class Person
        {
            public Option<int> Age { get; set; }
        }

        static IEnumerable<Person> Population => new[]
        {
            new Person { Age = Some(33) },
            new Person { }, // this person did not disclose her age
            new Person { Age = Some(37) },
        };

        internal static void _main()
        {
            IEnumerable<Option<int>> optionalAges = Population.Map(p => p.Age);
            // => [Some(33), None, Some(37)]

            var statedAges = Population.Bind(p => p.Age);
            // => [33, 37]

            var averageAge = statedAges.Average();
            // => 35
        }
    }
}
