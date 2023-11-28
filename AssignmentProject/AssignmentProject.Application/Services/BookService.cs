using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AspnetRun.Application.Models;
using AssignmentProject.Application.Interfaces;
using AssignmentProject.Application.Mapper;
using AssignmentProject.Application.Models;
using AssignmentProject.Core.Entities;
using AssignmentProject.Core.Repositories;

namespace AssignmentProject.Application.Services
{
    internal sealed class BookService : IBookService
    {
        private readonly IRepositoryWrapper _repositoryWrapper;

        public BookService(IRepositoryWrapper repositoryWrapper)
        {
            _repositoryWrapper = repositoryWrapper ?? throw new ArgumentNullException(nameof(repositoryWrapper));
        }

        public async Task<IEnumerable<BookModel>> GetBookList()
        {
            var bookList = await _repositoryWrapper.BookRepository.GetAllAsync();
            var mapped = ObjectMapper.Mapper.Map<IEnumerable<BookModel>>(bookList);
            return mapped;
        }

        public async Task<BookModel> GetBookById(int id)
        {
            var product = await _repositoryWrapper.BookRepository.GetByIdAsync(id);
            var mapped = ObjectMapper.Mapper.Map<BookModel>(product);
            return mapped;
        }

        public async Task Create(BookModel bookModel)
        {
            var mappedEntity = ObjectMapper.Mapper.Map<Book>(bookModel);
            if (mappedEntity == null)
                throw new ApplicationException($"Book Entity could not be mapped for: {bookModel}");

            _repositoryWrapper.BookRepository.Add(mappedEntity);
            await _repositoryWrapper.UnitOfWork.SaveChangesAsync();

        }

        public async Task Update(BookModel bookModel)
        {
            var editBook = await _repositoryWrapper.BookRepository.GetByIdAsync(bookModel.Id);
            if (editBook == null)
                throw new ApplicationException($"Entity could not be loaded for: {bookModel}");

            ObjectMapper.Mapper.Map<BookModel, Book>(bookModel, editBook);

            _repositoryWrapper.BookRepository.Update(editBook);
            await _repositoryWrapper.UnitOfWork.SaveChangesAsync();

        }

        public async Task<IEnumerable<BookModel>> GetBooksByName(string bookName)
        {
            var bookList = await _repositoryWrapper.BookRepository.GetBooksByNameAsync(bookName);
            var mapped = ObjectMapper.Mapper.Map<IEnumerable<BookModel>>(bookList);
            return mapped;
        }

        public async Task<IEnumerable<BookModel>> GetBooksByAuthor(string authorName)
        {
            var bookList = await _repositoryWrapper.BookRepository.GetBooksByAuthorAsync(authorName);
            var mapped = ObjectMapper.Mapper.Map<IEnumerable<BookModel>>(bookList);
            return mapped;
        }

        public async Task Delete(BookModel bookModel)
        {
            var editBook = await _repositoryWrapper.BookRepository.GetByIdAsync(bookModel.Id);
            if (editBook == null)
                throw new ApplicationException($"Entity could not be loaded for: {bookModel}");

            _repositoryWrapper.BookRepository.Delete(editBook);
            await _repositoryWrapper.UnitOfWork.SaveChangesAsync();

        }

    }
}
