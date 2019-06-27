using System;

namespace XolidQueryExample.Models
{
    public class User
    {
        public int? id { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public int? age { get; set; }
        public DateTime regDate { get; set; }
    }
}