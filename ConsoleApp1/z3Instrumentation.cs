using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using static ConsoleApp1.Chapter3.Instrumentation.F;
using Unit = System.ValueTuple; // empty tuple can only have 1 possible value,  so its as good as no value

namespace ConsoleApp1.Chapter3.Instrumentation
{
    public static class Instrumentation
    {
        // Function returns T - in the case below it is string
        public static T Time<T>(string op, Func<T> f)
        {
            var sw = new Stopwatch();
            sw.Start();

            // run the function returning T
            T t = f();

            sw.Stop();
            Console.WriteLine($"{op} took {sw.ElapsedMilliseconds}ms");
            return t;
        }

        // Had to overload Time just to get Action working
        //public static void Time(string op, Action act)
        //{
        //    var sw = new Stopwatch();
        //    sw.Start();

        //    act();

        //    sw.Stop();
        //    Console.WriteLine($"{op} took {sw.ElapsedMilliseconds}ms");
        //}
    }

    // write an adapter function to modify existing function to convert an Action into a Func<Unit>
    public static class F
    {
        // convenience method that allows you to write return Unit() in functions that return Unit.
        public static Unit Unit() => default(Unit);
    }

    public static class ActionExt
    {
        // extension method on Action that returns Func<Unit>
        public static Func<Unit> ToFunc(this Action action)
        {
            // local function
            Unit Func()
            {
                action();
                // Unit() is F.Unit() which is default(Unit) - Unit() is just a shorthand
                return Unit();
            }

            // could refactor this lambda to a method group
            return () => Func();
        }

        // extension method on Action that returns Func<Unit>
        public static Func<Unit> ToFunc2(this Action action)
        {
            // return a lambda which takes no parameters
            return () =>
            {
                action(); // run the action function
                return Unit(); // return the System.ValueTuple ie our concept of nothing
            };
        }

        // Adapter function for Action<T> ie takes T as a parameter and returns Unit
        public static Func<T, Unit> ToFunc<T>(this Action<T> action)
            => (t) =>
            {
                action(t);
                return Unit();
            };
    }

    public static class InstrumentationThing
    {
        public static void Run()
        {
            Console.WriteLine("hello!");
            var filename = @"..\..\..\file.txt";
            // Function returns a string (the contents of the file)
            // lambda expression - there are no input parameters so ()
            Func<string> read = () => File.ReadAllText(filename);

            //var contents = Instrumentation.Time("reading from file.txt" , () => File.ReadAllText("file.txt"));
            var contents = Instrumentation.Time("reading from file.txt", read);
            Console.WriteLine($"contents: {contents}");

            // but what about a void returning function which takes no parameters 
            //Action write = () => File.AppendAllText(filename, "New content!", Encoding.UTF8);
            Action write = () => File.AppendAllText(filename, "New content!", Encoding.UTF8);
            
            // uses adapter function .ToFunc() to return a ValueTuple (Unit)
            Instrumentation.Time("writing to file.txt", write.ToFunc());
        }
    }



    public class AgeTests
    {
        //    [Test]
        //    public void CalculateRiskProfile_Simple()
        //    {
        //        var result = AgeThing.CalculateRiskProfile(new Age(20));
        //        Assert.AreEqual(Risk.Low, result);
        //    }
        //    [Test]
        //    public void CalculateRiskProfile_SimpleMedium()
        //    {
        //        var result = AgeThing.CalculateRiskProfile(new Age(60));
        //        Assert.AreEqual(Risk.Medium, result);
        //    }
    }
}
