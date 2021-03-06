﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;

namespace ConsoleApp1.Chapter1.DB
{
    using static ConnectionHelper; // C#6 - import static members of this type
    //using static ConnectionHelper_V2; // C#6 - import static members of this type
    using static F; // Beginnings of functional library used in ConnectionHelper_V2

    public static class DBThing
    {
        public static void Run()
        {
            Console.WriteLine("Hello World!");
            //var logger = new DbLogger_V1();
            var logger = new DbLogger_V2();
            var id = logger.Log("test");
            Console.WriteLine($"just inserted id: {id}");
            var result = logger.GetLogs(7.Days().Ago()).ToList();
            result.ForEach(x => Console.WriteLine($"{x.ID} : {x.Timestamp} : {x.Message}"));
        }
    }

    // 2. Functional way of doing data access
    public class DbLogger_V2
    {
        string connString = "Server=(localdb)\\mssqllocaldb;Database=FP;Trusted_Connection=True;MultipleActiveResultSets=true";

        // DBLogger doesn't need to know about Creating, Opening or Disposing of the connection
        public int Log(string message) =>
            Connect(connString,
                c => c.Query<int>("Insert into Logs(message) values (@message); SELECT CAST(SCOPE_IDENTITY() as int)",
                        new {message})
                    .Single());

        public IEnumerable<LogMessage> GetLogs(DateTime since)
            => Connect(connString,
                c => c.Query<LogMessage>("SELECT * FROM [Logs] WHERE [Timestamp] > @since", new { since }));
    }

    // 1. Classic way of doing data access
    //public class DbLogger_V1
    //{
    //    string connString = "Server=(localdb)\\mssqllocaldb;Database=FP;Trusted_Connection=True;MultipleActiveResultSets=true";

    //    public int Log(string message)
    //    {
    //        using (var conn = new SqlConnection(connString))
    //        {
    //            conn.Open();
    //            var result = conn.Query<int>("Insert into Logs(message) values (@message); SELECT CAST(SCOPE_IDENTITY() as int)", new { message }).Single();
    //            return result;
    //        }
    //    }

    //    public IEnumerable<LogMessage> GetLogs(DateTime since)
    //    {
    //        var sqlGetLogs = "SELECT * FROM [Logs] WHERE [Timestamp] > @since";
    //        using (var conn = new SqlConnection(connString))
    //            return conn.Query<LogMessage>(sqlGetLogs, new { since });
    //    }
    //}

    public static class ConnectionHelper
    {
        // HOF - accepts function as an input
        // Generic method returning a generic object R
        // R is an IEnumerable<LogMessage> when calling GetLogs
        // R is int when calling Log
        // function accepts a SQLConnection, which will return an R
        public static R Connect<R>(string connString, Func<SqlConnection, R> func)
        {
            // using here is a statement (which doesn't return a value)
            using (var conn = new SqlConnection(connString))
            {
                conn.Open();
                return func(conn);
            }
        }
    }

    // Refactored into simpler functions
    public static class ConnectionHelper_V2
    {
        // Using here is an expression (expressions return values)
        // more compact expression body syntax now we are using Using
        public static R Connect<R>(string connString, Func<IDbConnection, R> func)
            => Using(new SqlConnection(connString), conn => { conn.Open(); return func(conn); });
    }

    // start of a functional library
    public static class F
    {
        // takes 2 arguments 
        // 1 - a disposable resource
        // 2 - a function to be executed before the resource is disposed
        public static R Using<TDisp, R>(TDisp disposable, Func<TDisp, R> f) where TDisp : IDisposable
        {
            using (disposable) return f(disposable);
        }
    }


    public class LogMessage
    {
        public int ID { get; set; }
        public DateTime Timestamp { get; set; }
        public string Message { get; set; }
    }

    public static class TimeSpanExt
    {
        public static TimeSpan Seconds(this int @this)
            => TimeSpan.FromSeconds(@this);

        public static TimeSpan Minutes(this int @this)
            => TimeSpan.FromMinutes(@this);

        public static TimeSpan Days(this int @this)
            => TimeSpan.FromDays(@this);

        public static DateTime Ago(this TimeSpan @this)
            => DateTime.UtcNow - @this;
    }
}
