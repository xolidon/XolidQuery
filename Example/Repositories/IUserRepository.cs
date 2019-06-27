using System.Collections.Generic;
using XolidQueryExample.Models;

namespace XolidQueryExample.Repositories
{
    public interface IUserRepository
    {
        List<User> GetUserList(User user);

        User GetUser(int id);

        int AddUser(User user);

        int UpdateUser(User user);

        int DeleteUser(int id);
    }
}