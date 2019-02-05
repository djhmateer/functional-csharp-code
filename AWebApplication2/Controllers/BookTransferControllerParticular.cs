using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using LaYumba.Functional;
using Microsoft.Extensions.Logging;
using Unit = System.ValueTuple; // empty tuple can only have 1 possible value,  so it's as good as no value
using static LaYumba.Functional.F;

namespace AWebApplication2.ControllersP
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookTransferPController: Controller
    {
        ILogger<BookTransferPController> logger;

        [HttpPost, Route("part")]
        public IActionResult MakeFutureTransfer([FromBody] BookTransfer request)
            => Handle(request).Match( // Unwraps value inside Validation
                Invalid: BadRequest, // If validation failed should send back a 400
                Valid: result => result.Match( // Unwraps value inside Exceptional
                    Exception: OnFaulted, // If persistence failed send a 500
                    Success: _ => Ok()));

        // Catch Exceptions and log them
        // Send to client a 500 and a general Error code
        IActionResult OnFaulted(Exception ex)
        {
            logger.LogError(ex.Message);
            return StatusCode(500, Errors.UnexpectedError);
        }

        Validation<Exceptional<Unit>> Handle(BookTransfer request)
            => Validate(request)
                .Map(Save);

        Validation<BookTransfer> Validate(BookTransfer cmd)
            => ValidateBic(cmd).Bind(ValidateDate);


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
                return Errors.TransferDateIsPast;
            return cmd;
        }

        // persistence

        string connString;

        Exceptional<Unit> Save(BookTransfer transfer)
        {
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


    // below should be in other files
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

    public static class Errors
    {
        public static InsufficientBalanceError InsufficientBalance
           => new InsufficientBalanceError();

        public static InvalidBicError InvalidBic
           => new InvalidBicError();

        public static CannotActivateClosedAccountError CannotActivateClosedAccount
           => new CannotActivateClosedAccountError();

        public static TransferDateIsPastError TransferDateIsPast
           => new TransferDateIsPastError();

        public static AccountNotActiveError AccountNotActive
           => new AccountNotActiveError();

        public static UnexpectedError UnexpectedError
           => new UnexpectedError();

        public static Error UnknownAccountId(Guid id)
           => new UnknownAccountId(id);
    }

    public sealed class UnknownAccountId : Error
    {
        Guid Id { get; }
        public UnknownAccountId(Guid id) { Id = id; }

        public override string Message
           => $"No account with id {Id} was found";
    }

    public sealed class UnexpectedError : Error
    {
        public override string Message { get; }
           = "An unexpected error has occurred";
    }

    public sealed class AccountNotActiveError : Error
    {
        public override string Message { get; }
           = "The account is not active; the requested operation cannot be completed";
    }

    public sealed class InvalidBicError : Error
    {
        public override string Message { get; }
           = "The beneficiary's BIC/SWIFT code is invalid";
    }

    public sealed class InsufficientBalanceError : Error
    {
        public override string Message { get; }
           = "Insufficient funds to fulfil the requested operation";
    }

    public sealed class CannotActivateClosedAccountError : Error
    {
        public override string Message { get; }
           = "Cannot activate an account that has been closed";
    }

    public sealed class TransferDateIsPastError : Error
    {
        public override string Message { get; }
           = "Transfer date cannot be in the past";
    }
}