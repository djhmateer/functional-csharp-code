using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using static System.Linq.Enumerable;

namespace ConsoleApp1.Chapter2.ListFormatter
{
    using static System.Console;
    using static System.Linq.Enumerable;

    public static class Thing
    {
        public static void Run()
        {
            var input = new List<string> { "coffee beans", "BANANAS", "Dates" };
            var output = new ListFormatter().Format(input);
            // Method group - same as writing x => WriteLine(x)
            output.ForEach(WriteLine);

            ListFormatter2.Format(input).ForEach(WriteLine);
            ListFormatter3.Format(input).ForEach(WriteLine);
        }
    }

    // Try 3 - no shared state, so easy to parallelise
    static class ListFormatter3
    {
        public static List<string> Format(List<string> list) =>
            list.AsParallel()
                .Select(StringExt.ToSentenceCase) 
                .Zip(ParallelEnumerable.Range(1, list.Count), (s, i) => $"{i}. {s}") // s is string, i is int
                .ToList();
    }

    // Try 2 - pure function as not using a mutable counter
    static class ListFormatter2
    {
        // when all variables required within a method are provided as input the method can be static
        public static List<string> Format(List<string> list) =>
            list
                .Select(StringExt.ToSentenceCase) // Method group
                .Zip(Range(1, list.Count), resultSelector: (s, i) => $"{i}. {s}") // s is string, i is int
                .ToList();
    }

    // Try 1
    class ListFormatter
    {
        int counter;

        // Impure function - mutates global state
        private string PrependCounter(string s)
        {
            return $"{++counter}. {s}";
        }

        // Pure and impure functions applied similarly
        // Expression body syntax C#6
        public List<string> Format(List<string> list)
            => list
                .Select(StringExt.ToSentenceCase) // Pure (Method Group)
                .Select(PrependCounter) // Impure as mutating global state
                .ToList();
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
            var output =  ListFormatter3.Format(input);
            Assert.AreEqual("1. Coffee beans", output[0]);
        }

        [Test]
        public void ItWorksOnLongerList()
        {
            var input = new List<string> { "coffee beans", "BANANAS" };
            var output = ListFormatter3.Format(input);
            Assert.AreEqual("1. Coffee beans", output[0]);
            Assert.AreEqual("2. Bananas", output[1]);
        }

        [Test]
        public void ItWorksOnAVeryLongList()
        {
            var size = 100000;
            var input = Range(1, size).Select(i => $"item{i}").ToList();
            var output = ListFormatter3.Format(input);
            Assert.AreEqual("100000. Item100000", output[size - 1]);
        }
    }
}
