using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Dimensional.TinyReturns.Core;

namespace Dimensional.TinyReturns.Web.Controllers
{
    public class FirstFullMonthController : ApiController
    {

        // GET api/values
        public IHttpActionResult Get()
        {
            var result = MasterFactory.GetPublicWebReportFacade().GetPortfolioPerformance().Select(x => x.FirstFullMonth);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        // GET api/values/5
        public IHttpActionResult Get(int id)
        {
            var result = MasterFactory.GetPublicWebReportFacade().GetPortfolioPerformance()
                .FirstOrDefault(p => p.Number == id);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result.FirstFullMonth);
        }

        // POST api/values
        public void Post([FromBody]string value)
        {
        }

    }
}
