using Model;
namespace IO;

internal class getDir : CommandUtils, ICommand
{

    #region ICommand
    public Command cmdType { get; set; } = Command.getDir;

    public string[] GetHelp()
    {
        string[] result = new string[] {"## " + this.GetType().Name  + "\r\n", this.GetType().Name + " path" + " (help: this command get all Dir names and file names in current dir) Example: getDir c:\\tmp",
                                         "call help: " + this.GetType().Name + " ?"};
        return result;
    }


    public KafkaMessage? Exec(KafkaMessage message, Command cmdType)
    {
        KafkaMessage? result = null;
        if (this.GetType().Name == cmdType.ToString())
        {
            var r = GetRequest(message.request, cmdType);
            if (r != null)
            {
                object? r0 = null;
                Enum.TryParse(typeof(Command), r[0], out r0);
                if (r0 != null)
                {
                    if (r.Length == 2 && r[1] != "?")
                    {
                        var a0 = utils.GetDirs(r[1]);
                        var a1 = utils.GetFilesOfDir2(r[1]);
                        message.response = a0.Union(a1).ToArray();                        
                    }                    
                    else if (r.Length == 2 && r[1] == "?")
                    {
                        message.response = new string[] { "help - get list of path" };
                    }
                    
                }
            }
        }
        result = message;
        return result;
    }
    #endregion
}

