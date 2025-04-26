using ClosedXML.Excel;
using DocumentFormat.OpenXml.InkML;
using Microsoft.EntityFrameworkCore;
using StackBook.Models;
using System;

namespace StackBook.Data
{
    public class AddData
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using var dbContext = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>());

            if (!dbContext.Categories.Any()) // Kiểm tra nếu bảng rỗng
            {
                dbContext.Categories.AddRange(new List<Category>
                {
                    new Category { CategoryName = "Literature & Fiction", DisplayOrder = 1 },
                    new Category { CategoryName = "Science & Math",  DisplayOrder = 2},
                    new Category { CategoryName = "Mystery, Thriller & Suspense" , DisplayOrder = 3},
                    new Category { CategoryName = "Business & Money", DisplayOrder = 4},
                    new Category { CategoryName = "Computers & Technology" , DisplayOrder = 5},
                    new Category { CategoryName = "Self-Help", DisplayOrder = 6 },
                    new Category { CategoryName = "Health, Fitness & Dieting" , DisplayOrder = 7},
                    new Category { CategoryName = "Science Fiction & Fantasy" , DisplayOrder = 8},
                });
                dbContext.SaveChanges();
                Console.WriteLine("✅ Đã thêm dữ liệu vào database!");
            }
            if (!dbContext.Authors.Any())
            {
                dbContext.Authors.AddRange(new List<Author>
                {
                    new Author { AuthorName = "Admiral William H. McRaven" },
                    new Author { AuthorName = "Agatha Christie" },
                    new Author { AuthorName = "Albert R Meyer" },
                    new Author { AuthorName = "Alex Hope" },
                    new Author { AuthorName = "Alex Wiltshire" },
                    new Author { AuthorName = "Ariel Lawhon" },
                    new Author { AuthorName = "Arthur Conan Doyle" },
                    new Author { AuthorName = "Bessel van der Kolk M.D" },
                    new Author { AuthorName = "Bill Gifford" },
                    new Author { AuthorName = "Calley Means" },
                    new Author { AuthorName = "Callie Hart" },
                    new Author { AuthorName = "Carl J. Pratt" },
                    new Author { AuthorName = "Casey Means MD" },
                    new Author { AuthorName = "Chris Guillebeau" },
                    new Author { AuthorName = "Dan Brown" },
                    new Author { AuthorName = "Daniel Casanave" },
                    new Author { AuthorName = "David Goggins" },
                    new Author { AuthorName = "David Vandermeulen" },
                    new Author { AuthorName = "Denis Rothman" },
                    new Author { AuthorName = "Eric Lehman" },
                    new Author { AuthorName = "Erik Gross" },
                    new Author { AuthorName = "Ernest Hemingway" },
                    new Author { AuthorName = "F Thomson Leighton" },
                    new Author { AuthorName = "Gene Kim" },
                    new Author { AuthorName = "George S. Clason" },
                    new Author { AuthorName = "George Spafford" },
                    new Author { AuthorName = "Hector Malot" },
                    new Author { AuthorName = "Heidi Murkoff" },
                    new Author { AuthorName = "Higashino Keigo" },
                    new Author { AuthorName = "Ho Chi Minh" },
                    new Author { AuthorName = "Homer" },
                    new Author { AuthorName = "Jack Stanley" },
                    new Author { AuthorName = "James Bernstein" },
                    new Author { AuthorName = "James Clear" },
                    new Author { AuthorName = "James Holler" },
                    new Author { AuthorName = "Jasmine Mas" },
                    new Author { AuthorName = "Jennifer Campbell" },
                    new Author { AuthorName = "Jesper Juul" },
                    new Author { AuthorName = "JIM MURPHY" },
                    new Author { AuthorName = "Joan Frese" },
                    new Author { AuthorName = "Jonathan Haidt" },
                    new Author { AuthorName = "Jonathan Katz" },
                    new Author { AuthorName = "Jonathan Levy" },
                    new Author { AuthorName = "Joseph Nguyen" },
                    new Author { AuthorName = "Kanae Minato" },
                    new Author { AuthorName = "Kenneth Rosen" },
                    new Author { AuthorName = "Kevin Behr" },
                    new Author { AuthorName = "Margarel Mitchell" },
                    new Author { AuthorName = "Mark Ciampa" },
                    new Author { AuthorName = "Mark Manson" },
                    new Author { AuthorName = "Martin Daunton" },
                    new Author { AuthorName = "Martin Wolf" },
                    new Author { AuthorName = "Mary Claire Haver MD" },
                    new Author { AuthorName = "Matt Dinniman" },
                    new Author { AuthorName = "Megan Logan MSW LCSW" },
                    new Author { AuthorName = "Mel Robbins" },
                    new Author { AuthorName = "Michael Jackson" },
                    new Author { AuthorName = "Michael Sipser" },
                    new Author { AuthorName = "Morgan Housel" },
                    new Author { AuthorName = "Napoleon Hill" },
                    new Author { AuthorName = "Nikolai A. Ostrovsky" },
                    new Author { AuthorName = "Nguyen Du" },
                    new Author { AuthorName = "Nguyen Nhat Anh" },
                    new Author { AuthorName = "Penelope Sky" },
                    new Author { AuthorName = "Peter Attia MD" },
                    new Author { AuthorName = "Phoenix Books" },
                    new Author { AuthorName = "Rachel Ignotofsky" },
                    new Author { AuthorName = "Ray Bradbury" },
                    new Author { AuthorName = "Rebecca Yarros" },
                    new Author { AuthorName = "Robert F. Kennedy Jr" },
                    new Author { AuthorName = "Robert T. Kiyosaki" },
                    new Author { AuthorName = "Sarah J. Maas" },
                    new Author { AuthorName = "Shoo Rayner" },
                    new Author { AuthorName = "Stephen R. Covey" },
                    new Author { AuthorName = "Stephen W. Hawking" },
                    new Author { AuthorName = "Steven Freund" },
                    new Author { AuthorName = "Suzanne Collins" },
                    new Author { AuthorName = "The Tech Academy" },
                    new Author { AuthorName = "Thomas Harris" },
                    new Author { AuthorName = "Thomas Sowell" },
                    new Author { AuthorName = "Various" },
                    new Author { AuthorName = "Victor Hugo" },
                    new Author { AuthorName = "Walter Isaacson" },
                    new Author { AuthorName = "Yehuda Lindell" },
                    new Author { AuthorName = "Yuval Noah Harari" },
                    new Author { AuthorName = "Ph.D"},
                });
                dbContext.SaveChanges();
                Console.WriteLine("✅ Đã thêm dữ liệu vào database!");
            }

            string filePath = "D:\\book23-8.xlsx";
            if (!File.Exists(filePath))
            {
                Console.WriteLine("❌ File Excel không tồn tại!");
                return;
            }
            using var workbook = new XLWorkbook(filePath);
            if (!dbContext.Books.Any())
            {
                var worksheet = workbook.Worksheet(1); // Lấy sheet đầu tiên
                var rows = worksheet.RowsUsed(); // Lấy tất cả các dòng có dữ liệu

                foreach (var row in rows.Skip(1)) // Bỏ dòng tiêu đề
                {
                    var check = dbContext.Books.Local.FirstOrDefault(ba => ba.BookTitle == row.Cell(1).GetString());
                    if (check != null)
                    {
                        continue;
                    }
                    var author = row.Cell(2).GetString();
                    string[] authors = author.Split(',');

                    List<Author> author2 = new List<Author>();
                    foreach (var authorName in authors)
                    {
                        var author1 = await dbContext.Authors.FirstOrDefaultAsync(a => a.AuthorName == authorName.Trim());
                        if (author1 != null) // Kiểm tra tác giả có tồn tại
                        {
                            author2.Add(author1);
                        }
                    }
                    var category = row.Cell(3).GetString();
                    string[] categories = category.Split("|");
                    List<Category> categories2 = new List<Category>();
                    foreach (var categoryName in categories)
                    {
                        var category1 = await dbContext.Categories.FirstOrDefaultAsync(a => a.CategoryName == categoryName);
                        if (category1 != null)
                        {
                            categories2.Add(category1);
                        }
                    }


                    dbContext.Books.Add(new Book
                    {
                        BookId = Guid.NewGuid(),
                        BookTitle = row.Cell(1).GetString(),
                        Price = row.Cell(4).GetDouble(),
                        Stock = 1000,
                        CreatedBook = row.Cell(6).GetDateTime(),
                        ImageURL = row.Cell(5).GetString(),
                        Description = row.Cell(7).GetString(),
                        Authors = author2,
                        Categories = categories2,
                    });
                }
                await dbContext.SaveChangesAsync();
            }
            if (!dbContext.Users.Any())
            {
                var worksheet3 = workbook.Worksheet(3);
                var rowUsers = worksheet3.RowsUsed();

                foreach (var user in rowUsers.Skip(1))
                {
                    // Role, LockStatus, IsEmailVerified: kiểu bool (có thể là 1/0 trong file)
                    bool role = user.Cell(4).GetValue<int>() == 1;
                    bool lockStatus = user.Cell(5).GetValue<int>() == 1;
                    bool isEmailVerified = user.Cell(6).GetValue<int>() == 1;

                    dbContext.Users.Add(new User
                    {
                        UserId = Guid.NewGuid(),
                        FullName = user.Cell(1).GetString(),
                        Email = user.Cell(2).GetString(),
                        Password = user.Cell(3).GetString(),
                        Role = role,
                        CreatedUser = DateTime.Now,
                        ResetPasswordToken = "",
                        ResetTokenExpiry = DateTime.Now,
                        LockStatus = lockStatus,
                        IsEmailVerified = isEmailVerified,
                    });
                }
                await dbContext.SaveChangesAsync();
            }
            if (!dbContext.Reviews.Any())
            {
                var worksheet2 = workbook.Worksheet(2);
                var rowReviews = worksheet2.RowsUsed();
                foreach (var rowReview in rowReviews.Skip(1))
                {
                    // Tìm user theo Email
                    var userEmail = rowReview.Cell(1).GetString().Trim();
                    var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
                    if (user == null) continue; // Nếu không tìm thấy user thì bỏ qua dòng này

                    // Tìm book theo tiêu đề
                    var bookTitle = rowReview.Cell(2).GetString().Trim();
                    var book = await dbContext.Books.FirstOrDefaultAsync(b => b.BookTitle == bookTitle);
                    if (book == null) continue; // Nếu không tìm thấy sách thì bỏ qua

                    // Thêm review
                    dbContext.Reviews.Add(new Review
                    {
                        ReviewId = Guid.NewGuid(),
                        UserId = user.UserId,
                        BookId = book.BookId,
                        Rating = rowReview.Cell(3).GetValue<int>(), // Rating là số nguyên
                        Comment = rowReview.Cell(4).GetString().Trim()
                    });
                }
                await dbContext.SaveChangesAsync();
            }
        }
    }
}
