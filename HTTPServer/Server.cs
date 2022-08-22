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
            Configuration.RedirectionRules = new Dictionary<string, string>();
            this.LoadRedirectionRules("redirectionRules.txt");
            //TODO: initialize this.serverSocket
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(new IPEndPoint(IPAddress.Any, portNumber));
        }
        // Dalia
        public void StartServer()
        {
            // TODO: Listen to connections, with large backlog.
            serverSocket.Listen(1000);
            // TODO: Accept connections in while loop and start a thread for each connection on function "Handle Connection"
            while (true)
            {
                //TODO: accept connections and start thread for each accepted connection.
                Socket clientSocket = this.serverSocket.Accept();
                Console.WriteLine("New client accepted: {0}", clientSocket.RemoteEndPoint);

                Thread newThread = new Thread(new ParameterizedThreadStart(HandleConnection));
                newThread.Start(clientSocket);
            }
        }
        // Dalia
        public void HandleConnection(object obj) 
        {
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
                    byte []requestReceived = new byte[5000];
                    int receivedBytesLength = clientSocket.Receive(requestReceived); 

                    // TODO: break the while loop if receivedLen==0
                    if (receivedBytesLength == 0)
                        break;

                    // TODO: Create a Request object using received request string
                    string requestString = Encoding.ASCII.GetString(requestReceived);
                    Request requestOfClient = new Request(requestString);
                    
                    // TODO: Call HandleRequest Method that returns the response
                    Response responseOfServer = HandleRequest(requestOfClient);

                    // TODO: Send Response back to client
                    byte[] responseByteArray = new byte[responseOfServer.ResponseString.Length];
                    responseByteArray = Encoding.ASCII.GetBytes(responseOfServer.ResponseString);
                    clientSocket.Send(responseByteArray);
                }
                catch (Exception ex)
                {
                    // TODO: log exception using Logger class
                    Logger.LogException(ex);
                }
            }
            // TODO: close client socket
            clientSocket.Close();
        }

        // Mo'men
        Response HandleRequest(Request request)
        {
            string content;
            StatusCode code;
            Response FinalResponse;
            try
            {
                //throw new Exception();
                //TODO: check for bad request 
                bool GoodResponse = request.ParseRequest();
                if (GoodResponse == false)
                {
                    code = StatusCode.BadRequest;
                    content = LoadDefaultPage(Configuration.BadRequestDefaultPageName);
                    FinalResponse = new Response(code, "text/html", content, null);
                    return FinalResponse;
                }
          
                //TODO: map the relativeURI in request to get the physical path of the resource.
                //TODO: check for redirect // a2ra page el redir
                string Redirect = GetRedirectionPagePathIFExist(request.relativeURI);
                if (Redirect != string.Empty)
                {
                    code = StatusCode.Redirect;
                    content = LoadDefaultPage(Configuration.RedirectionDefaultPageName);
                    FinalResponse = new Response(code, "text/html", content, Redirect);
                    return FinalResponse;
                }

                //TODO: check file exists
                string filePath = Path.Combine(Configuration.RootPath, request.relativeURI);
                if (!File.Exists(filePath))
                {
                    code = StatusCode.NotFound;
                    content = LoadDefaultPage(Configuration.NotFoundDefaultPageName);
                    FinalResponse = new Response(code, "text/html", content, null);
                    return FinalResponse;
                }
                //TODO: read the physical file
                content = LoadDefaultPage(request.relativeURI);
                // Create OK response
                code = StatusCode.OK;
                FinalResponse = new Response(code, "text/html", content, null);
                return FinalResponse;
            }
            catch (Exception ex)
            {
                // TODO: log exception using Logger class
                // TODO: in case of exception, return Internal Server Error.
                Logger.LogException(ex);
                code = StatusCode.InternalServerError;
              
                content = LoadDefaultPage(Configuration.InternalErrorDefaultPageName);

                FinalResponse = new Response(code, "text/html", content, null);
                return FinalResponse;
            }
        }

        // Mo'men
        private string GetRedirectionPagePathIFExist(string relativePath)
        {
            // using Configuration.RedirectionRules return the redirected page path if exists else returns empty
            if (Configuration.RedirectionRules.ContainsKey(relativePath))
                return Configuration.RedirectionRules[relativePath];
            return string.Empty;
        }

        // Mo'men
        private string LoadDefaultPage(string defaultPageName)
        {
            string filePath = Path.Combine(Configuration.RootPath, defaultPageName);
            // TODO: check if filepath not exist log exception using Logger class and return empty string     
            // else read file and return its content

            try {
                string content = "";
                using (StreamReader sr = File.OpenText(filePath)) {
                    string s = "";
                    while ((s = sr.ReadLine()) != null)
                        content += s;
                }
                return content;
            }

            catch(Exception ex) {
                Logger.LogException(ex);
                return string.Empty;
            }
        }
        private void LoadRedirectionRules(string filePath)
        {
            try
            {
                // TODO: using the filepath paramter read the redirection rules from file 
                if (File.Exists(filePath))
                {
                    // then fill Configuration.RedirectionRules dictionary 
                    using (StreamReader sr = File.OpenText(filePath))
                    {
                        string s = "";
                        while ((s = sr.ReadLine()) != null)
                            Configuration.RedirectionRules.Add(s.Split(',')[0], s.Split(',')[1]);
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
