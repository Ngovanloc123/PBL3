using System.Linq.Expressions;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using Microsoft.EntityFrameworkCore;
using StackBook.DAL.IRepository;
using StackBook.DAL.Repository;
using StackBook.Data;
using StackBook.Models;
using StackBook.ViewModels;
namespace StackBook.DAL
{
    public class BookRepository : Repository<Book>, IBookRepository
    {
        private readonly ApplicationDbContext _db;
        public BookRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        //public void Update(Book entity)
        //{
        //    _db.Books.Update(entity);
        //}
        //public IEnumerable<BookDetailViewModel> GetAllBookDetails()
        //{
        //    return _db.Books
        //        .Include(b => b.Authors)
        //        .Include(b => b.Categories)
        //        .Select(b => new BookDetailViewModel
        //        {
        //            BookId = b.BookId,
        //            BookTitle = b.BookTitle,
        //            Description = b.Description,
        //            Price = b.Price,
        //            Stock = b.Stock,
        //            ImageURL = b.ImageURL,
        //            CreatedBook = b.CreatedBook,
        //            AuthorsName = b.Authors.Select(a => a.AuthorName!).ToList(),
        //            CategoryNames = b.Categories.Select(c => c.CategoryName!).ToList()
        //        })
        //        .ToList();
        //}


        //public BookDetailViewModel? GetBookDetail(Guid? bookId)
        //{
        //    return _db.Books
        //        .Include(b => b.Authors)
        //        .Include(b => b.Categories!)
        //        .Where(b => b.BookId == bookId)
        //        .Select(b => new BookDetailViewModel
        //        {
        //            BookId = b.BookId,
        //            BookTitle = b.BookTitle,
        //            Description = b.Description,
        //            Price = b.Price,
        //            Stock = b.Stock,
        //            ImageURL = b.ImageURL,
        //            CreatedBook = b.CreatedBook,
        //            AuthorsName = b.Authors.Select(a => a.AuthorName!).ToList(),
        //            CategoryNames = b.Categories.Select(c => c.CategoryName!).ToList()
        //        })
        //        .FirstOrDefault();
        //    return null;
        //}

        //public List<BookWithAuthors> GetAllBookWithAuthor()
        //{
        //    return _db.Books
        //        .Include(b => b.Authors!)
        //        .Select(b => new BookWithAuthors
        //        {
        //            Book = b,
        //            Authors = b.Authors.ToList()
        //        })
        //        .ToList();
        //}

        //public BookWithAuthors? GetBookWithAuthor(Guid? bookId)
        //{
        //    if (bookId == null)
        //        return null;

        //    var book = _db.Books
        //        .Include(b => b.Authors)
        //        .FirstOrDefault(b => b.BookId == bookId);

        //    if (book == null)
        //        return null;

        //    return new BookWithAuthors
        //    {
        //        Book = book,
        //        Authors = book.Authors!.ToList()
        //    };
        //}



        //public void UpdateBookDetail(BookDetailViewModel bookUpdate)
        //{
        //    var bookDetail = _db.Books
        //        .Include(b => b.Authors)
        //        .Include(b => b.Categories)
        //        .FirstOrDefault(b => b.BookId == bookUpdate.BookId);

        //    if (bookDetail == null) return;

        //    // Cập nhật thông tin cơ bản
        //    bookDetail.BookTitle = bookUpdate.BookTitle;
        //    bookDetail.Description = bookUpdate.Description;
        //    bookDetail.Price = bookUpdate.Price;
        //    bookDetail.Stock = bookUpdate.Stock;
        //    if (bookUpdate.ImageURL != null)  // <-- Sửa: kiểm tra giá trị mới, không phải cũ
        //    {
        //        bookDetail.ImageURL = bookUpdate.ImageURL;
        //    }
        //    bookDetail.CreatedBook = bookUpdate.CreatedBook;

        //    // Cập nhật tác giả
        //    bookDetail.Authors.Clear(); // xóa danh sách cũ
        //    var authors = _db.Authors.Where(a => bookUpdate.AuthorsName!.Contains(a.AuthorName!)).ToList();
        //    foreach (var author in authors)
        //    {
        //        bookDetail.Authors.Add(author);
        //    }

        //    // Cập nhật thể loại
        //    bookDetail.Categories.Clear(); // xóa danh sách cũ
        //    var categories = _db.Categories.Where(c => bookUpdate.CategoryNames!.Contains(c.CategoryName!)).ToList();
        //    foreach (var category in categories)
        //    {
        //        bookDetail.Categories.Add(category);
        //    }

        //    //_db.SaveChanges();
        //}


        //public void AddBookDetail(BookDetailViewModel bookAdd)
        //{
        //    var newBook = new Book
        //    {
        //        BookId = Guid.NewGuid(),
        //        BookTitle = bookAdd.BookTitle,
        //        Description = bookAdd.Description,
        //        Price = bookAdd.Price,
        //        Stock = bookAdd.Stock,
        //        ImageURL = bookAdd.ImageURL,
        //        CreatedBook = bookAdd.CreatedBook
        //    };

        //    // Gán trực tiếp Authors nếu có
        //    if (bookAdd.AuthorsName?.Any() == true)
        //    {
        //        newBook.Authors = _db.Authors
        //            .Where(a => bookAdd.AuthorsName.Contains(a.AuthorName!))
        //            .ToList();
        //    }

        //    // Gán trực tiếp Categories nếu có
        //    if (bookAdd.CategoryNames?.Any() == true)
        //    {
        //        newBook.Categories = _db.Categories
        //            .Where(c => bookAdd.CategoryNames.Contains(c.CategoryName!))
        //            .ToList();
        //    }

        //    _db.Books.Add(newBook);
        //    _db.SaveChanges();
        //}


        //public void DeleteBookDetail(BookDetailViewModel bookDelete)
        //{
        //    var book = _db.Books
        //        .Include(b => b.Authors)
        //        .Include(b => b.Categories)
        //        .FirstOrDefault(b => b.BookId == bookDelete.BookId);

        //    if (book == null) return;

        //    // Xóa các liên kết với Authors và Categories (EF sẽ tự xử lý bảng trung gian)
        //    book.Authors.Clear();
        //    book.Categories.Clear();

        //    // Lưu thay đổi trước khi xóa book để tránh ràng buộc khóa ngoại
        //    _db.SaveChanges();

        //    // Xóa book chính
        //    _db.Books.Remove(book);
        //    _db.SaveChanges();
        //}



    }
}