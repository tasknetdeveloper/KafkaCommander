
namespace Model
{
    public class Settings
    {
        public string urlKafka { get; set; } = "";
        public string userName { get; set; } = "";
        public bool isConsole { get; set; } = false;
        public string token { get; set; } = "";
        public string password { get; set; } = "";
        public List<destination> destination { get; set; } = new List<destination>();
    }

    public class destination { 
        public string name { get; set; } = "";
        public string token { get; set; } = "";
        public string password { get; set; } = "";
    }
}
