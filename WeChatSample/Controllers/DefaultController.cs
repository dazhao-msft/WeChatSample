using System;
using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using WeChatSample.Models;

namespace WeChatSample.Controllers
{
    [Route("api/[controller]")]
    public class DefaultController : Controller
    {
        [HttpGet]
        public string Get()
        {
            //
            // TODO: Validate token: http://blog.csdn.net/JaHello/article/details/52423574
            //

            return Request.Query["echostr"];
        }

        [HttpPost]
        public string Post()
        {
            var requestMessage = MessageProvider.FromGeneralMessage(Request.Body) as TextMessage;

            const string Template = @"<xml>
<ToUserName><![CDATA[{0}]]></ToUserName>
<FromUserName><![CDATA[{1}]]></FromUserName>
<CreateTime>{2}</CreateTime>
<MsgType><![CDATA[text]]></MsgType>
<Content><![CDATA[{3}]]></Content>
</xml>";

            string responseMessage = string.Format(CultureInfo.CurrentCulture,
                                                   Template,
                                                   requestMessage.FromUserName,
                                                   requestMessage.ToUserName,
                                                   DateTimeOffset.UtcNow.Ticks,
                                                   "You just said: " + requestMessage.Content);

            return responseMessage;
        }
    }
}
