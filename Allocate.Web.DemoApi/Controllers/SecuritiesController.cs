using Allocate.Common.Database.Repositories;
using Allocate.Web.DemoApi.DataModels;

using Microsoft.AspNetCore.Mvc;

namespace Allocate.Web.DemoApi.Controllers;

[ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
public class SecuritiesController : OurControllerBase
{
    private readonly IGenericRepository _genericRepository;

    public SecuritiesController(IGenericRepository genericRepository) : base()
    {
        _genericRepository = genericRepository;
    }

    [HttpGet]
    public Security GetSecurity(string tickerSymbol)
    {
        Security result = _genericRepository.GetByStringKey<Security>("TickerSymbol", tickerSymbol).First();
        return result;
    }

    [HttpGet("")]
    public IEnumerable<ValueOverTime> GetValueOverTimeForSecurity(string tickerSymbol)
    {
        Security result = _genericRepository.GetByStringKey<Security>("TickerSymbol", tickerSymbol).First();
        var results = _genericRepository.GetByOtherGuid<ValueOverTime>("SecurityId", result.Id);
        return results;
    }
}