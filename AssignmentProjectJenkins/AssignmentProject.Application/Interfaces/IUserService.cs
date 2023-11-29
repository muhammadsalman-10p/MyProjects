using AssignmentProject.Application.Models;
using AssignmentProject.Core;
using AssignmentProject.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AssignmentProject.Application.Interfaces
{
     
    public interface IUserService
    {
        Task<UserModel> Authenticate(string username, string password);
        UserModel GetUserById(int id);
        Task<UserModel> Create(RegisterModel userModel, string password);
    }
}
