using System;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using LaYumba.Functional;
using Microsoft.Extensions.Logging;
using Unit = System.ValueTuple; 
using static LaYumba.Functional.F;

namespace AWebApplication2.ControllersP
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookTransferPController: Controller
    {
        ILogger<BookTransferPController> logger;

        // curl -i --header "Content-Type: application/json" -d "{\"Bic\":\"ABCDEFGHIJL\", \"Date\":\"2019-03-03\"}" http://localhost:55064/api/booktransferp/part
        [HttpPost, Route("part")]
        public IActionResult MakeFutureTransfer([FromBody] BookTransfer request)
            => Handle(request).Match( // Unwraps value inside Validation
                Invalid: BadRequest, // If validation failed should send back a 400 with Validation fail Message   
                Valid: result => result.Match( // Unwraps value inside Exceptional
                    Exception: OnFaulted, // If persistence failed send a 500 with a general error message
                    Success: _ => Ok()));

        // Catch Exceptions and log them
        // Send to client a 500 and a general error message
        IActionResult OnFaulted(Exception ex)
        {
            logger.LogError(ex.Message);
            return StatusCode(500, Errors.UnexpectedError);
        }

        // As 2 different return types, can't use bind, so map together to get nested types
        // combining the effect of validation (we may get validation errors instead of the desired return value)
        // with the effect of exception handling (even after validation passes, we may get an exception instead of the return value)
        // operation may fail for business reasons as well as technical reasons by stacking the 2 monadic effects
        Validation<Exceptional<Unit>> Handle(BookTransfer request)
            => Validate(request) // Validate returns a Validation<BookTransfer>
                .Map(Save); // Save returns an Exceptional

        Validation<BookTransfer> Validate(BookTransfer cmd)
            => ValidateBic(cmd)
                .Bind(ValidateDate);


        // bic code validation
        static readonly Regex regex = new Regex("^[A-Z]{6}[A-Z1-9]{5}$");
        Validation<BookTransfer> ValidateBic(BookTransfer cmd)
        {
            if (!regex.IsMatch(cmd.Bic.ToUpper()))
                return Errors.InvalidBic;
            return cmd;
        }

        // date validation
        DateTime now = DateTime.Now;
        Validation<BookTransfer> ValidateDate(BookTransfer cmd)
        {
            if (cmd.Date.Date <= now.Date)
                return Invalid(Errors.TransferDateIsPast); // could omit Invalid
            return Valid(cmd); // could omit Valid as implicit conversion is defined
        }

        // persistence
        string connString;
        Exceptional<Unit> Save(BookTransfer transfer)
        {
            // try/catch is as small as possible
            // immediately translate to functional style wrapping the result in an Exceptional
            try
            {
                // would end up with a 500 and generic error code to client
                // full error logged on server
                //throw new Exception("asdf");
                //ConnectionHelper.Connect(connString
                    //, c => c.Execute("INSERT ...", transfer));
            }
            catch (Exception ex) { return ex; }
            return Unit();
        }
    }


    // Perhaps called a Command from CQS
    // so Command should never return data
    public class BookTransfer : MakeTransfer { }

    public class MakeTransfer : Command
    {
        public string Bic { get; set; }

        public DateTime Date { get; set; }
    }

    // Can only be used as a base class to other classes 
    public abstract class Command { }

    public static class Errors
    {
        public static InvalidBicError InvalidBic
           => new InvalidBicError();

        public static TransferDateIsPastError TransferDateIsPast
           => new TransferDateIsPastError();

        public static UnexpectedError UnexpectedError
           => new UnexpectedError();
    }

    public sealed class UnexpectedError : Error
    {
        public override string Message { get; }
           = "An unexpected error has occurred";
    }

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
}