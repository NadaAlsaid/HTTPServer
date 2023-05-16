using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        public bool ParseRequest()
        {
            //throw new NotImplementedException();

            //TODO: parse the receivedRequest using the \r\n delimeter   

            // check that there is atleast 3 lines: Request line, Host Header, Blank line (usually 4 lines with the last empty line for empty content)

            // Parse Request line

            // Validate blank line exists
            // Load header lines into HeaderLines dictionary
            requestLines = requestString.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            if (requestLines.Length < 3)
            {
                return false;
            }

            // Parse the request line
            if (!ParseRequestLine())
            {
                return false;
            }
            // Validate that a blank line exists after the header section
            if (!ValidateBlankLine())
            {
                return false;
            }

            // Parse the header lines
            if (!LoadHeaderLines())
            {
                return false;
            }
            return true;
        }

        private bool ParseRequestLine()
        {
            //throw new NotImplementedException();
            requestLines = requestString.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            // Parse the request line
            string[] requestLineParts = requestLines[0].Split(' ');
            if (requestLineParts.Length != 3)// 1)HTTP method ,2)relative URI,3)HTTP version
            {
                return false; //wrong format
            }
            // Parse the HTTP method
            if (!Enum.TryParse(requestLineParts[0], out method))
            {
                // The HTTP method is not valid
                return false;
            }
            else if (!Enum.TryParse(requestLineParts[0], out method))
            {
                // The HTTP method is not valid
                return false;
            }
            // Parse the relative URI
            relativeURI = requestLineParts[1];

            // Parse the HTTP version
            switch (requestLineParts[2])
            {
                case "HTTP/1.0":
                    httpVersion = HTTPVersion.HTTP10;
                    break;
                case "HTTP/1.1":
                    httpVersion = HTTPVersion.HTTP11;
                    break;
                case "HTTP/0.9":
                    httpVersion = HTTPVersion.HTTP09;
                    break;
                default:
                    return false;//invalid HTTP version
            }
            return ValidateIsURI(relativeURI);
        }

        private bool ValidateIsURI(string uri)
        {
            return Uri.IsWellFormedUriString(uri, UriKind.RelativeOrAbsolute);
        }

        private bool LoadHeaderLines()
        {
            // throw new NotImplementedException();
            requestLines = requestString.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            // Parse the header lines
            headerLines = new Dictionary<string, string>();
            int i = 1; // Start at index 1, since index 0 is the request line
            string line;
            while ((line = requestLines[i]) != "")
            {
                int colonIndex = line.IndexOf(':');
                if (colonIndex == -1)
                {
                    // Invalid header format
                    return false;
                }
                string fieldName = line.Substring(0, colonIndex).Trim();
                string fieldValue = line.Substring(colonIndex + 1).Trim();
                headerLines[fieldName] = fieldValue;
                i++;
            }

            // Set the HTTP version
            string[] requestLineParts = requestLines[0].Split(' ');
            switch (requestLineParts[2])
            {
                case "HTTP/1.0":
                    httpVersion = HTTPVersion.HTTP10;
                    break;
                case "HTTP/1.1":
                    httpVersion = HTTPVersion.HTTP11;
                    break;
                case "HTTP/0.9":
                    httpVersion = HTTPVersion.HTTP09;
                    break;
                default:
                    // Invalid HTTP version
                    return false;
            }

            return true;
        }

        private bool ValidateBlankLine()
        {
            //throw new NotImplementedException();
            requestLines = requestString.Split(new string[] { "\r\n" }, StringSplitOptions.None);

            // Find the index of the first blank line
            int blankLineIndex = Array.IndexOf(requestLines, "");
            // Check whether a blank line is present
            if (blankLineIndex == -1 || blankLineIndex == requestLines.Length - 1)
            {
                // Blank line not found, request is invalid
                return false;
            }

            // Blank line found, request is valid
            return true;
        }

    }
}
