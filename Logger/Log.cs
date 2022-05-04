
namespace Logger
{
    public class Log
    {
        private string PathToFile = "";

        private const string separator = "\r\n";

        public Log(string FileName)
        {
            var asm = System.Reflection.Assembly.GetExecutingAssembly();
            if (string.IsNullOrEmpty(asm.Location)) return;

            var serverFolder = Path.Combine(Path.GetDirectoryName(asm.Location), "log");

            var LogFolder = MakeLogDir(serverFolder);


            PathToFile = (!string.IsNullOrEmpty(LogFolder) ? LogFolder + "\\" : string.Empty) +
                FileName + "_date_" +
                DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + "_time_" +
                DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + ".txt";
        }

        /// <summary>
        /// Создать дирректорию log
        /// </summary>
        private string MakeLogDir(string LogFolder)
        {
            try
            {
                var d = new DirectoryInfo(LogFolder);
                if (!d.Exists)
                {
                    Directory.CreateDirectory(LogFolder);
                }
                return LogFolder;
            }
            catch (Exception exp)
            {
                return string.Empty;
            }
        }

      
        /// <summary>
        /// Запись ошибки в лог
        /// </summary>
        /// <param name="sPathName"></param>
        /// <param name="sErrMsg"></param>
        public void ErrorLog(string sErrMsg)
        {
            try
            {
                var sLogFormat = DateTime.Now.ToShortDateString().ToString() + " " + DateTime.Now.ToLongTimeString().ToString() + " ==>" + separator;
                using (StreamWriter sw = new StreamWriter(PathToFile, true))
                {
                    sw.WriteLine(sLogFormat + sErrMsg);
                    sw.Flush();
                    sw.Close();
                }
            }
            catch
            {

            }
        }
       
    }
}
