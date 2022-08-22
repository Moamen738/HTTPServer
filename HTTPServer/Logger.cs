using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;

namespace HTTPServer
{
    class Logger
    {
        static Semaphore semaphore = new Semaphore(1,1);
        // Esraa 
        public static void LogException(Exception ex)
        {
            // TODO: Create log file named log.txt to log exception details in it
            string location = System.Environment.CurrentDirectory + "\\log.txt";
           
            //Datetime:
            string dateTime = "Date: " + DateTime.Now.ToString() + "\r\n";
            //message:
            string message ="Message:" + ex.Message;

            // for each exception write its details associated with datetime 
            semaphore.WaitOne();
            using (StreamWriter sw = new StreamWriter(location, true))
            {
                sw.WriteLine(dateTime);
                sw.WriteLine(message);
                sw.Flush();
            }
            semaphore.Release();
        }
    }
}
