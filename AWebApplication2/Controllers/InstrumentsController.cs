using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LaYumba.Functional;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AWebApplication2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InstrumentsController : ControllerBase
    {
        private IInstrumentService instruments;

        [HttpGet, Route("api/instruments/{ticker}/details")]
        public IActionResult GetAccountDetails(string ticker)
            => instruments.GetInstrumentDetails(ticker).Match<IActionResult>(
                Some: Ok,
                None: NotFound);
    }

    public interface IInstrumentService
    {
        Option<InstrumentDetails> GetInstrumentDetails(string ticker);
    }

    public class InstrumentDetails { }
}