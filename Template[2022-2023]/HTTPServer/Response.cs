using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTTPServer
{
    // الاسئلة :
    /*
     * create request sting  المقصود بيه ايه
     *  statusline  -> 
     *  المفروض اديله لينك الويب سايت ولا اسم الايرور بس ؟ وايه رساله 200 ؟
     * 
     * 
     */
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
        public Response(StatusCode code, string contentType, string content, string redirectoinPath)
        {
            //throw new NotImplementedException();
            // TODO: Add headlines (Content-Type, Content-Length,Date, [location if there is redirection])
            headerLines.Add("Content_Type " + contentType + "/r/n");
            headerLines.Add("Content_Length " + content.Length + "/r/n");
            headerLines.Add("Date " + DateTime.Now +"/r/n" );
            if (code == StatusCode.Redirect)
            {
                headerLines.Add("Location " + redirectoinPath + "/r/n");
            }
            headerLines.Add("/r/n");
            // TODO: Create the request string
            responseString += GetStatusLine(code);
            foreach (string lines in headerLines)
            {
                responseString += lines ;

            }
            responseString += content + "\r\n";
        }

        private string GetStatusLine(StatusCode code)
        {
            // TODO: Create the response status line and return it
            string statusLine = string.Empty;
            statusLine += Configuration.ServerHTTPVersion +" ";
            statusLine += code + " ";
            switch (code)
            {
                case StatusCode.NotFound:
                    //statusLine += Configuration.NotFoundDefaultPageName;
                    statusLine += "NotFound";
                    break;
                case StatusCode.InternalServerError:
                    //statusLine += Configuration.InternalErrorDefaultPageName;
                    statusLine += "InternalServerError";
                    break;
                case StatusCode.Redirect:
                    //statusLine += Configuration.RedirectionDefaultPageName;
                    statusLine += "Redirect";
                    break;
                case StatusCode.BadRequest:
                    //statusLine += Configuration.BadRequestDefaultPageName;
                    statusLine += "BadRequest";
                    break;
                case StatusCode.OK:
                    statusLine += "Ok";
                    break;
            }
            statusLine += "\r\n";
            return statusLine;
        }
    }
}
