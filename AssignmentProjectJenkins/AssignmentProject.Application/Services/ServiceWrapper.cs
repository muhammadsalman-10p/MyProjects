using AssignmentProject.Application.Interfaces;
using AssignmentProject.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace AssignmentProject.Application.Services
{
    
    public sealed class ServiceWrapper : IServiceWrapper
    {
        private readonly Lazy<IUserService> _lazyUserService;
        private readonly Lazy<IBookService> _lazyBookService;

        public ServiceWrapper(IRepositoryWrapper repositoryWrapper)
        {
            _lazyUserService = new Lazy<IUserService>(() => new UserService(repositoryWrapper));
            _lazyBookService = new Lazy<IBookService>(() => new BookService(repositoryWrapper));
        }

        public IUserService UserService
        {
            get
            {
                return _lazyUserService.Value;
            }
        }

        public IBookService BookService
        {
            get
            {
                return _lazyBookService.Value;
            }
        }
    }
}
