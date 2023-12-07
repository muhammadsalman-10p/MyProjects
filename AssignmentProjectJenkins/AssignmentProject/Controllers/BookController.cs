using AssignmentProject.Application.Interfaces;
using AssignmentProject.Application.Models;
using LoggingService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AssignmentProject.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class BookController : Controller
    {
        private readonly IServiceWrapper _serviceWrapper;
        private readonly ILoggerManager _logger;

        public BookController(IServiceWrapper serviceWrapper, ILoggerManager logger)
        {
            _serviceWrapper = serviceWrapper;
            _logger = logger;
        }

        public IEnumerable<BookModel> BooksList { get; set; } = new List<BookModel>();

        [HttpGet("index")]
        public async Task<IActionResult> Index()
        {
            //BookModel a = null;
            //var bm = a?.Author;

            BooksList = await _serviceWrapper.BookService.GetBookList();
            return Ok(BooksList);
        }
        [HttpGet("getBookbyId/{id}")]
        public async Task<IActionResult> GetBook(int? id)
        {
            if (id == null || id == 0)
            {
                _logger.LogError($"GetBook called without id passed");
                return BadRequest("Invalid id");
            }
            if (id > 0)
            {

                var Book = await _serviceWrapper.BookService.GetBookById(id.Value);
                if (Book == null)
                {
                    _logger.LogError($"Book with id: {id}, not found in db.");
                    return NotFound();
                }
                return Ok(Book);
            }
            return BadRequest("Invalid book id");
        }

        [HttpPost("upsert")]
        public async Task<IActionResult> Upsert([FromBody] BookModel model)
        {
            if (model == null)
            {
                _logger.LogError("BookModel object sent from client is null.");
                return BadRequest("BookModel object is null");
            }

            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid book object sent from client.");
                return BadRequest("Invalid model object");
            }

            if (model.Id > 0)
            {
                await _serviceWrapper.BookService.Update(model);
            }
            else
            {
                await _serviceWrapper.BookService.Create(model);
            }
            return Ok();
        }
    }
}