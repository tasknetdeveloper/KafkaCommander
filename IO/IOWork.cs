using Model;
namespace IO;

public class IOWork
{
    private List<ICommand> listICommand;
    public IOWork() {
        listICommand = new List<ICommand>();
        listICommand.Add(new getDrive());
        listICommand.Add(new getDir());
        listICommand.Add(new getFiles());
    }

    public string GetApplicationHelp() {
        var result = "HELP\r\n" + "-----------------\r\n"
                     + "# Kafka Commander\r\n";
        string[]r;
        listICommand.ForEach(x => {
            
            r = x.GetHelp();
            if (r != null)
            {
                r.ToList().ForEach(x => {
                    result += "\r\n" + x;
                });
                result += "\r\n\r\n";
            }
            
        });
        return result;
    }

    public KafkaMessage? Exec(KafkaMessage message, Command cmdType)
    {
        KafkaMessage? result = null;
        var r = listICommand.Where(x => x.cmdType == cmdType).FirstOrDefault();
        if (r != null)
        {
            result = new KafkaMessage();
            result = r.Exec(message, cmdType);
        }

        return result;
    }
}
