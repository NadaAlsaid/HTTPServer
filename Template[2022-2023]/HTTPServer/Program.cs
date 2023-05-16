using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTTPServer
{
    class Program
    {
        static void Main(string[] args)
        {
            // TODO: Call CreateRedirectionRulesFile() function to create the rules of redirection 

            //Start server
            // 1) Make server object on port 1000
            // 2) Start Server

            CreateRedirectionRulesFile();
            Server server = new Server(1000, "redirectionRules.txt");
            server.StartServer();
        }

        static void CreateRedirectionRulesFile()
        {
            // TODO: Create file named redirectionRules.txt
            // each line in the file specify a redirection rule
            // example: "aboutus.html,aboutus2.html"
            // means that when making request to aboustus.html,, it redirects me to aboutus2
            string redirection_file_Name = "redirectionRules.txt";

            // Define the redirection rules : each redirectionrule is two parts :the source URL and the destination URL
            string[] redirectionRules = new string[]
            {
              "aboutus.html,aboutus2.html",
              "contactus.html,contactus2.html",
              "home.html,home2.html",

            };

            // Create the file
            using (StreamWriter sw = File.CreateText(redirection_file_Name))
            {
                // Write each redirection rule to a new line in the file
                for (int i = 0; i < redirectionRules.Length; i++)
                {
                    sw.WriteLine(redirectionRules[i]);
                }
            }
        }
         
    }
}
