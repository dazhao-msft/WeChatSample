using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WeChatSample.Authorization;

namespace WeChatSample.Controllers
{
    [Route("api/[controller]")]
    public class TestController : Controller
    {
        private readonly IAccessTokenProvider _accessTokenProvider;

        public TestController(IAccessTokenProvider accessTokenProvider)
        {
            _accessTokenProvider = accessTokenProvider;
        }

        [HttpGet]
        public Task<string> Get()
        {
            return _accessTokenProvider.GetAccessTokenAsync(CancellationToken.None);
        }
    }
}
