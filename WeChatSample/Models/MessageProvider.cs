using System.IO;
using System.Xml.Linq;

namespace WeChatSample.Models
{
    public static class MessageProvider
    {
        public static Message FromGeneralMessage(Stream stream)
        {
            var document = XDocument.Load(new StreamReader(stream));

            if (document.Root.Element("MsgType").Value == "text")
            {
                return new TextMessage()
                {
                    MessageId = document.Root.Element("MsgId").Value,
                    ToUserName = document.Root.Element("ToUserName").Value,
                    FromUserName = document.Root.Element("FromUserName").Value,
                    CreateTime = long.Parse(document.Root.Element("CreateTime").Value),
                    Content = document.Root.Element("Content").Value
                };
            }

            return null;
        }
    }
}
