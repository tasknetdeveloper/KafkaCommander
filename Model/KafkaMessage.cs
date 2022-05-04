
namespace Model
{
    public class KafkaMessage
    {
        public KafkaType type { get; set; }
        public string from { get; set; }
        public string to { get; set; }
        public string token { get; set; }
        public Guid uid { get; set; }
        public string request { get; set; }
        public string[] response { get; set; }
    }
}
