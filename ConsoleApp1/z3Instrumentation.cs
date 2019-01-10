using System;
using System.Diagnostics;
using System.IO;
using System.Text;

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
        public static void Time(string op, Action act)
        {
            var sw = new Stopwatch();
            sw.Start();

            act();

            sw.Stop();
            Console.WriteLine($"{op} took {sw.ElapsedMilliseconds}ms");
        }

        public static class InstrumentationThing
        {
            public static void Run()
            {
                Console.WriteLine("hello!");
                var filename = @"..\..\..\file.txt";
                // Function returns a string (the contents of the file)
                // lambda expression - there are no input arguments so ()
                Func<string> read = () => File.ReadAllText(filename);

                //var contents = Instrumentation.Time("reading from file.txt" , () => File.ReadAllText("file.txt"));
                var contents = Instrumentation.Time("reading from file.txt", read);
                Console.WriteLine($"contents: {contents}");

                // but what about a void returning function eg

                //Func<> write = () => File.AppendAllText("file.txt", "New content!", Encoding.UTF8);
                Action write = () => File.AppendAllText(filename, "New content!", Encoding.UTF8);


                //Instrumentation.Time("writing to file.txt" , () => File.AppendAllText("file.txt", "New content!", Encoding.UTF8));
                Instrumentation.Time("writing to file.txt", write);
            }
        }

        public enum Risk
        {
            Low,
            Medium,
            High
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
}
