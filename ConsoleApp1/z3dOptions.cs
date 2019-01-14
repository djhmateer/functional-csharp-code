using System;
using LaYumba.Functional;

namespace ConsoleApp1.Chapter3.D
{
    using static LaYumba.Functional.F;
    public static class OptionThing
    {
        public static void Run()
        {
            // creates an Option in the None state
            // convention to use _ when a variable is ignored
            // note this is not a C#7 discard https://stackoverflow.com/questions/42920622/c7-underscore-star-in-out-variable/42924200
            // normal variable  with identifier _
            Option<string> _ = None;

            // Option is in the Some state
            Option<string> john = Some("John");

            // want to run different code based on if the Option is None or Some
            string result = Greet(_); // Sorry, who?
            string r2 = Greet("Dave"); // Hello, Dave
            string r3 = Greet(john); // Hello, John

            // 2. Subscriber
            var dave = new Subscriber { Name = "Dave", Email = "davemateer@gmail.com" };
            var newsletter = GreetingFor(dave); // Dear DAVE
            var anon = new Subscriber { Email = "anon@gmail.com" };
            var n2 = GreetingFor(anon); // Dear Subscriber

            // 3. Parsing strings
            // Int.Parse if defined below to return an Option<int>
            Option<int> i1 = Int.Parse("10"); // Some(10)

            // forcing the caller to deal with the None case 
            string rb = i1.Match(
                    () => "Not an int!",
                    i => $"number is {i}"
                 );
            Console.WriteLine(rb);

            // don't want to be able to do i1.HasValue() as this defeats the idea
            // point is we want to make unconditional calls to the contens without testing whether the content is there
            Option<int> i2 = Int.Parse("hello"); // None
            int asdf = i2.Match(
                () => 0, // so if the original parse fails, we set it to 0
                x => x); // asdf is 0

        }

        public static string Greet(Option<string> greetee) =>
            // Match (essentially this is a null check) takes 2 functions - for None and Some
            greetee.Match(
                None: () => "Sorry, who?",
                Some: (name) => $"Hello, {name}"
            );

        // conceptually Greet is similar to Greet2
        public static string Greet2(string name)
            => (name == null)
                ? "Sorry, who?"
                : $"Hello, {name}";

        // 2. By using Option you're forcing  the users of the API to handle the case in which no data is available
        // trading run time errors, for compile time errors
        public static string GreetingFor(Subscriber subscriber) =>
            subscriber.Name.Match(
                () => "Dear Subscriber",
                name => $"Dear {name.ToUpper()}");
    }

    public static class Int
    {
        public static Option<int> Parse(string s) => int.TryParse(s, out var result) ? Some(result) : None;
    }

    public class Subscriber
    {
        // rather than nullable, the Name is now optional
        public Option<string> Name { get; set; }
        public string Email { get; set; }
    }
}
