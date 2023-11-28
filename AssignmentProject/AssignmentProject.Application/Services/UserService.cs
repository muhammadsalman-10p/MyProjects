using AssignmentProject.Application.Interfaces;
using AssignmentProject.Application.Mapper;
using AssignmentProject.Application.Models;
using AssignmentProject.Core.Entities;
using AssignmentProject.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AssignmentProject.Application.Services
{
    internal sealed class UserService :IUserService
    {
        private readonly IRepositoryWrapper _repositoryWrapper;

        public UserService(IRepositoryWrapper repositoryWrapper)
        {
            _repositoryWrapper = repositoryWrapper ?? throw new ArgumentNullException(nameof(repositoryWrapper));
        }
        public UserModel GetUserById(int id)
        {
            var product =  _repositoryWrapper.UserRepository.GetUserById(id);
            var mapped = ObjectMapper.Mapper.Map<UserModel>(product);
            return mapped;
        }
        public async Task<UserModel> Create(RegisterModel userModel, string password)
        {
            var mappedEntity = ObjectMapper.Mapper.Map<User>(userModel);
            if (mappedEntity == null)
                throw new ApplicationException($"Entity could not be mapped.");

            var user = _repositoryWrapper.UserRepository.Create(mappedEntity, password);
            await _repositoryWrapper.UnitOfWork.SaveChangesAsync();
            var mappedModel = ObjectMapper.Mapper.Map<UserModel>(user);
            return mappedModel;

        }
        public async Task<UserModel> Authenticate(string username, string password)
        {
            var user = await _repositoryWrapper.UserRepository.AuthenticateAsync(username, password);
            var mappedModel = ObjectMapper.Mapper.Map<UserModel>(user);
            return mappedModel;

        }

    }
}
