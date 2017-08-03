using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace WeChatSample.Authorization
{
    public class AccessTokenProvider : IAccessTokenProvider
    {
        private readonly string _appId;
        private readonly string _appSecret;
        private readonly SemaphoreSlim _semaphore;

        private string _accessToken;
        private long _expiresOn;

        public AccessTokenProvider(IConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            _appId = configuration["wechat: appid"];
            _appSecret = configuration["wechat: appsecret"];
            _semaphore = new SemaphoreSlim(1);
        }

        public async Task<string> GetAccessTokenAsync(CancellationToken cancellationToken)
        {
            string accessToken = GetCachedAccessToken();

            if (string.IsNullOrEmpty(accessToken))
            {
                await _semaphore.WaitAsync();

                try
                {
                    accessToken = GetCachedAccessToken();

                    if (string.IsNullOrEmpty(accessToken))
                    {
                        await GetAccessTokenCoreAsync(cancellationToken);

                        accessToken = GetCachedAccessToken();
                    }
                }
                finally
                {
                    _semaphore.Release();
                }
            }

            return accessToken;
        }

        private async Task GetAccessTokenCoreAsync(CancellationToken cancellationToken)
        {
            using (var client = new HttpClient())
            {
                //
                // https://mp.weixin.qq.com/wiki?t=resource/res_main&id=mp1421140183
                //

                string url = FormattableString.Invariant($"https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={_appId}&secret={_appSecret}");

                var response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();

                    var payload = JsonConvert.DeserializeObject<PayLoad>(content);

                    Volatile.Write(ref _accessToken, payload.AccessToken);
                    Volatile.Write(ref _expiresOn, (DateTimeOffset.UtcNow + TimeSpan.FromSeconds(payload.ExpiresIn)).Ticks);
                }
                else
                {
                    // TODO: Deal with error code and then retry if necessary.
                }
            }
        }

        private string GetCachedAccessToken()
        {
            var accessToken = Volatile.Read(ref _accessToken);
            var expiresOn = Volatile.Read(ref _expiresOn);

            return (expiresOn - DateTimeOffset.UtcNow.Ticks > 0) ? accessToken : null;
        }

        private class PayLoad
        {
            [JsonProperty(PropertyName = "access_token")]
            public string AccessToken { get; set; }

            [JsonProperty(PropertyName = "expires_in")]
            public int ExpiresIn { get; set; }
        }

        private class ErrorPayload
        {
            [JsonProperty(PropertyName = "errcode")]
            public int ErrorCode { get; set; }

            [JsonProperty(PropertyName = "errmsg")]
            public string ErrorMessage { get; set; }
        }
    }
}
