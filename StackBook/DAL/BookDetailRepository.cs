using System.Linq.Expressions;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using Microsoft.EntityFrameworkCore;
using StackBook.DAL.IRepository;
using StackBook.Data;
using StackBook.Models;
using StackBook.ViewModels;

namespace StackBook.DAL
{
    public class BookDetailRepository : Repository<BookDetailViewModel>, IBookDetailRepository
    {
        private readonly ApplicationDbContext _db;
        public BookDetailRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public override IEnumerable<BookDetailViewModel> GetAll()
        {
            var books = _db.Books.ToList();

            return books.Select(b => Get(x => x.BookId == b.BookId)).Where(book => book != null).ToList();
        }




        public override BookDetailViewModel Get(Expression<Func<BookDetailViewModel, bool>> filter)
        {
            var query = _db.Books
                .Include(b => b.BookAuthors!)
                    .ThenInclude(ba => ba.Author)
                .Include(b => b.BookCategories!)
                    .ThenInclude(bc => bc.Category)
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
                    CategoryName = b.BookCategories!.Select(bc => bc.Category!.CategoryName).FirstOrDefault()
                });

            return query.FirstOrDefault(filter)!;
        }


        public void Update(BookDetailViewModel bookUpdate)
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
            if(bookDetail.ImageURL != null)
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
            var category = _db.Categories.FirstOrDefault(c => c.CategoryName == bookUpdate.CategoryName);
            if (category != null)
            {
                bookDetail.BookCategories.Add(new BookCategory { BookId = bookDetail.BookId, CategoryId = category.CategoryId });
            }

            _db.SaveChanges();
        }

        public override void Add(BookDetailViewModel bookAdd)
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

                // Thêm thể loại
                var category = _db.Categories.FirstOrDefault(c => c.CategoryName == bookAdd.CategoryName);
                if (category != null)
                {
                    _db.BookCategories.Add(new BookCategory
                    {
                        BookId = newBook.BookId,
                        CategoryId = category.CategoryId
                    });
                }

                _db.SaveChanges();
            }
        }

        public override void Remove(BookDetailViewModel bookDelete)
        {
            // Xóa quan hệ giữa Book Và Category
            var bookCategory = _db.BookCategories.FirstOrDefault(bc => bc.BookId == bookDelete.BookId);
            if (bookCategory != null)
            {
                _db.BookCategories.Remove(bookCategory);
            }

            // Xóa quan hệ giữa Book và Author
            var bookAuthors = _db.BookAuthors.Where(ba => ba.BookId == bookDelete.BookId).ToList();
            if (bookAuthors.Any())
            {
                _db.BookAuthors.RemoveRange(bookAuthors);
            }

            _db.SaveChanges();

            // Sách cần xóa
            var bookDel =  _db.Books.FirstOrDefault(b => b.BookId == bookDelete.BookId);
            if(bookDel != null)
            {
            _db.Books.Remove(bookDel);
            }
        }
    }
}
