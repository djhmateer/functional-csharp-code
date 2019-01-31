using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using LaYumba.Functional;
using static LaYumba.Functional.F;

namespace AWebApplication2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookTransferController : Controller
    {
        //[HttpGet]
        //public ActionResult<IEnumerable<string>> Index()
        //{
        //    return new[] { "dave", "bob" };
        //}

        // Client explicitly requests a transfer to be carried out on some future date
        [HttpPost, Route("api/Chapters6/transfers/future/restful")]
        public IActionResult BookTransfer_v1([FromBody] BookTransfer request)
            => Handle(request).Match<IActionResult>(
                Right: _ => Ok(),
                Left: BadRequest);

        // If success return nothing
        // Error representation
        Either<Error, ValueTuple> Handle(BookTransfer request)
            => Right(request)
                .Bind(ValidateBic)
                .Bind(ValidateDate)
                .Bind(Save);

        Regex bicRegex = new Regex("[A-Z]{11}");
        Either<Error, BookTransfer> ValidateBic(BookTransfer request)
        {
            if (!bicRegex.IsMatch(request.Bic))
                return Errors.InvalidBic;
            else return request;
        }

        DateTime now;
        Either<Error, BookTransfer> ValidateDate(BookTransfer request)
        {
            if (request.Date.Date <= now.Date)
                return Errors.TransferDateIsPast;
            else return request;
        }

        Either<Error, ValueTuple> Save(BookTransfer request)
        { throw new NotImplementedException(); }
    }

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