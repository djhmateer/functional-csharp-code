using System;
using LaYumba.Functional;
using static System.Math;

namespace ConsoleApp1.Chapter6.D
{
    // see AWebApplication2 - BookTransferController
    using static F;

    static class Thing
    {
        static DateTime now = DateTime.Now;
        internal static void Run()
        {
            Console.WriteLine("6d");
            var a = now.Date;

        }
    }

    //// Errors which are 'business as usual' not Exceptional
    //// he has own implmentation in library
    //public class Error
    //{
    //    // Virtual means a child class can override
    //    public virtual string Message { get; }
    //}

    //// Create 1 subclass for each Error type
    //public sealed class InvalidBic : Error
    //{
    //    public override string Message { get; }
    //        = "The beneficiary's BIC/SWIFT code is invalid";
    //}

    //public sealed class TransferDateIsPast : Error
    //{
    //    public override string Message { get; }
    //        = "Transfer date cannot be in the past";
    //}

    //// Static factory functions for creating specific subclasses of Error
    //// useful trick for keeping business code cleaner
    //// and good documentation
    //public static class Errors
    //{
    //    public static InvalidBic InvalidBic
    //        => new InvalidBic();

    //    public static TransferDateIsPast TransferDateIsPast
    //        => new TransferDateIsPast();
    //}
}
