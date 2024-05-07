using System.Data.SqlClient;
using kolokwium.Models;

namespace kolokwium.Repositories;

public class BookRepository : IBookRepository
{
    private IConfiguration _configuration;

    public BookRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    public async Task<bool> DoesAuthorExist(int id)
    {
        var query = "SELECT 1 FROM authors WHERE PK = @ID";

        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@ID", id);

        await connection.OpenAsync();

        var res = await command.ExecuteScalarAsync();

        return res is not null;
    }
    
    public async Task<Book> GetAuthor(int Idb)
    {
        var query = @"SELECT books.PK AS book_id, books.title, authors.first_name, authors.last_name FROM books JOIN books_authors ON books_authors.FK_book = books.PK JOIN authors ON books_authors.FK_author = authors.PK 
WHERE books.PK = @ID
";
        
        await using var connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using var command = new SqlCommand();

        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@ID", Idb);
        
        await connection.OpenAsync();

        var reader = await command.ExecuteReaderAsync();

        var firstName = reader.GetOrdinal("first_name");
        var lastNAme = reader.GetOrdinal("last_name");
        var title = reader.GetOrdinal("title");
        var id = reader.GetOrdinal("book_id");
        Book book = null;
        while (await reader.ReadAsync())
        {
            book = new Book()
            {
                ID = reader.GetInt32(id),
                Title = reader.GetString(title),
                Author = new Author()
                {
                    FirstName = reader.GetString(firstName),
                    LastName = reader.GetString(lastNAme)
                }
            };
        }

        if (book is null) throw new Exception();
        
        return book;
    }
    
    public async Task<int> AddNewBookWithAuthor(NewBook newBook)
    {
        var insert = @"INSERT INTO books VALUES(@title);SELECT @@IDENTITY AS ID;";
	    
        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();
	    
        command.Connection = connection;
        command.CommandText = insert;
	    
        command.Parameters.AddWithValue("@title", newBook.Title);

        await connection.OpenAsync();

        var transaction = await connection.BeginTransactionAsync();
        command.Transaction = transaction as SqlTransaction;
        
        try
        {
            //var book = await command.ExecuteScalarAsync();
            int bookId = Convert.ToInt32(await command.ExecuteScalarAsync());
            
                command.Parameters.Clear();
                command.CommandText = "INSERT INTO authors VALUES(@firstName, @LastName)";
                command.Parameters.AddWithValue("@firstName", newBook.Author.FirstName);
                command.Parameters.AddWithValue("@LastName", newBook.Author.LastName);

                await command.ExecuteNonQueryAsync();

                await transaction.CommitAsync();
                return bookId;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

}