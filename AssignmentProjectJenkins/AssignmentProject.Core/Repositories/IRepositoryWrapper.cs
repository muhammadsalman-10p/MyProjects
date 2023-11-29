using System;
using System.Collections.Generic;
using System.Text;

namespace AssignmentProject.Core.Repositories
{
    public interface IRepositoryWrapper
    {
        IUserRepository UserRepository { get; }

        IBookRepository BookRepository { get; }

        IUnitOfWork UnitOfWork { get; }
    }
}
