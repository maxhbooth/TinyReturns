using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Dimensional.TinyReturns.Web.Models
{
    public class ReturnsEditModel
    {
        public IEnumerable<SelectListItem> NetReturns { get; set; }
    }
}