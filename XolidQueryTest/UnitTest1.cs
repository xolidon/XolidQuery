using System;
using Dapper;
using NUnit.Framework;

namespace Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
            XolidQuery.SetMapPath("/Users/xolid/Workspace/XolidQuery/Example/Maps");
        }

        [Test]
        public void GetQueryTest()
        {
            string query = XolidQuery.GetQuery("User.findAll", new
            {
                name = "이지은"
            });
            
            Console.WriteLine(query);
            
            Assert.Pass();
        }
    }
}