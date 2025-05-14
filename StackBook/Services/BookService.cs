using Microsoft.Identity.Client;
using StackBook.Data;
using StackBook.Interfaces;
using StackBook.Models;
using StackBook.ViewModels;

namespace StackBook.Services
{
    public class BookService : IBookService
    {
        private readonly ApplicationDbContext _context;

        private readonly IAuthorService _authorService;

        public BookService(ApplicationDbContext context, IAuthorService authorService)
        {
            _context = context;
            _authorService = authorService;
        }




    }
}
