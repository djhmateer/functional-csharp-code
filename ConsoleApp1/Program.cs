using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using static System.Console;

namespace ConsoleApp1
{
    class Program
    {
        static void Main()
        {
            WriteLine("Hello World!");
            ListFormatter._main();
        }
    }

    class ListFormatter
    {
        int counter;

        // impure function - mutates global state
        string PrependCounter(string s) => $"{++counter}. {s}";

        // pure and impure functions applied similarly
        public List<string> Format(List<string> list)
            => list
                .Select(StringExt.ToSentenceCase)
                .Select(PrependCounter)
                .ToList();

        //internal static void _main()
        public static void _main()
        {
            var shoppingList = new List<string> { "coffee beans", "BANANAS", "Dates" };

            new ListFormatter()
                .Format(shoppingList)
                .ForEach(WriteLine);

            //Read();
        }
    }

    public static class StringExt
    {
        // Pure function (no side effects)
        // because its computation only depends on the input parameter it can be made static
        public static string ToSentenceCase(this string s)
            => s.ToUpper()[0] + s.ToLower().Substring(1);
    }

    public class ListFormatter_InstanceTests
    {
        [Test]
        public void ItWorksOnSingletonList()
        {
            var input = new List<string> { "coffee beans" };
            var output = new ListFormatter().Format(input);
            Assert.AreEqual("1. Coffee beans", output[0]);
        }

        [Test]
        public void ItWorksOnLongerList()
        {
            var input = new List<string> { "coffee beans", "BANANAS" };
            var output = new ListFormatter().Format(input);
            Assert.AreEqual("1. Coffee beans", output[0]);
            Assert.AreEqual("2. Bananas", output[1]);
        }

        [Test]
        public void ItWorksOnAVeryLongList()
        {
            var size = 100000;
            var input = Enumerable.Range(1, size).Select(i => $"item{i}").ToList();
            var output = new ListFormatter().Format(input);
            Assert.AreEqual("100000. Item100000", output[size - 1]);
        }
    }
}
