using System.Security.Permissions;

using Allocate.Common.Database.Repositories;
using Allocate.Common.Database.Services;
using Allocate.Web.DemoApi.DataModels;

using Microsoft.AspNetCore.Authorization;
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
}