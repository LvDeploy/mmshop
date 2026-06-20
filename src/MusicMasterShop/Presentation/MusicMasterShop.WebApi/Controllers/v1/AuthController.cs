using Microsoft.AspNetCore.Mvc;
using MusicMasterShop.Application.Middleware.Correlation;
using MusicMasterShop.WebApi.Controllers.Base;

namespace MusicMasterShop.WebApi.Controllers.v1
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ApiControllerBase
    {
        public AuthController(CorrelationId correlationId) : base(correlationId)
        {
        }
    }
}
