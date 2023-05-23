using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace HTTPServer
{
    class Server
    {
        Socket serverSocket;

        public Server(int portNumber, string redirectionMatrixPath)
        {
            //TODO: call this.LoadRedirectionRules passing redirectionMatrixPath to it
            //TODO: initialize this.serverSocket
            this.LoadRedirectionRules(redirectionMatrixPath);
            this.serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint ipEnd = new IPEndPoint(IPAddress.Any, portNumber);
            serverSocket.Bind(ipEnd);
            
        }

        public void StartServer()
        {
            // TODO: Listen to connections, with large backlog.
            serverSocket.Listen(5000);
            // TODO: Accept connections in while loop and start a thread for each connection on function "Handle Connection"
            while (true)
            {
                //TODO: accept connections and start thread for each accepted connection.
                Socket client_socket = serverSocket.Accept();
                //Console.WriteLine("New Client in Service Queue : ", client_socket.RemoteEndPoint);
                EndPoint clnt = client_socket.RemoteEndPoint;
                Thread newClient = new Thread(new ParameterizedThreadStart(HandleConnection));

                ThreadPool.QueueUserWorkItem(HandleConnection, client_socket);
                newClient.Start(client_socket);
                
                
            }
        }

        public void HandleConnection(object obj)
        {
            //TcpClient client = (TcpClient)obj;

            // TODO: Create client socket 
            Socket clientSocket = (Socket)obj;

            // set client socket ReceiveTimeout = 0 to indicate an infinite time-out period
            clientSocket.ReceiveTimeout = 0;

            // TODO: receive requests in while true until remote client closes the socket.
            while (true)
            {
                try
                {
                    // TODO: Receive request
                    byte[] buffer = new byte[1024];
                    int receivedLen = clientSocket.Receive(buffer);

                    // TODO: break the while loop if receivedLen==0
                    if (receivedLen == 0)
                    {
                        break;
                    }

                    // TODO: Create a Request object using received request string
                    string requestString =  Encoding.ASCII.GetString(buffer, 0, receivedLen);
                    Request request = new Request(requestString);

                    // TODO: Call HandleRequest Method that returns the response
                    Response response = HandleRequest(request);

                    // TODO: Send Response back to client
                    byte[] responseBytes = Encoding.ASCII.GetBytes(response.ResponseString);
                    clientSocket.Send(responseBytes);
                }
                catch (Exception ex)
                {
                    // TODO: log exception using Logger class
                    Logger.LogException(ex);
                }
            }
        }

        Response HandleRequest(Request request)
        {
            string content;
            try
            {
                // Check for bad request
                if (request == null || !request.ParseRequest())
                {
                    return new Response(StatusCode.BadRequest, "text/html", LoadDefaultPage(Configuration.BadRequestDefaultPageName), null);
                }

                // Map the relativeURI in request to get the physical path of the resource
                string filePath = Configuration.RootPath + request.relativeURI.Replace('/', '\\');
                string redirect  = GetRedirectionPagePathIFExist(request.relativeURI.Remove(0,1));
                              
                // Check for redirect
                if (redirect != String.Empty)
                {
                    redirect = redirect.Remove(redirect.Length - 1 , 1);
                    return new Response(StatusCode.Redirect, "text/html", LoadDefaultPage(redirect), redirect);
                }
                // Check file exists
                if (!File.Exists(filePath))
                {
                    return new Response(StatusCode.NotFound, "text/html", LoadDefaultPage(Configuration.NotFoundDefaultPageName), null);
                }

                // Read the physical file
                content = File.ReadAllText(filePath);

                // Create OK response
                return new Response(StatusCode.OK, "text/html", content, null);
            }
            catch (Exception ex)
            {
                // Log exception using Logger class
                Logger.LogException(ex);

                // In case of exception, return Internal Server Error
                return new Response(StatusCode.InternalServerError, "text/html", LoadDefaultPage(Configuration.InternalErrorDefaultPageName), null);
            }
        }

        private string GetRedirectionPagePathIFExist(string relativePath)
        {
            // using Configuration.RedirectionRules return the redirected page path if exists else returns empty
            if (Configuration.RedirectionRules.ContainsKey(relativePath))
            {
                return Configuration.RedirectionRules[relativePath];
            }
            return string.Empty;


        }

        private string LoadDefaultPage(string defaultPageName)
        {
            string filePath = Path.Combine(Configuration.RootPath, defaultPageName);
            // TODO: check if filepath not exist log exception using Logger class and return empty string
            // else read file and return its content

            if (!File.Exists(filePath))
            {

                try
                {
                    Console.WriteLine("File does not exist");
                }
                catch (Exception ex)
                {
                    Logger.LogException(ex);
                }
                return string.Empty;

            }

            else
            {
                return File.ReadAllText(filePath);
            }

        }

        private void LoadRedirectionRules(string filePath)
        {
            try
            {
                // TODO: using the filepath paramter read the redirection rules from file 
                // then fill Configuration.RedirectionRules dictionary 
                string file = File.ReadAllText(filePath);
                string[] lines = file.Split('\n');
                Configuration.RedirectionRules = new Dictionary<string, string>();
                foreach (string line in lines)
                {
                    string[] parts = line.Split(',');
                    if (parts.Length == 2)
                    {
                        string old = parts[0];
                        string neww = parts[1];
                        Configuration.RedirectionRules[ old] = neww ;
                    }
                }
            }
            catch (Exception ex)
            {
                // TODO: log exception using Logger class
                Logger.LogException(ex);
                Environment.Exit(1);
            }
        }
    }
}
