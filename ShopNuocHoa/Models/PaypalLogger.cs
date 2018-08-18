using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopNuocHoa.Models
{
    public class PaypalLogger
    {
        public static string LogDirectoryPath = Environment.CurrentDirectory;
        public static void Log(String lines)
        {
            // Write the string to a file.append mode is enabled so that the log
            // lines get appended to test.txt than wiping content and writing the log
            try
            {
                System.IO.StreamWriter file = new
                System.IO.StreamWriter(LogDirectoryPath + "\\Error.log", true);
                file.WriteLine(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " --> " +
                lines);
                file.Close();
            }
            catch
            {
            }
        }
    }
}