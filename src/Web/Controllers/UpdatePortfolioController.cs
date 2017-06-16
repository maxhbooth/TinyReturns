using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Dimensional.TinyReturns.Core.PortfolioReportingContext.Services.PublicWebReport;
using Newtonsoft.Json;

namespace Dimensional.TinyReturns.Web.Controllers
{
    public class UpdatePortfolioController : ApiController
    {



        // POST api/values
        public void Post([FromBody]string json)
        {

            UpdatePortfolioModel updatePortfolio = JsonConvert.DeserializeObject<UpdatePortfolioModel>(json);

        }


    }

    public class UpdatePortfolioModel
    {
        public string Name;
        public int Number;
        public PublicWebReportFacade.BenchmarkModel[] Benchmarks;
    }
}
