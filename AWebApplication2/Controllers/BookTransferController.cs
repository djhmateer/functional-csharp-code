using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using LaYumba.Functional;
//using static LaYumba.Functional.F;
using Unit = System.ValueTuple; // empty tuple can only have 1 possible value,  so its as good as no value

namespace AWebApplication2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookTransferController : Controller
    {
        // Get to test Errors
        [HttpGet]
        public ActionResult<IEnumerable<string>> Index()
        {
            var a = new InvalidBicError();

            // shortcut for above using static Errors factory
            var b = Errors.InvalidBic;

            // A generic error
            var c = new Error("test error");
            // shortcut for above
            var d = D.Error("test error");
            // can't do this as the ctor is Protected
            //var e = new Error();

            // Generic error 
            // implicitly converting string into an Error
            Error i = "test error";

            return new string[] { "dave", "bob", a.ToString(), i.ToString() };
        }

        // Client explicitly requests a transfer to be carried out on some future date
        // client sends to the API a BookTransfer request in json via POST
        [HttpPost, Route("api/Chapters6/transfers/future/restful")]
        public IActionResult BookTransfer_v1([FromBody] BookTransfer request)
            => Handle(request)
            // Translate elevate types down to normal (leave the abstraction)
            // and handle
            // This is an outer layer adapter from the core (where we use the abstraction) 
            // to the outside world
                .Match<IActionResult>(
                Right: _ => Ok(),
                Left: BadRequest);

        // Handle defines the high level workflow
        // ie ValidateBic, ValidateDate, then Save
        Either<Error, Unit> Handle(BookTransfer request)
            // Using F here as I don't want the rest of this code referencing the F library implicitly
            => F.Right(request)
                .Bind(ValidateBic)
                .Bind(ValidateDate)
                .Bind(Save);

        // Each validation function is world crossing going from
        // 'normal' value to an 'elevated' value Either<Error, BookTransfer>
        // so we can combine these elevated values with Bind in Handle
        Regex bicRegex = new Regex("[A-Z]{11}");
        Either<Error, BookTransfer> ValidateBic(BookTransfer request)
        {
            // not pure as relying on external field
            if (!bicRegex.IsMatch(request.Bic))
                return Errors.InvalidBic;
            return request;
        }

        Either<Error, BookTransfer> ValidateDate(BookTransfer request)
        {
            // Date must be in the future
            // so this function is not pure as we have date
            if (request.Date.Date <= DateTime.Now.Date)
                return Errors.TransferDateIsPast;
            return request;
        }

        Either<Error, ValueTuple> Save(BookTransfer request)
        { throw new NotImplementedException(); }
    }

    //
    // Booktransfer below
    //
    public class BookTransfer : MakeTransfer { }

    public class MakeTransfer : Command
    {
        public Guid DebitedAccountId { get; set; }

        public string Beneficiary { get; set; }
        public string Iban { get; set; }
        public string Bic { get; set; }

        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public string Reference { get; set; }
    }

    // Can only be used as a base class to other classes 
    public abstract class Command
    {
        public DateTime Timestamp { get; set; }

        public T WithTimestamp<T>(DateTime timestamp)
            where T : Command
        {
            T result = (T)MemberwiseClone();
            result.Timestamp = timestamp;
            return result;
        }
    }


    //
    // Errors below
    //

    // Convenience. Factory functions for creating specific subclasses of Error
    // also a nice place to store all the errors for documentation
    public static class Errors
    {
        public static InvalidBicError InvalidBic
           => new InvalidBicError();

        public static TransferDateIsPastError TransferDateIsPast
           => new TransferDateIsPastError();
    }

    // Sealed so cannot be inherited
    // These errors are not exceptional and are business as usual, so that is
    // why we are using a custom type
    public sealed class InvalidBicError : Error
    {
        public override string Message { get; }
           = "The beneficiary's BIC/SWIFT code is invalid";
    }

    public sealed class TransferDateIsPastError : Error
    {
        public override string Message { get; }
            = "Transfer date cannot be in the past";
    }


    // This is usually in the functional library 
    // shown here for clarity
    // Partial as there are normally lots of these partial 'F' classes 
    public static partial class D
    {
        public static Error Error(string message) => new Error(message);
    }

    public class Error
    {
        // Can override this property in subclasses
        public virtual string Message { get; }
        // Easy to get the message out ie don't have to say .Message
        public override string ToString() => Message;
        // ctor - can only be called from a subclass ie can't instantiate
        protected Error() { }

        // So can can call Error("test error") if need to
        // Internal so only types within the currently assembly can instantiate
        internal Error(string Message) { this.Message = Message; }
        // a Conversion operator
        // implicit means conversions will occur automatically
        // converting from string to Error
        public static implicit operator Error(string m) => new Error(m);
    }



    //[HttpPost, Route("api/Chapters6/transfers/future/resultDto")]
    //public ResultDto<ValueTuple> BookTransfer_v2([FromBody] BookTransfer request)
    //    => Handle(request).ToResult();

    //public class ResultDto<T>
    //{
    //    public bool Succeeded { get; }
    //    public bool Failed => !Succeeded;

    //    public T Data { get; }
    //    public Error Error { get; }

    //    internal ResultDto(T data) { Succeeded = true; Data = data; }
    //    internal ResultDto(Error error) { Error = error; }
    //}

    //public static class EitherExt
    //{
    //    public static ResultDto<T> ToResult<T>(this Either<Error, T> @this)
    //        => @this.Match(
    //            Right: data => new ResultDto<T>(data),
    //            Left: error => new ResultDto<T>(error));
    //}


}