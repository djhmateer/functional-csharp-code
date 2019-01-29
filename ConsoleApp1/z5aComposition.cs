using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using HtmlAgilityPack;
using LaYumba.Functional;

namespace ConsoleApp1.Chapter5.A
{
    using System.Collections.Generic;
    using static F;

    static class Thing
    {
        internal static void Run()
        {
            var joe = new Person { First = "Joe", Last = "Bloggs" };
            // method chaining
            // preferable way of achieving functional composition in C#
            var email = joe.AbbreviateName().AppendDomain(); // jobl@manning.com

            // composition in an elevated world
            Option<Person> p = Some(new Person {First = "Joe", Last = "Bloggs"});
            Option<string> a = p.Map(AbbreviateName).Map(AppendDomain);
        }

        static string AbbreviateName(this Person p) => 
            Abbreviate(p.First) + Abbreviate(p.Last);

        static string AppendDomain(this string localPart) =>
            $"{localPart}@manning.com";

        static string Abbreviate(string s) => 
            s.Substring(0, 2).ToLower();
    }

    class Person
    {
        public string First { get; set; }
        public string Last { get; set; }
    }
}
