using System.Net.Mime;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Allocate.Web.DemoApi.Controllers;

/// <summary>
/// This is a template ControllerBase for an API's Controllers.
/// Every API Controller in a Web API project should extend this base class,
/// so that we can apply all the Attributes and Filters in just one place.
/// </summary>

[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
public abstract class OurControllerBase : ControllerBase
{

    protected OurControllerBase()
    {
    }
}