using Model;
namespace IO;

internal class getFiles : CommandUtils, ICommand
{

    #region ICommand
    public Command cmdType { get; set; } = Command.getFiles;
    public string[] GetHelp() {
        return null;
    }
    public KafkaMessage? Exec(KafkaMessage message, Command cmdType)
    {
        KafkaMessage? result = null;
        if (this.GetType().Name == cmdType.ToString())
        {
            var arr = GetRequest(message.request, cmdType);
            if (arr != null)
            {
                message.response = utils.GetFilesOfDir2(arr[0]);
            }            
        }
        result = message;
        return result;
    }
    #endregion
}

