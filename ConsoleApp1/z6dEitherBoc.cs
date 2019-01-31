using System;
using LaYumba.Functional;
using static System.Math;

namespace ConsoleApp1.Chapter6.D
{
    using static F;

    static class Thing
    {
        internal static void Run()
        {
            Console.WriteLine("6d");
        }
    }

    public class Chapter6_BookTransferController : Controller
    {
        [HttpPost, Route("api/Chapters6/transfers/future/restful")]
        public IActionResult BookTransfer_v1([FromBody] BookTransfer request)
            => Handle(request).Match<IActionResult>(
                Right: _ => Ok(),
                Left: BadRequest);

        [HttpPost, Route("api/Chapters6/transfers/future/resultDto")]
        public ResultDto<ValueTuple> BookTransfer_v2([FromBody] BookTransfer request)
            => Handle(request).ToResult();

        Either<LaYumba.Functional.Error, ValueTuple> Handle(BookTransfer request) { throw new NotImplementedException(); }
    }

    // Errors which are 'business as usual' not Exceptional
    // he has own implmentation in library
    public class Error
    {
        // Virtual means a child class can override
        public virtual string Message { get; }
    }

    // Create 1 subclass for each Error type
    public sealed class InvalidBic : Error
    {
        public override string Message { get; }
            = "The beneficiary's BIC/SWIFT code is invalid";
    }

    public sealed class TransferDateIsPast : Error
    {
        public override string Message { get; }
            = "Transfer date cannot be in the past";
    }

    // Static factory functions for creating specific subclasses of Error
    // useful trick for keeping business code cleaner
    // and good documentation
    public static class Errors
    {
        public static InvalidBic InvalidBic
            => new InvalidBic();

        public static TransferDateIsPast TransferDateIsPast
            => new TransferDateIsPast();
    }
}
