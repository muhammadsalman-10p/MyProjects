using AssignmentProject.Core.Entities;
using AssignmentProject.Core.Repositories.Base;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AssignmentProject.Core.Repositories
{ 
    public interface IUserRepository : IRepository<User>
    {
        Task<User> AuthenticateAsync(string userName, string password);
        User Create(User user, string password);
        User GetUserById(int id);
    }
}
