using System;

namespace Dapper
{
    public class XQueryException : Exception
    { 
        protected string _message;
        public override string Message
        {
            get { return _message; }
        }
    }

    public class SQLMapPathNotDefinedXQueryException : XQueryException
    {
        public SQLMapPathNotDefinedXQueryException()
        {
            _message = "SQL_MAP_PATH is not defined. Set xml path in Startup.cs, likes follow code\n";
            _message += "====================================================================================\n";
            _message += "public Startup(IConfiguration configuration, IHostingEnvironment hostingEnvironment)\n";
            _message += "{\n";
            _message += "Configuration = configuration;\n\n";
            _message += "// Set mapper xml file path\n";
            _message += "XolidQuery.SetMapPath(hostingEnvironment.ContentRootPath + \"/Maps\");\n";
            _message += "}\n";
            _message += "====================================================================================\n";
        }
    }

    public class MapperFileNotFoundXQueryException : XQueryException
    {
        public MapperFileNotFoundXQueryException(string filePath)
        {
            _message = string.Format("filePath : {0}", filePath);
        }
    }

    public class InvalidQueryIdXQueryException : XQueryException
    {
        public InvalidQueryIdXQueryException(string queryId)
        {
            _message = string.Format("queryId is must to be <FileName>.<QueryId> (ex: User.getOne). Input queryId : {0}", queryId);
        }
    }

    public class PropertyNotFoundXQueryException : XQueryException
    {
        public PropertyNotFoundXQueryException(string propertyName)
        {
            _message = string.Format("Property Name '{0}' is not found in tag block.", propertyName);
        }
    }

    public class UnsupportedTagXQueryException : XQueryException
    {
        public UnsupportedTagXQueryException(string tagName)
        {
            _message = string.Format("Unsupported tag name : {0}", tagName);
        }
    }
}