using System.Linq.Expressions;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using Microsoft.EntityFrameworkCore;
using StackBook.DAL.IRepository;
using StackBook.Data;
using StackBook.Models;
using StackBook.ViewModels;
using static StackBook.ViewModels.BookWithAuthors;
namespace StackBook.DAL
{
    public class BookRepository : Repository<Book>, IBookRepository
    {
        private readonly ApplicationDbContext _db;
        public BookRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public IEnumerable<BookDetailViewModel> GetAllBookDetails()
        {
            return _db.Books
                .Include(b => b.BookAuthors!).ThenInclude(ba => ba.Author)
                .Include(b => b.BookCategories!).ThenInclude(bc => bc.Category)
                .Select(b => new BookDetailViewModel
                {
                    BookId = b.BookId,
                    BookTitle = b.BookTitle,
                    Description = b.Description,
                    Price = b.Price,
                    Stock = b.Stock,
                    ImageURL = b.ImageURL,
                    CreatedBook = b.CreatedBook,
                    AuthorsName = b.BookAuthors!.Select(ba => ba.Author!.AuthorName!).ToList(),
                    CategoryNames = b.BookCategories!.Select(bc => bc.Category!.CategoryName!).ToList()
                })
                .ToList();
        }

        public BookDetailViewModel? GetBookDetail(Guid? bookId)
        {
            return _db.Books
                .Include(b => b.BookAuthors!).ThenInclude(ba => ba.Author)
                .Include(b => b.BookCategories!).ThenInclude(bc => bc.Category)
                .Where(b => b.BookId == bookId)
                .Select(b => new BookDetailViewModel
                {
                    BookId = b.BookId,
                    BookTitle = b.BookTitle,
                    Description = b.Description,
                    Price = b.Price,
                    Stock = b.Stock,
                    ImageURL = b.ImageURL,
                    CreatedBook = b.CreatedBook,
                    AuthorsName = b.BookAuthors!.Select(ba => ba.Author!.AuthorName!).ToList(),
                    CategoryNames = b.BookCategories!.Select(bc => bc.Category!.CategoryName!).ToList()
                })
                .FirstOrDefault();
        }

        public List<BookWithAuthors> GetAllBookWithAuthor()
        {
            return _db.Books
                .Include(b => b.BookAuthors!)
                    .ThenInclude(ba => ba.Author)
                .Select(b => new BookWithAuthors
                {
                    Book = b,
                    Authors = b.BookAuthors!.Select(ba => ba.Author).ToList()
                })
                .ToList();
        }

        public BookWithAuthors? GetBookWithAuthor(Guid? bookId)
        {
            if (bookId == null)
                return null;

            var book = _db.Books
                .Include(b => b.BookAuthors!)
                    .ThenInclude(ba => ba.Author)
                .FirstOrDefault(b => b.BookId == bookId);

            if (book == null)
                return null;

            return new BookWithAuthors
            {
                Book = book,
                Authors = book.BookAuthors!.Select(ba => ba.Author).ToList()
            };
        }



        public void UpdateBookDetail(BookDetailViewModel bookUpdate)
        {
            var bookDetail = _db.Books
                .Include(b => b.BookAuthors)
                .Include(b => b.BookCategories)
                .FirstOrDefault(b => b.BookId == bookUpdate.BookId);

            if (bookDetail == null) return;

            bookDetail.BookTitle = bookUpdate.BookTitle;
            bookDetail.Description = bookUpdate.Description;
            bookDetail.Price = bookUpdate.Price;
            bookDetail.Stock = bookUpdate.Stock;
            if (bookDetail.ImageURL != null)
            {
                bookDetail.ImageURL = bookUpdate.ImageURL;
            }
            bookDetail.CreatedBook = bookUpdate.CreatedBook;

            // Cập nhật tác giả (Xóa tác giả cũ, thêm tác giả mới)
            bookDetail.BookAuthors!.Clear();
            var authors = _db.Authors.Where(a => bookUpdate.AuthorsName!.Contains(a.AuthorName!)).ToList();
            foreach (var author in authors)
            {
                bookDetail.BookAuthors.Add(new BookAuthor { BookId = bookDetail.BookId, AuthorId = author.AuthorId });
            }

            // Cập nhật thể loại
            bookDetail.BookCategories!.Clear();
            var categorys = _db.Categories.Where(c => bookUpdate.CategoryNames!.Contains(c.CategoryName!)).ToList();
            foreach (var category in categorys)
            {
                bookDetail.BookCategories.Add(new BookCategory { BookId = bookDetail.BookId, CategoryId = category.CategoryId });
            }

            _db.SaveChanges();
        }

        public void AddBookDetail(BookDetailViewModel bookAdd)
        {
            var newBook = new Book
            {
                BookId = Guid.NewGuid(),
                BookTitle = bookAdd.BookTitle,
                Description = bookAdd.Description,
                Price = bookAdd.Price,
                Stock = bookAdd.Stock,
                ImageURL = bookAdd.ImageURL,
                CreatedBook = bookAdd.CreatedBook
            };

            // Them sach vao DB  
            _db.Books.Add(newBook);
            _db.SaveChanges(); // Lưu để có id sử dụng cho bước tiếp theo  

            // Đưa vào bản trung gian BookAuthor  
            if (bookAdd.AuthorsName != null && bookAdd.AuthorsName.Any())
            {
                // Lấy danh sách Author có AuthorName trong bookAdd  
                var authors = _db.Authors
                    .Where(a => bookAdd.AuthorsName.Contains(a.AuthorName))
                    .ToList();

                // Đưa dữ liệu vào bảng trung gian  
                foreach (var author in authors)
                {
                    _db.BookAuthors.Add(new BookAuthor
                    {
                        BookId = newBook.BookId,
                        AuthorId = author.AuthorId
                    });
                }
            }

            // Thêm vào bảng trung gian BookCategory
            if (bookAdd.CategoryNames != null && bookAdd.CategoryNames.Any())
            {
                var categories = _db.Categories.Where(c => bookAdd.CategoryNames.Contains(c.CategoryName!)).ToList();
                foreach (var categoryItem in categories) // Renamed 'category' to 'categoryItem' to avoid conflict  
                {
                    _db.BookCategories.Add(new BookCategory
                    {
                        BookId = newBook.BookId,
                        CategoryId = categoryItem.CategoryId
                    });
                }
            }

            _db.SaveChanges();
        }

        public void DeleteBookDetail(BookDetailViewModel bookDelete)
        {
            // Xóa quan hệ giữa Book Và Category
            var bookCategory = _db.BookCategories.Where(bc => bc.BookId == bookDelete.BookId).ToList();
            if (bookCategory != null)
            {
                _db.BookCategories.RemoveRange(bookCategory);
            }

            // Xóa quan hệ giữa Book và Author
            var bookAuthors = _db.BookAuthors.Where(ba => ba.BookId == bookDelete.BookId).ToList();
            if (bookAuthors.Any())
            {
                _db.BookAuthors.RemoveRange(bookAuthors);
            }

            _db.SaveChanges();

            // Sách cần xóa
            var bookDel = _db.Books.FirstOrDefault(b => b.BookId == bookDelete.BookId);
            if (bookDel != null)
            {
                _db.Books.Remove(bookDel);
            }
        }


    }
}