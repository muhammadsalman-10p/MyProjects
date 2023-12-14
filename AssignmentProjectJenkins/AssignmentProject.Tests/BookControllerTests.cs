using AssignmentProject.Application.Interfaces;
using AssignmentProject.Application.Models;
using AssignmentProject.Controllers;
using AssignmentProject.Core.Repositories;
using LoggingService;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace AssignmentProject.Tests
{
    public class BookControllerTests
    {
        public readonly Mock<IServiceWrapper> _serviceWrapper;
        private readonly Mock<ILoggerManager> _logger;
        public readonly BookController _bookController;

        public BookControllerTests()
        {
            _serviceWrapper = new Mock<IServiceWrapper>();
            _logger = new Mock<ILoggerManager>();
            _bookController = new BookController(_serviceWrapper.Object, _logger.Object);
           
           
        }
        [Fact]
        public async Task Index_ActionExecutes_ViewSuccess()
        {
            
            _serviceWrapper.Setup(repo => repo.BookService.GetBookList()).ReturnsAsync(GetTestBooks());
            var result = await _bookController.Index();

            Assert.IsType<OkObjectResult>(result);
        }
        [Fact]
        public async Task Index_MatchBookModelAndCount()
        {
            _serviceWrapper.Setup(repo => repo.BookService.GetBookList()).ReturnsAsync(GetTestBooks());
            int expectedBookCount = 2;
            var result = await _bookController.Index();

            var viewRessult = Assert.IsType<OkObjectResult>(result);
            var booksReturned = Assert.IsAssignableFrom<List<BookModel>>(viewRessult.Value);
            Assert.Equal(expectedBookCount, booksReturned.Count);
        }
        [Fact]
        public async Task GetBook_InvlidId_ReturnView()
        {
            int? id = null;
            var result = await _bookController.GetBook(id);

            var viewRessult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid id - fest failed", viewRessult.Value);
            //Assert.IsAssignableFrom<BookModel>(viewRessult.Value);
        }
        [Fact]
        public async Task GetBook_ReturnValidBook()
        {
            int bookId = 2;
            var book = GetTestBooks()[1];
            _serviceWrapper.Setup(repo => repo.BookService.GetBookById(bookId)).ReturnsAsync(book);
            var result = await _bookController.GetBook(bookId);

            var viewRessult = Assert.IsType<OkObjectResult>(result);
            BookModel bm = Assert.IsAssignableFrom<BookModel>(viewRessult.Value);
            Assert.Equal(bookId, bm.Id);
        }
        [Fact]
        public async Task GetBook_NotFound()
        {
            //arrange
            int bookId = 3;
            _serviceWrapper.Setup(repo => repo.BookService.GetBookById(bookId)).ReturnsAsync((BookModel)null);
            //act

            var result = await _bookController.GetBook(bookId);
            //assert
            Assert.IsType<NotFoundResult>(result);
        }
        [Fact]
        public async Task Upsert_Create_InvalidModel_ReturnsView()
        {
            //arrange
            BookModel book = null;//new BookModel { Author = "Author 1" };
            //act
            _bookController.ModelState.AddModelError("Name", "Name is required");
            var result = await _bookController.Upsert(model: book);
            //assert
            var viewResult = Assert.IsType<BadRequestObjectResult>(result);
           //var testBook = Assert.IsAssignableFrom<BookModel>(viewResult.Value);
            Assert.Equal("BookModel object is null", viewResult.Value);
            //Assert.Equal(testBook.Author, book.Author);
            _serviceWrapper.Verify(x => x.BookService.Create(It.IsAny<BookModel>()), Times.Never);

        }
        [Fact]
        public async Task Upsert_Create_InvalidModelState_ReturnsView()
        {
            //arrange
            var book = new BookModel { Author = "Author 1" };
            //act
            _bookController.ModelState.AddModelError("Name", "Name is required");
            var result = await _bookController.Upsert(model: book);
            //assert
            var viewResult = Assert.IsType<BadRequestObjectResult>(result);
            //var testBook = Assert.IsAssignableFrom<BookModel>(viewResult.Value);
            Assert.Equal("Invalid model object", viewResult.Value);
            //Assert.Equal(testBook.Author, book.Author);
            _serviceWrapper.Verify(x => x.BookService.Create(It.IsAny<BookModel>()), Times.Never);

        }
        [Fact]
        public async Task Upsert_Create_Save_Success()
        {
            //arrange
            BookModel testBook = null;// { Name = "Programming", Author = "Author 1", PublishedDate=new DateTime() };
            _serviceWrapper.Setup(repo => repo.BookService.Create(It.IsAny<BookModel>())).Callback<BookModel>(data => testBook = data);
            BookModel book = new BookModel { Name = "Programming", Author = "Author 1", PublishedDate = new DateTime() };

            //act
            var result = await _bookController.Upsert(model: book);
            //assert
            _serviceWrapper.Verify(x => x.BookService.Create(It.IsAny<BookModel>()), Times.Once);

            Assert.Equal(testBook, book);
            //Assert.Equal(testBook.Name, book.Name);
            //Assert.Equal(testBook.Author, book.Author);

            var actionResult = Assert.IsType<OkResult>(result);
            //Assert.Equal("Index", actionResult.ActionName);
            //Assert.Equal("Book", actionResult.ControllerName);
        }
        [Fact]
        public async Task Upsert_Edit_Save_Success()
        {
            //arrange
            BookModel testBook = null;// { Name = "Programming", Author = "Author 1", PublishedDate=new DateTime() };
            _serviceWrapper.Setup(repo => repo.BookService.Update(It.IsAny<BookModel>())).Callback<BookModel>(data => testBook = data);
            BookModel book = new BookModel { Id = 2, Name = "Programming", Author = "Author 1", PublishedDate = new DateTime() };

            //act
            var result = await _bookController.Upsert(model: book);
            //assert
            _serviceWrapper.Verify(x => x.BookService.Update(It.IsAny<BookModel>()), Times.Once);

            Assert.Equal(testBook, book);
            //Assert.Equal(testBook.Name, book.Name);
            //Assert.Equal(testBook.Author, book.Author);

            var actionResult = Assert.IsType<OkResult>(result);
            //Assert.Equal("Index", actionResult.ActionName);
            //Assert.Equal("Book", actionResult.ControllerName);
        }
        private List<BookModel> GetTestBooks()
        {
            var books = new List<BookModel>();
            books.Add(new BookModel { Id = 1, Name = "Abc", Author = "Xyz", PublishedDate = new DateTime() });
            books.Add(new BookModel { Id = 2, Name = "DS", Author = "John", PublishedDate = new DateTime() });

            return books;
        }
    }
}
