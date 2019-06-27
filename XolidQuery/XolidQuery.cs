using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text.RegularExpressions;

using NSoup;
using NSoup.Nodes;
using NSoup.Select;

namespace Dapper
{
    public static class XolidQuery
    {
        private static string _sqlMapPath = null;
        private static Dictionary<string, string> _documents;

        /// <summary>
        /// Set directory path, XML Query Mapper files located.
        /// </summary>
        /// <param name="mapPath"></param>
        public static void SetMapPath(string mapPath)
        {
            _sqlMapPath = mapPath;
        }
        
        /// <summary>
        /// Get xml file name and query id from queryInfo
        /// queryInfo = <XML File Name>.<queryId>
        /// </summary>
        /// <param name="queryInfo"></param>
        /// <returns></returns>
        /// <exception cref="InvalidQueryIdXQueryException"></exception>
        private static QueryInfo GetQueryInfo(string queryInfo)
        {
            if (!Regex.IsMatch(queryInfo, "^[a-zA-Z0-9]*[.][a-zA-Z0-9]*$"))
            {
                throw new InvalidQueryIdXQueryException(queryInfo);
            }

            string[] info = queryInfo.Split(".");

            return new QueryInfo()
            {
                FileName = info[0] + ".xml",
                Id = info[1]
            };
        }
        
        private class QueryInfo
        {
            public string FileName;
            public string Id;
        }
        
        private static Document GetDocument(string fileName)
        {
            if (_documents == null)
            {
                _documents = new Dictionary<string, string>();
            }

            if (_documents.ContainsKey(fileName))
            {
                return NSoupClient.Parse(_documents[fileName]);
            }
            
            if (_sqlMapPath == null)
            {
                throw new SQLMapPathNotDefinedXQueryException();
            }
            
            string filePath = Path.Combine(_sqlMapPath , fileName);
            if (!File.Exists(filePath))
            {
                throw new MapperFileNotFoundXQueryException(filePath);
            }
            
            string xml = File.ReadAllText(filePath);

            // bug fix select tag in NSoupClient.Parse : can't get children of select element, why not?
            xml = xml.Replace("<select", "<sel").Replace("</select>", "</sel>");
            
            Document doc = NSoupClient.Parse(xml);
            _documents.Add(fileName, xml);

            return doc;
        }

        private static string GetParamValue(object param, string property)
        {
            string value = null;

            if (param != null && param.GetType().GetProperty(property) != null)
            {
                var val = param.GetType().GetProperty(property).GetValue(param, null);
                if (val != null) value = val.ToString();
            }

            return value;
        }

        public static string GetQuery(string queryInfo, object param = null)
        {
            var info = GetQueryInfo(queryInfo);
            Document doc = GetDocument(info.FileName);

            return GetQuery(doc, info.Id, param);
        }
        
        private static string GetQuery(Document doc, string queryId, object param = null)
        {
            Element element = doc.Select("#" + queryId).First;
            if (element == null)
            {
                throw new QueryNotFoundXQueryException(queryId);
            }

            Tag(doc, element, param);

            string query = element.Text();

            return query;
        }
        
        private static bool Test(string test, object param)
        {
            bool result = false;
            
            string[] expression = test.Split(" ");
            
            string aValue = GetParamValue(param, expression[0]);
            string op = expression[1];
            string bValue = expression[2];

            bValue = bValue.Replace("'", "").Replace("\"", "");

            
            //if (aValue == null) value = "null";
                
            Regex isNumber = new Regex(@"^\d+$");

            if (isNumber.Match(aValue).Success)
            {
                float aNumberValue = float.Parse(aValue);
                float bNumberValue = float.Parse(bValue);

                switch (op)
                {
                    case "==":
                        result = aNumberValue == bNumberValue;
                        break;
                    case "!=":
                        result = aNumberValue != bNumberValue;
                        break;
                    case "<":
                        result = aNumberValue < bNumberValue;
                        break;
                    case "<=":
                        result = aNumberValue <= bNumberValue;
                        break;
                    case ">":
                        result = aNumberValue > bNumberValue;
                        break;
                    case ">=":
                        result = aNumberValue >= bNumberValue;
                        break;
                }
            }
            else
            {
                switch (op)
                {
                    case "==":
                        result = aValue.Equals(bValue);
                        break;
                    case "!=":
                        result = !aValue.Equals(bValue);
                        break;
                }
            }

            return result;
        }

        private static void Tag(Document doc, Element element, object param)
        {
            Elements tags = element.Select("> *");
            foreach (Element tag in tags)
            {
                switch (tag.Tag.Name)
                {
                    case "isnotnull":
                        IsNotNullTag(doc, tag, param);
                        break;
                    case "if":
                        IfTag(doc, tag, param);
                        break;
                    case "choose":
                        ChooseTag(doc, tag, param);
                        break;
                    case "include":
                        IncludeTag(doc, tag, param);
                        break;
                    case "where":
                        WhereTag(doc, tag, param);
                        break;
                    default:
                        throw new UnsupportedTagXQueryException(tag.Tag.Name);
                }
            }
        }

        private static void IsNotNullTag(Document doc, Element element, object param)
        {
            string property = element.Attr("property");

            string value = GetParamValue(param, property);

            if (value == null)
            {
                element.Remove();
            }
            else
            {
                Tag(doc, element, param);
            }
        }

        private static void IfTag(Document doc, Element element, object param)
        {
            string test = element.Attr("test");

            bool result = Test(test, param);

            if (!result)
            {
                element.Remove();
            }
            else
            {
                Tag(doc, element, param);
            }
        }

        private static void ChooseTag(Document doc, Element element, object param)
        {
            Elements whenElements = element.Select("> when");
            Element otherwiseElement = element.Select("> otherwise").First;
            
            bool isAleardyPassed = false;
            foreach (var whenElement in whenElements)
            {
                if (isAleardyPassed)
                {
                    whenElement.Remove();
                }
                else
                {
                    string test = whenElement.Attr("test");
                    isAleardyPassed = Test(test, param);

                    if (!isAleardyPassed)
                    {
                        whenElement.Remove();
                    }
                    else
                    {
                        Tag(doc, whenElement, param);
                    }
                }
            }

            if (isAleardyPassed)
            {
                otherwiseElement.Remove();
            }
            else
            {
                Tag(doc, otherwiseElement, param);
            }
        }

        private static void IncludeTag(Document doc, Element element, object param)
        {
            string refId = element.Attr("refid");

            string query = GetQuery(doc, refId, param);

            element.Text(query);
        }

        private static void WhereTag(Document doc, Element element, object param)
        {
            Tag(doc, element, param);

            string whereQuery = element.Text();

            // Remove first 'AND'
            if (whereQuery.ToLower().StartsWith("and"))
            {
                whereQuery = " WHERE " + whereQuery.Substring(3, whereQuery.Length - 3);
            }
            else if(!string.IsNullOrEmpty(whereQuery))
            {
                whereQuery = " WHERE " + whereQuery;
            }

            element.Text(whereQuery);
        }
    }
}