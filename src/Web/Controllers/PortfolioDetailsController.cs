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
using WebGrease.Css.Extensions;

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

        public object Get(string attribute=null)
        {
            object result;

            if (attribute == null)
            {
                return BadRequest("incorrect attribute");
            }
            else if (attribute.ToLower() == "firstfullmonth")
            {
                result = _publicWebReportFacade.GetPortfolioPerformance().Select(x => (FirstFullMonthModel) x);
            }
            else if (attribute.ToLower() == "onemonth")
            {
                result = _publicWebReportFacade.GetPortfolioPerformance().Select(x => (OneMonthModel) x);
            }
            else if (attribute.ToLower() == "sixmonth")
            {
                result = _publicWebReportFacade.GetPortfolioPerformance().Select(x => (SixMonthModel) x);
            }
            else if (attribute.ToLower() == "growthofwealth")
            {
                return BadRequest("incorrect attribute");
            }
            else
            {
                return BadRequest("incorrect attribute");
            }

            if (result == null)
            {
                return BadRequest("null result");
            }

            return result;
        }

        // GET api/values/5
        public IHttpActionResult Get( int id, string attribute = null)
        {
            object result;

            if (string.IsNullOrEmpty(attribute))
            {
                result = _publicWebReportFacade.GetPortfolioPerformance().FirstOrDefault(p => p.Number == id);
            }
            else if (attribute.ToLower() == "firstfullmonth")
            {
                result = (FirstFullMonthModel) _publicWebReportFacade.GetPortfolioPerformance()
                    .FirstOrDefault(p => p.Number == id);

            }
            else if (attribute.ToLower() == "onemonth")
            {
                result = (OneMonthModel) _publicWebReportFacade.GetPortfolioPerformance()
                    .FirstOrDefault(p => p.Number == id);

            }
            else if (attribute.ToLower() == "sixmonth")
            {
                 result = (SixMonthModel) _publicWebReportFacade.GetPortfolioPerformance()
                    .FirstOrDefault(p => p.Number == id);

            }
            else if (attribute.ToLower() == "growthofwealth")
            {
                return BadRequest("incorrect attribute");
            }
            else
            {
                return BadRequest("incorrect attribute");
            }

            if (result == null)
            {
                return BadRequest("null result");
            }


            return Ok(result);
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        [HttpPatch]
        [HttpPut]
        public void Put([FromBody]PublicWebReportFacade.PortfolioModel portfolio)
        {

        }

    }

    public class FirstFullMonthModel
    {
        public decimal? FirstFullMonth;
        public int id;

        public static implicit operator FirstFullMonthModel(PublicWebReportFacade.PortfolioModel portfolio)
        {
            if (portfolio == null)
                return null;

            return new FirstFullMonthModel()
            {
                id = portfolio.Number,
                FirstFullMonth = portfolio.FirstFullMonth
            };
        }
    }

    public class OneMonthModel
    {
        public decimal? OneMonth;
        public int id;

        public static implicit operator OneMonthModel(PublicWebReportFacade.PortfolioModel portfolio)
        {
            if (portfolio == null)
                return null;

            return new OneMonthModel()
            {
                id = portfolio.Number,
                OneMonth = portfolio.OneMonth
            };
        }
    }

    public class SixMonthModel
    {
        public decimal? SixMonth;
        public int id;

        public static implicit operator SixMonthModel(PublicWebReportFacade.PortfolioModel portfolio)
        {
            if (portfolio == null)
                return null;

            return new SixMonthModel()
            {
                id = portfolio.Number,
                SixMonth = portfolio.SixMonth
            };
        }
    }

    public static class ModelHelper
    {

        public static OneMonthModel CastAsOneMonthModel(this PublicWebReportFacade.PortfolioModel portfolio)
        {
            if (portfolio == null)
                return null;

            return new OneMonthModel()
            {
                id = portfolio.Number,
                OneMonth = portfolio.OneMonth
            };
        }

        public static FirstFullMonthModel CastAsFirstFullMonthModel(this PublicWebReportFacade.PortfolioModel portfolio)
        {
            if (portfolio == null)
                return null;

            return new FirstFullMonthModel()
            {
                id = portfolio.Number,
                FirstFullMonth = portfolio.FirstFullMonth
            };
        }
    }
}


