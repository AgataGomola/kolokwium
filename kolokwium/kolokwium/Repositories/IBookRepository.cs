using kolokwium.Models;

namespace kolokwium.Repositories;

public interface IBookRepository
{
    public Task<Book> GetAuthor(int authorId);
    public Task<bool> DoesAuthorExist(int id);
    public Task<int> AddNewBookWithAuthor(NewBook newBook);
}