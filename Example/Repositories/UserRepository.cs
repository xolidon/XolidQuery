using System.Collections.Generic;
using System.Data;
using Dapper;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using XolidQueryExample.Models;

namespace XolidQueryExample.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IConfiguration _config;
        
        public UserRepository(IConfiguration config)
        {
            _config = config;
        }
        
        public IDbConnection Connection
        {
            get { return new MySqlConnection(_config.GetConnectionString("mysql")); }
        }
        
        public List<User> GetUserList(User user)
        {
            List<User> users = null;

            using (var conn = Connection)
            {
                users = (List<User>) conn.XQuery<User>("User.findAll", user);
            }

            return users;
        }

        public User GetUser(int id)
        {
            User user = null;

            using (var conn = Connection)
            {
                user = (User) conn.XQuery<User>("User.getOne", new
                {
                    id = id
                });
            }

            return user;
        }

        public int AddUser(User user)
        {
            int ret = 0;
            using (var conn = Connection)
            {
               ret = conn.Execute("User.insert", user);
            }

            return ret;
        }

        public int UpdateUser(User user)
        {
            throw new System.NotImplementedException();
        }

        public int DeleteUser(int id)
        {
            throw new System.NotImplementedException();
        }
    }
}