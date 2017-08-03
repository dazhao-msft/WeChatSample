using System.Threading;
using System.Threading.Tasks;

namespace WeChatSample.Authorization
{
    public interface IAccessTokenProvider
    {
        Task<string> GetAccessTokenAsync(CancellationToken cancellationToken);
    }
}
