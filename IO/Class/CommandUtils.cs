using Common;
using Model;
namespace IO;

internal class CommandUtils
{
    protected Utils? utils = null;    
    internal CommandUtils()
    {
        utils = new Utils("CommandUtils");       
    }
    protected string[] GetRequest(string request, Command type)
    {
        string[] result = new string[10];
        if (string.IsNullOrEmpty(request)) return result;
        var arr = request.Split(' ');
        if (arr.Length > 0)
        {
            result = arr;
        }
        return result;
    }
}

