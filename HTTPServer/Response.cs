using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTTPServer
{

    public enum StatusCode
    {
        OK = 200,
        InternalServerError = 500,
        NotFound = 404,
        BadRequest = 400,
        Redirect = 301
    }

    class Response
    {
        string responseString;
        public string ResponseString
        {
            get
            {
                return responseString;
            }
        }
        StatusCode code;
        List<string> headerLines = new List<string>();

        // Doaa
        public Response(StatusCode code, string contentType, string content, string redirectionPath)
        {
            // The actual message of the status, because code gives us the number
            string statusLine = GetStatusLine(code); // GetStatusLine(code); // Version?
            // version fel config

            // TODO: Add headlines (Content-Type, Content-Length, Date, [location if there is redirection])
            string dateHeader = "Date: " + DateTime.Now.ToString() + "\r\n";
            headerLines.Add(dateHeader);

            string contentTypeHeader = "Content-Type: " + contentType + "\r\n";
            headerLines.Add(contentTypeHeader);

            string contentLengthHeader = "Content-Length" + content.Length.ToString() + "\r\n";
            headerLines.Add(contentLengthHeader);

            if (code == StatusCode.Redirect) {
                string locationHeader = "Location: " + redirectionPath + "\r\n";
                headerLines.Add(locationHeader);
            }

            string header = "";
            foreach(string headerLine in headerLines) {
                header += headerLine;
            }

            // TODO: Create the response string
            responseString = statusLine + header + "\r\n" + content;
        }

        // Doaa
        private string GetStatusLine(StatusCode code)
        {
            // TODO: Create the response status line and return it
            int statusCode = (int)code;
            string  statusLine = Configuration.ServerHTTPVersion + " " + statusCode.ToString() + " " + code + "\r\n";
            return statusLine;
        }
    }
}
