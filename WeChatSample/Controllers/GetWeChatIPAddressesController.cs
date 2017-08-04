using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WeChatSample.Authorization;

namespace WeChatSample.Controllers
{
    [Route("api/[controller]")]
    public class GetWeChatIPAddressesController : Controller
    {
        private readonly IAccessTokenProvider _accessTokenProvider;

        public GetWeChatIPAddressesController(IAccessTokenProvider accessTokenProvider)
        {
            _accessTokenProvider = accessTokenProvider;
        }

        [HttpGet]
        public async Task<string> Get()
        {
            string accessToken = await _accessTokenProvider.GetAccessTokenAsync(CancellationToken.None);

            using (var client = new HttpClient())
            {
                string url = FormattableString.Invariant($"https://api.weixin.qq.com/cgi-bin/getcallbackip?access_token={accessToken}");

                var response = await client.GetAsync(url);

                return await response.Content.ReadAsStringAsync();
            }
        }
    }
}
