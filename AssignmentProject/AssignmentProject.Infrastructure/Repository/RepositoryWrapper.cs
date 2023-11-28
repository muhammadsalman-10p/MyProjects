using AssignmentProject.Core.Repositories;
using AssignmentProject.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace AssignmentProject.Infrastructure.Repository
{
    public sealed class RepositoryWrapper : IRepositoryWrapper
    {
        private readonly Lazy<IUserRepository> _lazyUserRepository;
        private readonly Lazy<IBookRepository> _lazyBookRepository;
        private readonly Lazy<IUnitOfWork> _lazyUnitOfWork;

        public RepositoryWrapper(ApplicationDbContext dbContext)
        {
            _lazyUserRepository = new Lazy<IUserRepository>(() => new UserRepository(dbContext));
            _lazyBookRepository = new Lazy<IBookRepository>(() => new BookRepository(dbContext));
            _lazyUnitOfWork = new Lazy<IUnitOfWork>(() => new UnitOfWork(dbContext));
        }

        public IUserRepository UserRepository
        {
            get
            {
                return _lazyUserRepository.Value;
            }
        }

        public IBookRepository BookRepository
        {
            get
            {
                return _lazyBookRepository.Value;
            }
        }

        public IUnitOfWork UnitOfWork
        {
            get
            {
                return _lazyUnitOfWork.Value;
            }
        }
    }
}
