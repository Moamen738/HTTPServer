using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace HTTPServer
{
    public enum RequestMethod
    {
        GET,
        POST,
        HEAD
    }

    public enum HTTPVersion
    {
        HTTP10,
        HTTP11,
        HTTP09
    }

    class Request
    {
        string[] requestLines;
        RequestMethod method;
        public string relativeURI;
        Dictionary<string, string> headerLines;
        public static bool check = false;
        public Dictionary<string, string> HeaderLines
        {
            get { return headerLines; }
        }

        HTTPVersion httpVersion;
        string requestString;
        string[] contentLines;

        public Request(string requestString)
        {
            this.requestString = requestString;
        }
        /// <summary>
        /// Parses the request string and loads the request line, header lines and content, returns false if there is a parsing error
        /// </summary>
        /// <returns>True if parsing succeeds, false otherwise.</returns>
        /// 

        // Habiba
        public bool ParseRequest()
        {
            
            //TODO: parse the receivedRequest using the \r\n delimeter   
            string[] stringSeparators = new string[] { "\r\n" };
            requestLines = requestString.Split(stringSeparators, StringSplitOptions.None);
            // check that there is atleast 3 lines: Request line, Host Header, Blank line (usually 4 lines with the last empty line for empty content)
            if (requestLines.Length < 4)
                return false;
            // Parse Request line
            bool validRequest = ParseRequestLine();
            // Validate blank line exists
            validRequest &= ValidateBlankLine();
            // Load header lines into HeaderLines dictionary
            validRequest &= LoadHeaderLines();

            return validRequest;
        }

        // Habiba
        private bool ParseRequestLine()
        {
            string[] requestLine_parts = requestLines[0].Split(' ');
            if (requestLine_parts[0] == "GET")
                method = RequestMethod.GET;
            else if (requestLine_parts[0] == "POST")
            {
                method = RequestMethod.POST;

                createXmlFile(requestLines);
            }
            else if (requestLine_parts[0] == "HEAD")
                method = RequestMethod.HEAD;
            else
                return false;

            if (ValidateIsURI(requestLine_parts[1]))
                relativeURI = requestLine_parts[1].Split('/')[1];
            else
                return false;

            if (requestLine_parts[2] == "HTTP/0.9")
                httpVersion = HTTPVersion.HTTP09;
            else if (requestLine_parts[2] == "HTTP/1.0")
                httpVersion = HTTPVersion.HTTP10;
            else if (requestLine_parts[2] == "HTTP/1.1")
                httpVersion = HTTPVersion.HTTP11;
            else
                return false;

            return true;
        }
        void createXmlFile(string[] Arr)
        {
            using (XmlWriter writer = XmlWriter.Create("Content.xml"))
            {
                writer.WriteStartElement("Data");
                writer.WriteElementString("Name", Arr[22]);
                writer.WriteEndElement();
                writer.Flush();
            }
        }
        private bool ValidateIsURI(string uri)
        {
            return Uri.IsWellFormedUriString(uri, UriKind.RelativeOrAbsolute);
        }

        // Esraa
        private bool LoadHeaderLines()
        {
            headerLines = new Dictionary<string, string>();
            bool hostAttribute = false;
            string[] stringSeparators = new string[] { ": " };
            for (int i = 1; i < requestLines.Length; i++)
            {
                // End of Header Lines
                if (requestLines[i].Length == 0)
                    break;
                // If the Header Line not formatted correctly
                if (!requestLines[i].Contains(": "))
                    return false;

                string[] Attributes = requestLines[i].Split(stringSeparators, StringSplitOptions.None);
                if(Attributes[0] == "Host")
                {
                    if (hostAttribute)
                        return false;
                    hostAttribute = true;
                }
                headerLines[Attributes[0]] = Attributes[1];
            }
            return hostAttribute;
        }

        // Esraa
        private bool ValidateBlankLine()
        {
            string[] stringSeparators = new string[] { "\r\n" };
            string[] lines = requestString.Split(stringSeparators, StringSplitOptions.None);    
            // E3mily check en hyzhar empty wykon lessa el-index < Length - 1
            foreach(string line in lines) {
                if (line == string.Empty)
                    return true;
            }
            return false;
        }

    }
}
