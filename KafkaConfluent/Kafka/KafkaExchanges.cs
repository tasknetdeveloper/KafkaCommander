using System.Xml;
using Confluent.Kafka;
using IO;
using Model;
using Common;
using Logger;
namespace KafkaSpace
{
    public class KafkaExchanges
    {
        private Log? log = null;
        private IOWork io = new IOWork();
        private Utils utils = new Utils("");
        private CryptUtils? cuilts = null;
        private static Settings cfg = new Settings();
        private Acks acks = Acks.None;
        public KafkaExchanges(Settings cfg_) {
            cfg = cfg_;
            log = new Log("Exchanges");
            cuilts = new CryptUtils(0, 0, 0, 0, utils);
        }


        public void Producer()
        {
            if (cfg.isConsole)
            {
                Console.Write(">");
                while (true)
                {                    
                    var scommand = Console.ReadLine();
                    var p = ParseCommandLineInput(scommand);                  
                    
                    Producer(p.commandline,
                        cfg.userName + Static.PairSeparator +
                        ((!string.IsNullOrEmpty(p.username)) ? p.username
                                                             : cfg.destination[0].name
                        )
                        + Static.PairSeparator);
                    
                }
            }
        }
        public async void Producer(string scommand, string newKey)
        {
            if (string.IsNullOrEmpty(scommand)) return;
            var config = new ProducerConfig
            {                
                BootstrapServers = cfg.urlKafka,
                Acks = this.acks  
            };

            using (var p = new ProducerBuilder<string, string>(config).Build())
            {
                try
                {
                    var m = new Message<string, string>()
                    {
                        Key = newKey,
                        Value = cuilts.Encrypt2(
                            utils.GetUtf8String(" " + scommand), cfg.password)
                    };
                    var dr = await p.ProduceAsync("testtopic", m);
                }
                catch (ProduceException<Null, string> e)
                {
                    if(cfg.isConsole)
                        Console.WriteLine($"Delivery failed: {e.Error.Reason}");
                    log.ErrorLog(e.Error.Reason);
                }
            }
        }

        Func<string, destination> destinationFuncFull = (a) => {
            var r = cfg.destination.Where(x => x.name == a).FirstOrDefault();
            return r;
        };

        public void Consumer()
        {            
            var conf = new ConsumerConfig
            {
                ClientId=cfg.userName,
                GroupId = "test-consumer-group23",
                BootstrapServers = cfg.urlKafka,
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            using (var c = new ConsumerBuilder<string, string>(conf).Build())
            {
                c.Subscribe("testtopic");

                CancellationTokenSource cts = new CancellationTokenSource();
                Console.CancelKeyPress += (_, e) => {
                    e.Cancel = true; // prevent the process from terminating.
                    cts.Cancel();
                };
                XmlDocument? xml = null;
                KafkaMessage? response = null;
                KafkaMessage? resp = null;
                var g = Command.empty;
                var xx = "";
                try
                {
                    while (true)
                    {
                        try
                        {
                            var cr = c.Consume(cts.Token);
                            var t = utils.GetStrPair(cr.Key,Static.PairSeparator);

                            var f = destinationFuncFull(t.from);
                            if (t.from!="" && f!=null)
                            {
                                xx = cuilts.Decrypt2(cr.Value, f.password);

                                g = Command.empty;
                                response = null;

                                if (!string.IsNullOrEmpty(xx))
                                {
                                    xx = xx.TrimStart();
                                    g = Get_cmdType(xx);

                                    response = io.Exec(CreateResponse(xx, cfg.userName, t.from,
                                                                          KafkaType.get, Guid.NewGuid()), g);


                                    if (g != Command.empty && response != null)
                                    {
                                        xml = utils.Serialize<KafkaMessage>(response);
                                        if (xml != null)
                                        {
                                            Console.WriteLine("Sent: " + xml.OuterXml);
                                            Producer(xml.OuterXml, cfg.userName + Static.PairSeparator + t.from + Static.PairSeparator);
                                        }
                                    }
                                    else
                                    {
                                        resp = utils.Deserialize<KafkaMessage>(xx);

                                        if (resp != null && resp.response != null && resp.response.Length > 0)
                                        {
                                            if (resp.type != KafkaType.cancel)
                                                resp.response.Where(x => !string.IsNullOrEmpty(x)).ToList().ForEach(x =>
                                                {
                                                    Console.WriteLine($"{x}");
                                                });
                                        }

                                        if (cfg.isConsole)
                                        {
                                            Console.WriteLine("");
                                            Console.Write(">");
                                        }
                                    }
                                }

                                if (!cfg.isConsole && (string.IsNullOrEmpty(xx) || resp == null))
                                {
                                    response = CreateResponse("", cfg.userName, t.from, KafkaType.get, Guid.NewGuid());
                                    xml = utils.Serialize<KafkaMessage>(response);
                                    Producer(xml.OuterXml, cfg.userName + Static.PairSeparator + t.from + Static.PairSeparator);
                                }                               
                            }
                        }
                        catch (ConsumeException e)
                        {
                            if (cfg.isConsole)
                                Console.WriteLine($"Error occured: {e.Error.Reason}");
                            log.ErrorLog(e.Message);
                        }
                    }
                }
                catch (OperationCanceledException e)
                {
                    log.ErrorLog(e.Message);
                    c.Close();
                }
            }
        }  

        private Command Get_cmdType(string s)
        {
            Command cmd = Command.empty;
            if (String.IsNullOrEmpty(s)) return cmd;
            object obj = new object();
            s = s.Trim();
            var arr = s.Split(' ');
            foreach (var item in arr)
            {
                Enum.TryParse(typeof(Command), item, out obj);
                break;
            }            

            if (obj != null)
                cmd = (Command)obj;
            return cmd;
        }

        private KafkaMessage CreateResponse(string request, string from, string to, KafkaType type, Guid uid)
        {
            var result = new KafkaMessage
            {
                from = from,
                request = request,
                to = to,
                token = "",
                type = type,
                uid = uid,
            };
            return result;
        }

        private (string username, string commandline) ParseCommandLineInput(string s)
        {
            var username = "";
            var command = "";
            if(string.IsNullOrEmpty(s)) return (username, command);

            s = s.TrimStart();
            var arr = s.Split(' ');

            if (arr != null && arr.Length > 1)
            {
                var f = destinationFuncFull(arr[0]);
                if (f != null)
                {
                    username = arr[0];
                    command = s.Substring(username.Length).TrimStart();
                }
                else
                    command = s;                
            }
            else
                command = s;

            return (username, command);
        }

    }
}
