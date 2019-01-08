using System;
using System.Collections.Generic;
using System.Linq;
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
}
