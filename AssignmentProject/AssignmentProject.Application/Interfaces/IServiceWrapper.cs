using System;
using System.Collections.Generic;
using System.Text;

namespace AssignmentProject.Application.Interfaces
{
    public interface IServiceWrapper
    {
        IUserService UserService { get; }

        IBookService BookService { get; }
    }
}
