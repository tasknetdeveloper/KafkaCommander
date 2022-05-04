using Model;
namespace IO;

internal class getDrive: CommandUtils, ICommand
{

    #region ICommand
    public Command cmdType { get; set; } = Command.getDrive;
    public string[] GetHelp() {
        string[] result = new string[] {"## " + this.GetType().Name + "\r\n", this.GetType().Name + " (this command get all drive names)",
                                         "call help: " + this.GetType().Name + " ?"};
        return result;
    }
    public KafkaMessage? Exec(KafkaMessage message, Command cmdType)
    {
        KafkaMessage? result = null;
        if (this.GetType().Name == cmdType.ToString())
        {
            var r = GetRequest(message.request, cmdType);
            if (r!=null)
            {
                object? r0 = null;
                Enum.TryParse(typeof(Command), r[0],out r0);
                if (r0 != null)
                { 
                    if ( r.Length == 1)
                        message.response = utils.GetDrivers();
                    if (r.Length == 2 && r[1] == "?")
                    {
                        message.response = new string[]{ "help - get all disk" };
                    }
                }
                    
            }
        }
        result = message;
        return result;
    } 
    #endregion
}
