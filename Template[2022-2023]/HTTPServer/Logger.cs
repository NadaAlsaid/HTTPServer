using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.InteropServices.ComTypes;

namespace HTTPServer
{
    class Logger
    {
        static StreamWriter sr = new StreamWriter("log.txt");
        public static void LogException(Exception ex)
        {
            // TODO: Create log file named log.txt to log exception details in it
            //Datetime:
            //message:
            // for each exception write its details associated with datetime 
            sr.WriteLine("Log at:" + DateTime.Now.ToString()  );
            sr.WriteLine("Ended with: " + ex.Message.ToString() );
            sr.AutoFlush= true;
            sr.Close();
        }
    }
}
