
namespace Model;

public interface ICommand
{
    public Command cmdType { get; set; }
    KafkaMessage? Exec(KafkaMessage message, Command cmdType);
    string[] GetHelp();
}
