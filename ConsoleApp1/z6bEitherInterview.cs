using System;
using LaYumba.Functional;
using static System.Math;

namespace ConsoleApp1.Chapter6.B
{
    using static F;

    static class Thing
    {
        internal static void Run()
        {
            Console.WriteLine("chapter6b");

            Option<Age> a = parseAge("26"); // => Some(26)

            // make parseAge return an Either

            var b = ParseIntVerbose("26"); // Right(26)
            var c = ParseIntVerbose("asdf"); // Left('asdf' is not a valid representation of an int)
        }

        static Either<string, int> ParseIntVerbose(this string s)
            => Int.Parse(s).ToEither(() => $"'{s}' is not a valid representation of an int");

        // ToEither : (Option<R>, Func<L>) -> Either<L, R>
        static Either<L, R> ToEither<L, R>(this Option<R> @this, Func<L> left)
            => @this.Match<Either<L, R>>(
                None: () => left(),
                Some: r => r);

        static Func<string, Option<Age>> parseAge = s
            // Int.Parse returns an Option<int>
            // Age.Of returns an Option<Age>
            => Int.Parse(s).Bind(Age.Of);
    }

    public class Age
    {
        private int Value { get; }

        // Smart constructor
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


        public override string ToString() => Value.ToString();
    }
}
