using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Dimensional.TinyReturns.Core;
using Dimensional.TinyReturns.Core.PortfolioReportingContext.Services.PublicWebReport;
using Newtonsoft.Json;
using System.Data;

namespace Dimensional.TinyReturns.Web.Controllers
{
    public class PortfolioDetailsController : ApiController
    {
        private readonly PublicWebReportFacade _publicWebReportFacade;

        public PortfolioDetailsController()
        {
            _publicWebReportFacade =  MasterFactory.GetPublicWebReportFacade();
        }

        public PortfolioDetailsController(PublicWebReportFacade publicWebReportFacade)
        {
            _publicWebReportFacade = publicWebReportFacade;
        }

        public object Get(string attribute="")
        {
            object result;

            if (attribute == "")
            {
                return BadRequest("incorrect attribute");
            }
            else if (attribute.ToLower() == "firstfullmonth")
            {
                result = _publicWebReportFacade.GetPortfolioPerformance().Select(x => new FirstFullMonthModel()
                {
                    id = x.Number,
                    FirstFullMonth = x.FirstFullMonth
                });
            }
            else if (attribute.ToLower() == "onemonth")
            {
                result = _publicWebReportFacade.GetPortfolioPerformance().Select(x => new OneMonthModel()
                {
                    id = x.Number,
                    OneMonth = x.OneMonth
                });
            }
            else if (attribute.ToLower() == "sixmonth")
            {
                result = _publicWebReportFacade.GetPortfolioPerformance().Select(x => new SixMonthModel()
                {
                    id = x.Number,
                   SixMonth= x.SixMonth
                });
            }
            else if (attribute.ToLower() == "growthofwealth")
            {
                return BadRequest("incorrect attribute");
            }
            else
            {
                return BadRequest("incorrect attribute");
            }


            return result;
        }

        // GET api/values/5
        public IHttpActionResult Get( int id, string attribute = null)
        {
            object result;

            if (attribute == null)
            {
                return BadRequest("incorrect attribute");
            }
            else if (attribute.ToLower() == "firstfullmonth")
            {
                result = _publicWebReportFacade.GetPortfolioPerformance().FirstOrDefault(p => p.Number == id);
            }
            else if (attribute.ToLower() == "onemonth")
            {
                result = _publicWebReportFacade.GetPortfolioPerformance().FirstOrDefault(p => p.Number == id);

            }
            else if (attribute.ToLower() == "sixmonth")
            {
                result = _publicWebReportFacade.GetPortfolioPerformance().FirstOrDefault(p => p.Number == id);

            }
            else if (attribute.ToLower() == "growthofwealth")
            {
                return BadRequest("incorrect attribute");
            }
            else
            {
                return BadRequest("incorrect attribute");
            }


            return Ok(result);
        }

        // POST api/values
        public void Post([FromBody]string value)
        {
        }

    }

    public class FirstFullMonthModel
    {
        public decimal? FirstFullMonth;
        public int id;

    }

    public class OneMonthModel
    {
        public decimal? OneMonth;
        public int id;

    }

    public class SixMonthModel
    {
        public decimal? SixMonth;
        public int id;

    }


}
