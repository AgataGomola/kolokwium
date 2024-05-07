using kolokwium.Models;
using kolokwium.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace kolokwium.Controllers;

[ApiController]
[Route("api/[controller]")]
public class booksController : ControllerBase
{
    private IBookRepository _bookRepository;

    public booksController(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetAuthor(int id)
    {
        if (!await _bookRepository.DoesAuthorExist(id))
        {
            return NotFound($"Author with id: {id} does not exist");
        }
        var author = await _bookRepository.GetAuthor(id);
        return Ok(author);
    }
    [HttpPost]
    public async Task<IActionResult> AddBook(NewBook newBook)
    {
        var id = await _bookRepository.AddNewBookWithAuthor(newBook);
        return Created(Request.Path.Value ?? "api/books", newBook);
    }
}