namespace WeChatSample.Models
{
    public class Message
    {
        public string MessageId { get; set; }

        public string ToUserName { get; set; }

        public string FromUserName { get; set; }

        public long CreateTime { get; set; }
    }
}
