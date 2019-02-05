using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using LaYumba.Functional;
using Unit = System.ValueTuple; // empty tuple can only have 1 possible value,  so it's as good as no value

namespace AWebApplication2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookTransferController : Controller
    {
        // Testing Validation Errors (not Exceptions)
        // curl -i http://localhost:55064/api/booktransfer 
        // Returns a 200 
        [HttpGet]
        public ActionResult<IEnumerable<string>> Index()
        {
            var a = new InvalidBicError();

            // Shortcut for above using static Errors factory (normal way to make an Error)
            var b = Errors.InvalidBic;

            // A generic error (Unusual)
            var c = new Error("test error");
            // shortcut for above
            var d = D.Error("test error");
            // can't do this as the ctor is Protected
            //var e = new Error();

            // Generic error  (Unusual)
            // implicitly converting string into an Error
            Error i = "test error";

            return new[] { "dave", "bob", a.ToString(), i.ToString() };
        }

        // curl -i http://localhost:55064/api/booktransfer/test 
        // Returns a 400 Bad Request
        [HttpGet, Route("test")]
        public IActionResult Test()
        {
            //return Ok();
            return BadRequest();
        }

        public class ThingDto { public string Name { get; set; } }


        // curl -i --header "Content-Type: application/json" -d "{\"Name\":\"Dave\"}" http://localhost:55064/api/booktransfer/test2
        // Using a string here didn't work - had to encapsulate in this ThingDto.
        // Returns a 200
        [HttpPost, Route("test2")]
        public IActionResult Test2([FromBody] ThingDto thing)
        {
            return Ok($"received: {thing.Name}");
        }


        // curl -i --header "Content-Type: application/json" -d "{\"Bic\":\"ABCDEFGHIJL\", \"Date\":\"2019-03-03\"}" http://localhost:55064/api/booktransfer/restful
        // Client explicitly requests a transfer to be carried out on some future date
        // client sends to the API a BookTransfer request in json via POST
        [HttpPost, Route("restful")]
        public IActionResult BookTransfer_v1([FromBody] BookTransfer request)
            => Handle(request)
                // Translate elevate types down to normal (leave the abstraction)
                // and handle
                // This is an outer layer adapter from the core (where we use the abstraction) 
                // to the outside world
                .Match<IActionResult>(
                Right: _ => Ok(),
                Left: error => BadRequest(error)); // BadRequest from MVC framework takes an error object to return

        // curl -i --header "Content-Type: application/json" -d "{\"Bic\":\"ABCDEFGHIJL\", \"Date\":\"2019-03-03\"}" http://localhost:55064/api/booktransfer/resultDto
        [HttpPost, Route("resultDto")]
        public ResultDto<ValueTuple> BookTransfer_v2([FromBody] BookTransfer request)
            => Handle(request).ToResult();

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
            // if request.Bic is null this throws a null ref exception!
            // which is Exceptional and not normal control flow

            // not pure as relying on external field (bicRegex)
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

        Either<Error, Unit> Save(BookTransfer request)
        {
            //throw new NotImplementedException();
            //return Errors.SaveProblem;
            // Pretend it has been implemented and has been successful
            return F.Unit();
        }
    }

    public class ResultDto<T>
    {
        public bool Succeeded { get; }
        public bool Failed => !Succeeded;

        public T Data { get; }
        public Error Error { get; }

        internal ResultDto(T data) { Succeeded = true; Data = data; }
        internal ResultDto(Error error) { Error = error; }
    }

    public static class EitherExt
    {
        // translate Either<Error, T> to a ResultDto for easy serialisation and access on client side
        // @this is allowing us to declare a reserved keyword as a variable
        public static ResultDto<T> ToResult<T>(this Either<Error, T> @this)
            => @this.Match(
                Right: data => new ResultDto<T>(data),
                Left: error => new ResultDto<T>(error));
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

        public static SaveProblemError SaveProblem
                  => new SaveProblemError();

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

    public sealed class SaveProblemError : Error
    {
        public override string Message { get; }
            = "Save problem";
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


}