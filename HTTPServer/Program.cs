using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTTPServer
{
    class Program
    {
        // Dalia
        static string Project_Path = System.AppDomain.CurrentDomain.BaseDirectory;
        static void Main(string[] args)
        {
            // TODO: Call CreateRedirectionRulesFile() function to create the rules of redirection 
            CreateRedirectionRulesFile();
            //Start server
            // 1) Make server object on port 1000
            Server server = new Server(1000, "redirectionRules.txt");
            // 2) Start Server
            server.StartServer();
        }
        // Habiba
        static void CreateRedirectionRulesFile()
        {
            // TODO: Create file named redirectionRules.txt
            string fileName = "redirectionRules.txt";
            if (File.Exists(fileName))
                File.Delete(fileName);
            else
            {
                using (FileStream fs = File.Create(fileName))
                {
                    // each line in the file specify a redirection rule
                    // example: "aboutus.html,aboutus2.html"
                    // means that when making request to aboustus.html,, it redirects me to aboutus2
                    Byte[] redirectionRule1 = new UTF8Encoding(true).GetBytes("aboutus.html,aboutus2.html");
                    fs.Write(redirectionRule1, 0, redirectionRule1.Length);
                }
            }
        }
         
    }
}
