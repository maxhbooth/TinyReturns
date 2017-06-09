using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dimensional.TinyReturns.Core.SharedContext.Services.TinyReturnsDatabase.Portfolio
{
    public interface ICountriesDataTableGateway
    {
        void Insert(CountryDto[] dtos);

        CountryDto[] GetAll();
    }
}
