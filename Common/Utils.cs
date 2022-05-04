using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.Diagnostics;
using Logger;
namespace Common
{
    public class Utils : IDisposable
    {
        private Log _log = new Log("Utils");

        public void Dispose() { }

        public Utils(string logFolder)
        {
            
        }

        public (string from, string to, string key) GetStrPair(string s, char separator)
        { 
            if(string.IsNullOrEmpty(s)) return ("","","");
            var arr=s.Split(separator);
            if(arr!=null && arr.Length>2)
                return (arr[0],arr[1],arr[2]);
            return ("", "", "");
        }

        

        public string[] GetDrivers()
        {
            string[] result = new string[12];
            var i = 0;
            foreach (var drive in DriveInfo.GetDrives())
            {
                result[i++]= drive.Name;
            }
            return result;
        }

        public string[]? GetDirs(string path)
        {
            return (!string.IsNullOrEmpty(path)) ? Directory.GetDirectories(path) : null;            
        }

        public string? GetCurrentFolderOfApplication()
        {
            var result = "";
            string path = System.Reflection.Assembly.GetExecutingAssembly().Location;
            result = System.IO.Path.GetDirectoryName(path);
            return result;
        }

        public bool isApplicationStartedAlready(string name)
        {
            var result = false;
            if (!string.IsNullOrEmpty(name))
            {
                var p = Process.GetProcessesByName(name).ToList();
                if (p != null && p.Count > 0)
                {
                    result = true;
                }
            }
            return result;
        }

        public bool isProgramStartAlready()
        {
            var result = false;
            var thisprocessname = Process.GetCurrentProcess().ProcessName;

            if (Process.GetProcesses().Count(p => p.ProcessName == thisprocessname) > 1)
                result = true;

            return result;
        }

       


        #region Path
        public int GetNumberInFileName_mypad(string path)
        {
            var result = -1;
            if (!string.IsNullOrEmpty(path))
            {
                var s = Path.GetFileName(path);
                var ext = Path.GetExtension(path);
                if (!string.IsNullOrEmpty(s))
                {
                    s = s.Replace(ext, "");
                    int.TryParse(s, out result);
                }
            }
            return result;
        }



        public string? GetDiskByPath(string path)
        {
            string result = "";
            if (!string.IsNullOrEmpty(path))
            {
                result = Path.GetPathRoot(path);
            }
            return result;
        }

        public string[]? GetFilesOfDir(string pathDir)
        {
            string[]? result = null;
            if (Directory.Exists(pathDir))
            {
                result = Directory.GetFiles(pathDir);
            }
            return result;
        }
        public string[] GetFilesOfDir2(string pathDir)
        {
            return Directory.GetFiles(pathDir); 
        }

        public FileSystemInfo[]? GetFilesOfDirOrderByDate(string pathDir)
        {
            FileSystemInfo[]? result = null;
            if (string.IsNullOrEmpty(pathDir)) return null;
            var di = new DirectoryInfo(pathDir);
            if (di.Exists)
            {
                FileSystemInfo[] files = di.GetFileSystemInfos();
                result = files.OrderBy(f => f.CreationTime).ToArray();
            }
            return result;
        }

        public bool CreateFolder(string path)
        {
            var result = false;
            try
            {
                if (!string.IsNullOrEmpty(path) && !Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                result = true;
            }
            catch (Exception) { }

            return result;
        }

        public string GetPathCombine(string[] items)
        {
            if (items==null) return "";
            return Path.Combine(items);
        }

        public string GetPathCombine(string filename, string folder = "")
        {
            if (string.IsNullOrEmpty(filename)) return "";

            if (!string.IsNullOrEmpty(folder))
                return Path.Combine(folder, filename);
            else
                return filename;
        }
        #endregion

      

      

        #region xml

        public XmlDocument ToXmlDocument(XDocument xDocument)
        {
            var xmlDocument = new XmlDocument();
            using (var xmlReader = xDocument.CreateReader())
            {
                xmlDocument.Load(xmlReader);
            }
            return xmlDocument;
        }

        public XDocument ToXDocument(XmlDocument xmlDocument)
        {
            using (var nodeReader = new XmlNodeReader(xmlDocument))
            {
                nodeReader.MoveToContent();
                return XDocument.Load(nodeReader);
            }
        }

        public string XmlNodeToString(System.Xml.XmlNode node, int? indentation = null, bool isIndented = false)
        {
            using (var sw = new System.IO.StringWriter())
            {
                using (var xw = new System.Xml.XmlTextWriter(sw))
                {
                    xw.Formatting = (isIndented) ? System.Xml.Formatting.Indented : System.Xml.Formatting.None;
                    if (indentation != null) xw.Indentation = (int)indentation;
                    node.WriteContentTo(xw);
                }
                return sw.ToString();
            }
        }


        public static XmlElement? Obj2Xml(object obj, string Namespace = "")
        {
            var result = new XmlDocument();
            if (obj == null) return null;
            XmlSerializer sr = new XmlSerializer(obj.GetType());
            StringBuilder sb = new StringBuilder();
            StringWriter w = new StringWriter(sb, System.Globalization.CultureInfo.InvariantCulture);
            sr.Serialize(w, obj, new XmlSerializerNamespaces(
                   new XmlQualifiedName[] { new XmlQualifiedName(string.Empty) }));
            var s = sb.ToString();
            result.LoadXml(s);
            if (!string.IsNullOrEmpty(Namespace) && result.DocumentElement!=null)
            {
                result.DocumentElement.SetAttribute("xmlns:xsd", Namespace);
                return result.DocumentElement;
            }
            return null;
        }

        public XmlDocument Obj2Xml(object obj)
        {
            var result = new XmlDocument();
            if (obj == null) return null;
            XmlSerializer sr = new XmlSerializer(obj.GetType());
            StringBuilder sb = new StringBuilder();
            StringWriter w = new StringWriter(sb, System.Globalization.CultureInfo.InvariantCulture);
            sr.Serialize(w, obj, new XmlSerializerNamespaces(
                   new XmlQualifiedName[] { new XmlQualifiedName(string.Empty) }));
            var s = sb.ToString();
            result.LoadXml(s);
            return result;
        }

        public XmlDocument ChangeXmlEncoding(XmlDocument xmlDoc, string newEncoding)
        {
            if (xmlDoc.FirstChild.NodeType == XmlNodeType.XmlDeclaration)
            {
                XmlDeclaration xmlDeclaration = (XmlDeclaration)xmlDoc.FirstChild;
                xmlDeclaration.Encoding = newEncoding;
            }
            return xmlDoc;
        }
      
        #endregion

        public string GetFileNameWithoutExtension(string path)
        {
            var result = "";

            if (!string.IsNullOrEmpty(path))
                result = Path.GetFileNameWithoutExtension(path);
            return result;
        }

        public string GetExtension(string path)
        {
            var result = "";

            if (!string.IsNullOrEmpty(path))
                result = Path.GetExtension(path);
            return result;
        }

        public bool isFolderExist(string spath)
        {
            var dir = new DirectoryInfo(spath);
            return dir.Exists;
        }

        public string ByteArrayToString(byte[] array)
        {
            var result = "";
            if (array != null && array.Length > 0)
            {
                //result = System.Text.Encoding.Default.GetString(array);
                result = System.Text.Encoding.UTF8.GetString(array);
            }
            return result;
        }



        #region #0
        public string GetFileInBase64(string fileName)
        {
            var result = "";
            if (string.IsNullOrEmpty(fileName)) return result;

            var bytes = File.ReadAllBytes(fileName);
            result = Convert.ToBase64String(bytes);
            return result;
        }

       

        public byte[] GetFileInByteFromBase64(String base64)
        {
            byte[] result = null;
            if (string.IsNullOrEmpty(base64)) return result;

            result = Convert.FromBase64String(base64);
            return result;
        }

        public void OpenFileWithFromBase64(string base64,string filename)
        {
            byte[]? result = null;
            if (string.IsNullOrEmpty(base64)) return;
            if (string.IsNullOrEmpty(filename)) return;

            filename = Path.GetFileName(filename);
            
            result = GetFileInByteFromBase64(base64);
            Stream stream = new MemoryStream(result);
            var path = Path.Combine(Path.GetTempPath(),
                               Guid.NewGuid() + filename);
            File.WriteAllBytes(path, result);
            Process.Start(path);
        }


        public void SaveFileAsFromBase64(string base64, string filename)
        {
            byte[]? result = null;
            if (string.IsNullOrEmpty(base64)) return;
            
            if (!string.IsNullOrEmpty(filename))
            {
                result = GetFileInByteFromBase64(base64);
                Stream stream = new MemoryStream(result);
                File.WriteAllBytes(filename, result);
            }                        
        }

        public String GetBase64FromByteArray(Byte[] bytes)
        {
            String result = "";
            result = Convert.ToBase64String(bytes);
            return result;
        }
        #endregion

       
      
        public string GetUtf8String(string s)
        {
            var result = "";
            byte[] bytes = Encoding.Default.GetBytes(s);
            result = Encoding.UTF8.GetString(bytes);
            return result;
        }


        public Guid StringToGuid(string guid)
        {
            var result = Guid.Empty;
            if (!string.IsNullOrEmpty(guid))
            {
                Guid.TryParse(guid, out result);
            }

            return result;
        }
       

        #region Serialize-Deserialize
        public T Deserialize<T>(string xml)
        {
            try
            {
                var xs = new XmlSerializer(typeof(T));
                return (T)xs.Deserialize(new StringReader(xml));
            }
            catch (Exception exp)
            {
                var s = exp.Message;
                return default(T);
            }
        }

        public T Deserialize<T>(string xml, string encodingStyle)
        {
            var xs = new XmlSerializer(typeof(T));
            XmlReader xmlReader = XmlReader.Create(new StringReader(xml));
            return (T)xs.Deserialize(xmlReader, encodingStyle);
        }

        public XmlDocument Serialize<T>(T obj)
        {
            try
            {
                using (StringWriter stringWriter = new StringWriter(new StringBuilder()))
                {
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
                    xmlSerializer.Serialize(stringWriter, obj);
                    XmlDocument _xml = new XmlDocument();
                    if (!string.IsNullOrEmpty(stringWriter.ToString()))
                    { _xml.LoadXml(stringWriter.ToString()); }
                    return _xml;
                }
            }
            catch (Exception exp)
            {

            }
            return null;
        }

       
        #endregion

        #region IO

        public void SaveXml(XmlDocument xml, string path)
        {
            try
            {
                var fn = new FileInfo(path);
                if (fn.Exists)
                {
                    DeleteIO(path, true);
                }
                xml.Save(path);
            }
            catch (Exception exp)
            {
                _log.ErrorLog("SaveXml:" + exp.Message);
            }
        }

        public bool CreateAndWriteFile(string s, string filename)
        {
            var result = false;
            var fn = new FileInfo(filename);

            if (fn.Directory!=null &&  !fn.Directory.Exists)
            {
                if (!string.IsNullOrEmpty(fn.DirectoryName))
                {
                    Directory.CreateDirectory(fn.DirectoryName);
                }                
            }

            if (fn.Exists)
            {
                if (!DeleteIO(filename, true)) return false;                
            }

            try
            {
                var i = 0;
                while (i < 10)
                {
                    fn = new FileInfo(filename);
                    if (!fn.Exists)
                    {
                        using (var sw = new StreamWriter(filename, true))
                        {
                            sw.WriteLine(s);
                            sw.Flush();
                            sw.Close();
                            result = true;
                        }
                        break;
                    }
                    Thread.Sleep(100); i++;
                }
            }
            catch (Exception exp)
            {
                _log.ErrorLog("CreateAndWriteFile:" + exp.Message);
            }
            return result;
        }


        public bool DeleteIO(string path, bool isFile)
        {
            var result = false;
            try
            {
                var fn = new FileInfo(path);
                if (fn.Exists)
                {
                    result = true;
                    Directory.Delete(path);
                }
            }
            catch (Exception)
            {
            }
            return result;
        }

        public bool RenameIO(string pathSource, string pathTarget, bool isFile)
        {
            var result = false;
            try
            {
                var info = new FileInfo(pathSource);
                info.MoveTo(pathTarget);
            }
            catch (Exception exp)
            {
                _log.ErrorLog("RenameIO:" + exp.Message);
            }
            return result;
        }

        public bool isFile(string pathSource)
        {
            var result = false;
            try
            {
                var info = new FileInfo(pathSource);
                result = info.Exists;
            }
            catch (Exception exp)
            {
                _log.ErrorLog("isFile:" + exp.Message);
            }
            return result;
        }

        public string ReadFileToString(string fileName)
        {
            var result = "";
            if (File.Exists(fileName))
            {
                result = File.ReadAllText(fileName);
            }
            return result;
        }

        public byte[] ReadAllBytes(string fileName)
        {
            byte[] buffer = null;
            using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                buffer = new byte[fs.Length];
                fs.Read(buffer, 0, (int)fs.Length);
            }
            return buffer;
        }

        #endregion

        #region #2
        public byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        public Stream ToStream(string str)
        {
            var s = str;
            Stream StringStream = new MemoryStream();
            StringStream.Read(GetBytes(s), 0, str.Length);
            return StringStream;
        }

        public Encoding DetectEncoding(string fileName, out string contents)
        {
            // open the file with the stream-reader:
            using (StreamReader reader = new StreamReader(fileName, true))
            {
                // read the contents of the file into a string
                contents = reader.ReadToEnd();

                // return the encoding.
                return reader.CurrentEncoding;
            }
        }

        public string EncodeStringToBase64_ASCII(string s)
        {
            var result = "";
            if (!string.IsNullOrEmpty(s))
            {
                byte[] bytes = Encoding.ASCII.GetBytes(s); 
                result = Convert.ToBase64String(bytes);
            }
            return result;
        }

        public string EncodeStringToBase64(string s)
        {
            var result = "";
            if (!string.IsNullOrEmpty(s))
            {
                byte[] bytes = Encoding.UTF8.GetBytes(s); 
                result = Convert.ToBase64String(bytes);
            }
            return result;
        }

        public string DecodeStringFromBase64(string s)
        {
            var result = s;
            if (!string.IsNullOrEmpty(s))
            {
                try
                {
                    if (isBase64(s))
                    {
                        var bytes2 = Convert.FromBase64String(s);
                        result = BytesToStringConverted(bytes2);
                    }
                }
                catch (Exception exp)
                {
                    throw new Exception("DecodeStringFromBase64 Error:" + exp.Message);
                }
            }
            return result;
        }

        public byte[] DecodeStringFromBase64_GetBytes(string s)
        {
            byte[] result = null;
            if (!string.IsNullOrEmpty(s))
            {
                try
                {
                    if (isBase64(s))
                    {
                        result = Convert.FromBase64String(s);
                    }
                }
                catch (Exception exp)
                {
                    
                }
            }
            return result;
        }

        public bool isBase64(string base64String)
        {
            var result = true;
            if (string.IsNullOrEmpty(base64String) || base64String.Length % 4 != 0
                || base64String.Contains(" ") || base64String.Contains("\t") || base64String.Contains("\r") || base64String.Contains("\n"))
                result = false;

            return result;
        }

        private string BytesToStringConverted(byte[] bytes)
        {
            using (var stream = new MemoryStream(bytes))
            {
                using (var streamReader = new StreamReader(stream))
                {
                    return streamReader.ReadToEnd();
                }
            }
        }
        #endregion
    }
}
