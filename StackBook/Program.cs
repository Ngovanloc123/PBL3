using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using StackBook.DAL;
using StackBook.DAL.IRepository;
using StackBook.Data;
using StackBook.Models;
using StackBook.Services;
using StackBook.Utils;
using Microsoft.AspNetCore.Builder;
using StackBook.Configurations;
using StackBook.DAL.Repository;
using StackBook.Interfaces;
using StackBook.Controllers;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpContextAccessor();

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<BookService>();
builder.Services.AddScoped<CategoryService>();
builder.Services.AddScoped<AuthorService>();
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
builder.Services.AddScoped<OAuthGoogleService>();
builder.Services.AddScoped<JwtUtils>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<AccountController>();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddScoped<EMailUtils>();

var app = builder.Build();

//builder.Services.AddHttpContextAccessor();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();



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
    if (!dbContext.Books.Any()) // Kiểm tra nếu bảng rỗng
    {
        dbContext.Books.AddRange(new List<Book>
        {
            // Thể loại Literature & Fiction
            new Book
            {
                BookTitle = "Give Me a Ticket to Childhood",
                Price = 4.00,
                Stock = 100,
                CreatedBook = new DateTime(2008, 2, 1),
                ImageURL = "https://res.cloudinary.com/dhik9tniv/image/upload/v1740818425/uploaded_images/cddwg4theevsfzyawf8n.jpg",
                Description = "\"\"\"Give Me a Ticket to Childhood\"\" (Cho tôi xin một vé đi tuổi thơ) is a famous novel by Nguyễn Nhật Ánh, telling a nostalgic story of childhood filled with innocence, mischief, and life lessons.                                                                                                                                                                                                      1. Summary\r\nThe novel follows the story of Mùi, an adult reminiscing about his childhood with his close friends Tí, Hải, Tủn, and their dog Liêm. Through their innocent yet imaginative pranks and adventures, Mùi gradually realizes the differences between childhood and adulthood, as well as the inevitable changes brought by time The story not only captures the joy and purity of childhood but also raises deep questions about the meaning of life, growing up, and nostalgia.\r\n2. Chapters and Main Content\r\nAlthough the book does not follow a traditional chapter structure, it can be divided into key sections:\r\nPart 1: Introducing the Group of Friends and Their Innocent Childhood\r\nThe protagonist Mùi recalls his childhood with Tí, Hải, Tủn, and their dog Liêm.\r\nThey engage in creative and mischievous games, such as establishing a \"\"children’s government\"\" or experimenting with their own ideas about life.\r\nPart 2: Naïve Yet Profound Thoughts\r\nThe characters begin to notice the differences between children and adults.\r\nChildren are curious and carefree, while adults are bound by responsibilities and societal expectations.\r\nMùi wonders: \"\"Why don’t adults play like children?\"\"\r\nPart 3: Loss and the Lessons of Growing Up\r\nAs they grow older, Mùi and his friends drift apart, and their childhood games lose their charm.\r\nThey realize that childhood is something that cannot be revisited—only remembered.\r\nThe story concludes with a lingering sense of nostalgia and longing for the past.\r\n3. Character Thoughts and Psychology\r\nMùi (Main Character and Narrator)\r\nAs a child: Curious, imaginative, and free-spirited, always engaging in mischief.\r\nAs an adult: Nostalgic about childhood, realizing how adulthood often strips people of their innocence and creativity.\r\nTí, Hải, Tủn (Mùi’s Childhood Friends)\r\nEach friend has their own personality but collectively represent the carefree and playful nature of children.\r\nThey share beautiful memories with Mùi but eventually change as they grow up.\r\nAdults (Parents, Teachers, and Society)\r\nRepresent the stark contrast between childhood and adulthood: children are full of wonder, while adults are weighed down by responsibilities.\r\nChildren see adults as serious, unimaginative, and distant from the joy of life.\r\n4. Themes and Messages\r\nChildhood is a beautiful but irretrievable part of life.\r\nAdults can learn from children how to see the world with simplicity and joy.\r\nGrowing up involves inevitable loss, but memories remain a vital part of our lives.\""
            },
            new Book
            {
                BookTitle = "Yellow Flowers on the Green Grass",
                Price = 5.00,
                Stock = 100,
                CreatedBook = new DateTime(2010, 12, 9),
                ImageURL = "https://res.cloudinary.com/dhik9tniv/image/upload/v1740818437/uploaded_images/rtx7mrm3pkb5mqqepdla.jpg",
                Description = "\"\"\"Yellow Flowers on the Green Grass\"\" (Tôi thấy hoa vàng trên cỏ xanh) by Nguyễn Nhật Ánh is a coming-of-age novel set in rural Vietnam in the 1980s. The story follows Thiều, a young boy navigating childhood with his younger brother Tường. Despite their deep bond, jealousy and misunderstandings arise, especially when Thiều feels overshadowed by Tường's kindness and innocence. Their lives take a dramatic turn when a tragic accident tests their relationship, forcing Thiều to confront his guilt and emotions.\r\n\r\nThe novel beautifully portrays brotherhood, childhood dreams, and the struggles of growing up, while also reflecting the hardships of rural life. With poetic descriptions of Vietnam’s countryside, it captures the innocence, conflicts, and warmth of youth, making it a nostalgic and emotional journey.\""
            },
            new Book
            {
                BookTitle = "The Blue Eyes",
                Price = 10.00,
                Stock = 100,
                CreatedBook = new DateTime(1990, 3, 24),
                ImageURL = "https://res.cloudinary.com/dhik9tniv/image/upload/v1740818430/uploaded_images/diy7hfa4bwgcrrha0ipb.jpg",
                Description = "\"\"\"The Blue Eyes\"\" (Mắt Biếc) by Nguyễn Nhật Ánh is a poignant coming-of-age novel about unrequited love, dreams, and personal growth. The story follows Ngạn, a sensitive and artistic boy, who falls deeply in love with Hà Lan, a beautiful girl from his village. As they grow up, Hà Lan becomes enchanted by the bustling city life, leaving Ngạn behind with his quiet admiration and heartbreak. Despite his unwavering love, Hà Lan chooses Dũng, a wealthy and reckless city boy, leading to years of pain and longing for Ngạn.\r\n\r\nChapter Structure:\r\nChildhood in the Village – Ngạn and Hà Lan’s innocent friendship and bond in their peaceful hometown.\r\nCity Life and Changes – Hà Lan moves to the city and gradually distances herself from Ngạn.\r\nHeartbreak and Sacrifice – Hà Lan’s struggles, her relationship with Dũng, and Ngạn’s silent suffering.\r\nA Love That Never Fades – Ngạn continues to care for Hà Lan’s daughter, Trà Long, finding solace in an unfulfilled love.\r\nThe novel beautifully captures the innocence of first love, the pain of longing, and the acceptance of reality, leaving readers with a sense of nostalgia and bittersweet emotions.\""
            },
            new Book
            {
                BookTitle = "How the Steel Was Tempered",
                Price = 15.00,
                Stock = 100,
                CreatedBook = new DateTime(1936, 5, 1),
                ImageURL = "https://res.cloudinary.com/dhik9tniv/image/upload/v1740818432/uploaded_images/p8hej0kpmxwrnem4uaaa.jpg",
                Description = "\"\"\"How the Steel Was Tempered\"\" (Thép đã tôi thế đấy) by Nikolai Ostrovsky is a Soviet classic that tells the inspiring story of Pavel Korchagin, a young revolutionary who overcomes immense hardships in his fight for communism.\r\n\r\nSet during the Russian Civil War and the early Soviet era, the novel follows Pavel from his difficult childhood to becoming a dedicated Bolshevik soldier. Despite enduring war, imprisonment, and severe illness, he remains unbreakable in his commitment to his ideals. Even when he becomes paralyzed, Pavel continues his revolutionary work by writing.\r\n\r\nThe novel highlights themes of perseverance, sacrifice, and ideological devotion, portraying the forging of one's character—like steel—through hardship and struggle.\""
            },
            new Book
            {
                BookTitle = "Les Misérables",
                Price = 21.00,
                Stock = 100,
                CreatedBook = new DateTime(1862, 3, 31),
                ImageURL = "https://res.cloudinary.com/dhik9tniv/image/upload/v1740818431/uploaded_images/torx2vnkeejokrocoa3p.jpg",
                Description = "\"\"\"Les Misérables\"\" (Những Người Khốn Khổ) by Victor Hugo is a masterpiece of French literature that explores themes of justice, redemption, and love.\r\n\r\nThe novel follows Jean Valjean, a former convict who, after being shown kindness by a bishop, seeks to rebuild his life and become an honest man. However, he is relentlessly pursued by Inspector Javert, who believes in absolute justice. Along the way, Valjean cares for Cosette, the orphaned daughter of Fantine, a struggling factory worker. As Cosette grows up, she falls in love with Marius, a young revolutionary, leading to dramatic events during the June Rebellion in Paris.\r\n\r\nThrough its deeply emotional narrative, the novel portrays the struggles of the poor and oppressed while emphasizing compassion, sacrifice, and the power of redemption.\""
            },
            new Book
            {
                BookTitle = "The Tale of Kieu",
                Price = 15.9,
                Stock = 100,
                CreatedBook = new DateTime(1820, 1, 1),
                ImageURL = "https://res.cloudinary.com/dhik9tniv/image/upload/v1740818439/uploaded_images/z3zcggjngvyz2qgsydmu.jpg",
                Description = "\"\"\"The Tale of Kieu\"\" (Truyện Kiều) by Nguyễn Du is a Vietnamese literary masterpiece that tells the tragic yet heroic story of Thúy Kiều, a talented and beautiful young woman.\r\n\r\nKiều falls in love with Kim Trọng, but their happiness is shattered when her family faces a wrongful accusation. To save them, Kiều sacrifices herself, selling into marriage but ultimately falling into a life of betrayal, suffering, and hardship. She endures years of misfortune, being deceived, forced into prostitution, and abused. Despite her struggles, Kiều remains resilient, and after many trials, she is finally reunited with her family and Kim Trọng, though their love can never return to what it once was.\r\n\r\nThe poem is a profound reflection on fate, morality, sacrifice, and human resilience, showcasing Nguyễn Du’s deep understanding of human suffering and the impermanence of life.\""
            },
            new Book
            {
                BookTitle = "Sans Famille",
                Price = 22.9,
                Stock = 100,
                CreatedBook = new DateTime(1878, 1, 1),
                ImageURL = "https://res.cloudinary.com/dhik9tniv/image/upload/v1740818427/uploaded_images/rivzcrs38u9ouxrqw3tn.jpg",
                Description = "\"\"\"Sans Famille\"\" (Không Gia Đình) by Hector Malot is a classic French novel about Rémi, an orphan who is sold to a traveling street performer, Vitalis.\r\n\r\nUnder Vitalis' guidance, Rémi learns music and performance while experiencing the hardships of life on the road. Along the way, he encounters kindness, betrayal, and struggles to find his true family. After many adventures, losses, and challenges, Rémi eventually discovers his real origins and is reunited with his biological family.\r\n\r\nThe novel beautifully portrays themes of resilience, friendship, and the search for belonging, emphasizing the value of kindness and perseverance in overcoming adversity.\r\n\""
            },
            new Book
            {
                BookTitle = "Gone with the Wind",
                Price = 13.78,
                Stock = 100,
                CreatedBook = new DateTime(1936, 6, 30),
                ImageURL = "https://res.cloudinary.com/dhik9tniv/image/upload/v1740818426/uploaded_images/uizao33dygtae4p0ncpo.jpg",
                Description = "\"\"\"Gone with the Wind\"\" (Cuốn theo chiều gió) by Margaret Mitchell is a historical novel set during the American Civil War and Reconstruction era, following the life of Scarlett O’Hara, a strong-willed Southern belle.\r\n\r\nScarlett is deeply in love with Ashley Wilkes, but he marries Melanie Hamilton, forcing her into a series of turbulent relationships. As war devastates the South, Scarlett struggles to survive, using her intelligence and determination to protect her family and restore her plantation, Tara. She eventually marries the charming but cynical Rhett Butler, yet her obsession with Ashley leads to heartbreak. By the time she realizes her true feelings for Rhett, he has grown weary and leaves her with the famous words, \"\"Frankly, my dear, I don’t give a damn.\"\"\r\n\r\nThe novel explores themes of love, survival, loss, and change, portraying Scarlett as a symbol of resilience amid the downfall of the Old South.\""
            },
            new Book
            {
                BookTitle = "The Old Man and the Sea",
                Price = 12.59,
                Stock = 100,
                CreatedBook = new DateTime(1852, 9, 1),
                ImageURL = "https://res.cloudinary.com/dhik9tniv/image/upload/v1740818428/uploaded_images/evjioc8zc0colkfjx8zo.jpg",
                Description = "\"\"\"The Old Man and the Sea\"\" (Ông già và biển cả) by Ernest Hemingway is a timeless tale of perseverance and resilience.\r\n\r\nThe story follows Santiago, an aging Cuban fisherman who has gone 84 days without catching a fish. Determined to break his bad luck, he sails far into the Gulf Stream and hooks a giant marlin. After an exhausting three-day struggle, he finally kills the fish, but on his way back, sharks attack and devour his prize, leaving only the skeleton. Though he returns empty-handed, Santiago remains undefeated in spirit, embodying Hemingway’s theme of man’s struggle against nature and the indomitable will to endure.\""
            },
            new Book
            {
                BookTitle = "The Prison Diary of Ho Chi Minh",
                Price = 5.38,
                Stock = 100,
                CreatedBook = new DateTime(1943, 9, 10),
                ImageURL = "https://res.cloudinary.com/dhik9tniv/image/upload/v1740818431/uploaded_images/jqq6ngyjtaxuj1kznevz.jpg",
                Description = "\"\"\"The Prison Diary of Ho Chi Minh\"\" (Nhật Ký Trong Tù) is a powerful collection of poems written by President Hồ Chí Minh during his imprisonment in China (1942-1943). Composed in classical Chinese verse, the diary reflects his resilience, patriotism, and unbreakable revolutionary spirit.\r\n\r\nThrough over 130 poems, Hồ Chí Minh vividly depicts the harsh realities of prison life—hunger, cold, exhaustion, and injustice—while maintaining an unwavering belief in freedom and revolution. Despite suffering, he finds strength in the beauty of nature and the hope of national liberation. His words convey courage, wisdom, and a deep love for Vietnam, inspiring generations to fight for independence.\r\n\r\nMore than just a record of hardship, The Prison Diary is a testament to the indomitable spirit of a leader who, even in chains, never ceased to dream of a free and sovereign nation.\""
            },


            // Thể loại Science & Math
            new Book
            {
                BookTitle = "introduction theory of computation",
                Price = 100.00,
                Stock = 100,
                CreatedBook = new DateTime(1997, 1, 1),
                ImageURL = "https://res.cloudinary.com/dhik9tniv/image/upload/v1740717316/uploaded_images/obrvgihyyczoslrh4moo.jpg",
                Description = "\"Gain a clear understanding of even the most complex, highly theoretical computational theory topics in the approachable presentation found only in the market-leading INTRODUCTION TO THE THEORY OF COMPUTATION, 3E. The number one choice for today''s computational theory course, this revision continues the book''s well-know, approachable style with timely revisions, additional practice, and more memorable examples in key areas. A new first-of-its-kind theoretical treatment of deterministic context-free languages is ideal for a better understanding of parsing and LR(k) grammars. You gain a solid understanding of the fundamental mathematical properties of computer hardware, software, and applications with a blend of practical and philosophical coverage and mathematical treatments, including advanced theorems and proofs. INTRODUCTION TO THE THEORY OF COMPUTATION, 3E''s comprehensive coverage makes this a valuable reference for your continued studies in theoretical computing.\r\n\""
            },
            new Book
            {
                BookTitle = "Introduction to Modern Cryptography",
                Price = 110.00,
                Stock = 100,
                CreatedBook = new DateTime(2007, 8, 31),
                ImageURL = "https://res.cloudinary.com/dhik9tniv/image/upload/v1740717318/uploaded_images/rqnbvpdkqq5njbofpmqr.jpg",
                Description = "Now the most used texbook for introductory cryptography courses in both mathematics and computer science, the Third Edition builds upon previous editions by offering several new sections, topics, and exercises. The authors present the core principles of modern cryptography, with emphasis on formal definitions, rigorous proofs of security."
            },
            new Book
            {
                BookTitle = "Discrete Mathematics and Its Applications",
                Price = 87.00,
                Stock = 100,
                CreatedBook = new DateTime(2011, 1, 1),
                ImageURL = "https://res.cloudinary.com/dhik9tniv/image/upload/v1740717328/uploaded_images/uojcihlww7btszuif5n5.jpg",
                Description = "ISBN: 9781260091991 is an International Student Edition of Discrete Mathematics and Its Applications 8th Edition By Kenneth Rosen This ISBN: 9781260091991 is student textbook only. It will not come with online access code. Online Access code sold separately at ISBN: 9781259731242 The content of this is the same on other formats. Rosen's Discrete Mathematics and its Applications presents a precise, relevant, comprehensive approach to mathematical concepts. This world-renowned best-selling text was written to accommodate the needs across a variety of majors and departments, including mathematics, computer science, and engineering. As the market leader, the book is highly flexible, comprehensive and a proven pedagogical teaching tool for instructors. Digital is becoming increasingly important and gaining popularity, crowning Connect as the digital leader for this discipline. McGraw-Hill Education's Connect, available as an optional add on item, is the only integrated learning system that empowers students by continuously adapting to deliver precisely what they need, when they need it and how they need it - ensuring class time is more effective. Connect allows the professor to assign homework, quizzes, and tests easily and automatically grades and records the scores of the student's work. Problems are randomized to prevent sharing of answers and may also have a \"multi-step solution\" which helps move the students' learning along if they experience difficulty."
            },
            new Book
            {
                BookTitle = "Mathematics for Computer Science",
                Price = 45.00,
                Stock = 100,
                CreatedBook = new DateTime(2018, 6, 6),
                ImageURL = "https://res.cloudinary.com/dhik9tniv/image/upload/v1740717329/uploaded_images/prgofemwmaianhj3lyil.jpg",
                Description = "\"This book covers elementary discrete mathematics for computer science and engineering. It emphasizes mathematical definitions and proofs as well as applicable methods. Topics include formal logic notation, proof methods; induction, well-ordering; sets, relations; elementary graph theory; integer congruences; asymptotic notation and growth of functions; permutations and combinations, counting principles; discrete probability. Further selected topics may also be covered, such as recursive definition and structural induction; state machines and invariants; recurrences; generating functions.\r\n\r\nThe color images and text in this book have been converted to grayscale.\""
            },
            new Book
            {
                BookTitle = "Euclid: The Man Who Invented Geometry (Mega Minds)",
                Price = 8.00,
                Stock = 100,
                CreatedBook = new DateTime(2017, 11, 2),
                ImageURL = "https://res.cloudinary.com/dhik9tniv/image/upload/v1740717321/uploaded_images/tduucwhkhdlmtijfij57.jpg",
                Description = "\"This book brings geometry to life with Euclid explaining the principles of Geometry to his friends. Full of fun, explanation and even jokes, thei is the perfect introduction to a sometimes tricky subject.\r\n\r\nEuclid lived 2300 years ago in Alexandria, in northern Egypt. His was a brilliant mind. He devised a method of learning Geometry starting from the simplest idea - an Axiom - something we can all agree is self-evident. Euclid built argument upon argument, creating a brillaintly simple system for learning Geometry which he wrote down in his book - Euclid's Elements - which was still in everyday use in schools well into the 20th century.\r\n\r\nShoo Rayner brings another Mega-mind to life with wit, clarity and sensitivity. For any age that wants to get to grips with geometry.\r\n\""
            },
            new Book
            {
                BookTitle = "Sapiens: A Brief History of Humankind",
                Price = 14.29,
                Stock = 100,
                CreatedBook = new DateTime(2015, 5, 1),
                ImageURL = "https://res.cloudinary.com/dhik9tniv/image/upload/v1740717322/uploaded_images/cvkpso24ienfemqbuevp.jpg",
                Description = "\"From a renowned historian comes a groundbreaking narrative of humanity's creation and evolution - a number one international best seller - that explores the ways in which biology and history have defined us and enhanced our understanding of what it means to be \"\"human\"\".\r\n\r\nOne hundred thousand years ago, at least six different species of humans inhabited Earth. Yet today there is only one - Homo sapiens. What happened to the others? And what may happen to us?\r\n\r\nMost books about the history of humanity pursue either a historical or a biological approach, but Dr. Yuval Noah Harari breaks the mold with this highly original book that begins about 70,000 years ago, with the appearance of modern cognition. From examining the role evolving humans have played in the global ecosystem to charting the rise of empires, Sapiens integrates history and science to reconsider accepted narratives, connect past developments with contemporary concerns, and examine specific events within the context of larger ideas.\r\n\r\nDr. Harari also compels us to look ahead, because, over the last few decades, humans have begun to bend laws of natural selection that have governed life for the past four billion years. We are acquiring the ability to design not only the world around us but also ourselves. Where is this leading us, and what do we want to become?\r\n\r\nThis provocative and insightful work is sure to spark debate and is essential for aficionados of Jared Diamond, James Gleick, Matt Ridley, Robert Wright, and Sharon Moalem.\""
            },
            new Book
            {
                BookTitle = "A Brief History of Time: From the Big Bang to Black Holes",
                Price = 34.70,
                Stock = 100,
                CreatedBook = new DateTime(1988, 3, 1),
                ImageURL = "https://res.cloudinary.com/dhik9tniv/image/upload/v1740717326/uploaded_images/wsqc5w8yqecbcuodddrh.jpg",
                Description = "\"One of the greatest minds of our time explores profound questions such as: How did the universe begin — and what made its start possible? Does time always flow forward? Is the universe unending — or are there boundaries? Are there other dimensions in space? What will happen when it all ends?\r\n\r\nTold in language we all can understand, A Brief History of Time plunges into the exotic realms of black holes and quarks, of antimatter and “arrows of time,” of the big bang and a bigger God — where the possibilities are wondrous and unexpected. Stephen Hawking brings us closer to the ultimate secrets at the very heart of creation.\r\n\r\n©2011 Stephen Hawking (P)2005 Phoenix Books, Inc.\""
            },
            new Book
            {
                BookTitle = "Albert Einstein: The Man, the Genius, and the Theory of Relativity (Pioneers of Science)",
                Price = 22.35,
                Stock = 100,
                CreatedBook = new DateTime(2021, 7, 30),
                ImageURL = "https://res.cloudinary.com/dhik9tniv/image/upload/v1740717320/uploaded_images/ylki6i4iqdxxmy19i0cv.jpg",
                Description = "Even the youngest science enthusiasts know the name \"Einstein.\" To them, it represents intelligence and ingenuity. But they may not know much about Albert Einstein as a man and why his fame reached such great heights. In this comprehensive biography, which draws on new research and personal documents, accessible text tells the fascinating story of Einstein's life, including his early years in Germany, his achievements that led to the Nobel Prize, and his role in the development of the atomic bomb. Plentiful photographs, explanatory diagrams, and illuminating sidebars add to the reader's experience, helping to reveal the person and the genius behind the name."
            },
            new Book
            {
                BookTitle = "Quantum Physics for Beginners: From Wave Theory to Quantum Computing. Understanding How Everything Works by a Simplified Explanation of Quantum Physics and Mechanics Principles",
                Price = 15.76,
                Stock = 100,
                CreatedBook = new DateTime(2021, 3, 14),
                ImageURL = "https://res.cloudinary.com/dhik9tniv/image/upload/v1740717324/uploaded_images/hociletnci1zhoj97dlh.jpg",
                Description = "\"Master the basics of quantum physics without feeling overwhelmed by complex math!\r\nWith over 35,000 copies sold, Quantum Physics for Beginners is the perfect starting point for anyone curious about this fascinating topic. It breaks down complex ideas into simple, clear explanations and reveals the secrets behind many of today’s most exciting technological innovations.\r\nLet the author guide you through two centuries of discoveries, exploring the principles that shape the universe in a way that’s both accessible and captivating.\""
            },
            new Book
            {
                BookTitle = "Greatest Scientific Minds: Charles Darwin, Albert Einstein, Isaac Newton",
                Price = 22.48,
                Stock = 100,
                CreatedBook = new DateTime(2024, 6, 23),
                ImageURL = "https://res.cloudinary.com/dhik9tniv/image/upload/v1740717325/uploaded_images/z89myfpwfw5vb1avxnun.jpg",
                Description = "\"Enrich the scientific wisdom in you with this skillfully crafted boxed set of three seminal works of the greatest scientists. This set brings together Charles Darwin's Origin of Species, which introduces the revolutionary concept of natural selection, Albert Einstein’ s Relativity: The General and the Special Theory, which delves into the realm of theoretical physics, and Newton’ s The Principia, which laid the groundwork for classical mechanics. This meticulous collection gives its readers an insightful glimpse into the groundbreaking discoveries of these legends of the scientific world and the impact they have on our present lives.\r\nDelve into the intellectual prowess of three iconic scientists!\r\nA beautiful collection of three books showcasing groundbreaking scientific discoveries. Ideal books to dive deep into the intricate concepts in the world of science. Develops a sense of wonder and appreciation for the brilliant contributions made by these legends in the field of science. A perfect boxed set for gifting and keepsakes. An ideal value addition to any library.\""
            },


            // Thể loại Mystery, Thriller & Suspense
            new Book
            {
                BookTitle = "The Da Vinci Code",
                Price = 18.0,
                Stock = 100,
                CreatedBook = new DateTime(2016, 9, 13),
                ImageURL = "https://res.cloudinary.com/dz2s1q9um/image/upload/v1740754934/uploaded_images/ewvof21fatzcdpw6ribe.jpg",
                Description = "\"Dan Brown’s mega-bestseller is now available for a new generation of readers. This young adult adaptation is the perfect way to get ready for Origin, the latest novel featuring the character Robert Langdon. It will remind fans everywhere why the New York Times calls The Da Vinci Code “blockbuster perfection.” \r\n\r\nIncludes over twenty color photos showing important locations, landmarks, and artwork, taking readers from Paris to London and beyond! \r\n\r\nThe greatest conspiracy of the past two thousand years is about to unravel. \r\n\r\nRobert Langdon, professor of religious symbology at Harvard, is in Paris to give a lecture. At the reception that follows, he is scheduled to meet with a revered curator from the world-famous Louvre museum. But the curator never shows up, and later that night Langdon is awakened by authorities and told that the curator has been found dead. He is then taken to the Louvre—the scene of the crime—where he finds out that baffling clues have been left behind. \r\n\r\nThus begins a race against time, as Robert Langdon becomes a suspect and, with the help of French cryptologist Sophie Neveu, must decipher a mystifying trail of clues that the two come to realize have been left specifically for them. If Robert and Sophie cannot solve the puzzle in time, an ancient truth could be lost forever—and they themselves might end up as collateral damage. \"",
            },
            new Book
            {
                BookTitle = "The Sherlock Holmes Collection: Deluxe 6-Book Hardcover Boxed Settion (Arcturus Collector's Classics, 2)",
                Price = 59.99,
                Stock = 100,
                CreatedBook = new DateTime(2017, 9, 1),
                ImageURL = "https://res.cloudinary.com/dz2s1q9um/image/upload/v1740754939/uploaded_images/ny7fhahpafu77phvee0y.jpg",
                Description = "\"Step into the cobbled streets of Victorian London with this deluxe slipcased collection of Sherlock Holmes stories, presented in six handsome clothbound volumes. \r\nArthur Conan Doyle's famous detective Sherlock Holmes has captivated readers for over a century. This 6-volume boxset brings together his most famous cases, featuring cipher messages, stolen treasure and a mysterious beast roaming the moors... \r\nInspired by Victorian-era bookbinding, each volume is clothbound in beautiful autumnal shades and gold-foil embossing. The slipcase has luxurious cloth accents on the top and bottom and makes a wonderful display piece in any home library. \r\nStories include: \r\n• A Study in Scarlet \r\n• The Sign of Four \r\n• The Valley of Fear \r\n• The Hound of the Baskervilles \r\n• The Adventures of Sherlock Holmes \r\n• The Memoirs of Sherlock Holmes \r\n• The Returns of Sherlock Holmes \r\n• His Last Bow \r\nABOUT THE SERIES: The Arcturus Collector's Classics series are high-quality, clothbound box-sets of classic works of literature. With elegant embossed cover-designs and colored endpapers, these editions make wonderful gifts or collectibles to treasure forever.\"",
            },
            new Book
            {
                BookTitle = "The Silence of the Lambs",
                Price = 19.0,
                Stock = 100,
                CreatedBook = new DateTime(2018, 1, 1),
                ImageURL = "https://res.cloudinary.com/dz2s1q9um/image/upload/v1740754941/uploaded_images/xjuetgxdawz4mejmfnmh.png",
                Description = "\"A serial murderer known only by a grotesquely apt nickname—Buffalo Bill—is stalking women. He has a purpose, but no one can fathom it, for the bodies are discovered in different states. Clarice Starling, a young trainee at the FBI Academy, is surprised to be summoned by Jack Crawford, chief of the Bureau's Behavioral Science section. Her assignment: to interview Dr. Hannibal Lecter—Hannibal the Cannibal—who is kept under close watch in the Baltimore State Hospital for the Criminally Insane. \r\nDr. Lecter is a former psychiatrist with a grisly history, unusual tastes, and an intense curiosity about the darker corners of the mind. His intimate understanding of the killer and of Clarice herself form the core of Thomas Harris' The Silence of the Lambs—and ingenious, masterfully written book and an unforgettable classic of suspense fiction.\"",
            },
            new Book
            {
                BookTitle = "Death on the Nile",
                Price = 13.99,
                Stock = 100,
                CreatedBook = new DateTime(2011, 2, 1),
                ImageURL = "https://res.cloudinary.com/dz2s1q9um/image/upload/v1740754928/uploaded_images/oowvx2hstbbt1xmoapn7.jpg",
                Description = "\"“A top-notch literary brainteaser.” –New York Times \r\nSoon to be a major motion picture sequel to Murder on the Orient Express with a screenplay by Michael Green, directed by and starring Kenneth Branagh alongside Gal Gadot—coming Feb 11, 2022! \r\nBeloved detective Hercule Poirot embarks on a journey to Egypt in one of Agatha Christie’s most famous mysteries. \r\nThe tranquility of a luxury cruise along the Nile was shattered by the discovery that Linnet Ridgeway had been shot through the head. She was young, stylish, and beautiful. A girl who had everything . . . until she lost her life. \r\nHercule Poirot recalled an earlier outburst by a fellow passenger: “I’d like to put my dear little pistol against her head and just press the trigger.” Yet under the searing heat of the Egyptian sun, nothing is ever quite what it seems. \r\nA sweeping mystery of love, jealousy, and betrayal, Death on the Nile is one of Christie’s most legendary and timeless works.\"",
            },
            new Book
            {
                BookTitle = "The Devotion of Suspect X: A Detective Galileo Novel",
                Price = 19,
                Stock = 100,
                CreatedBook = new DateTime(2012, 2, 28),
                ImageURL = "https://res.cloudinary.com/dz2s1q9um/image/upload/v1740754935/uploaded_images/wb1ahexm5l5gxonpbby3.jpg",
                Description = "Yasuko Hanaoka is a divorced, single mother who thought she had finally escaped her abusive ex-husband Togashi. When he shows up one day to extort money from her, threatening both her and her teenaged daughter Misato, the situation quickly escalates into violence and Togashi ends up dead on her apartment floor. Overhearing the commotion, Yasuko's next door neighbor, middle-aged high school mathematics teacher Ishigami, offers his help, disposing not only of the body but plotting the cover-up step-by-step. When the body turns up and is identified, Detective Kusanagi draws the case and Yasuko comes under suspicion. Kusanagi is unable to find any obvious holes in Yasuko's manufactured alibi and yet is still sure that there's something wrong. Kusanagi brings in Dr. Manabu Yukawa, a physicist and college friend who frequently consults with the police. Yukawa, known to the police by the nickname Professor Galileo, went to college with Ishigami. After meeting up with him again, Yukawa is convinced that Ishigami had something to do with the murder. What ensues is a high level battle of wits, as Ishigami tries to protect Yasuko by outmaneuvering and outthinking Yukawa, who faces his most clever and determined opponent yet.",
            },
            new Book
            {
                BookTitle = "Inferno",
                Price = 13.99,
                Stock = 100,
                CreatedBook = new DateTime(2014, 5, 14),
                ImageURL = "https://res.cloudinary.com/dz2s1q9um/image/upload/v1740754930/uploaded_images/kxxqne466o0doxnzinao.jpg",
                Description = "\"This enhanced eBook of the #1 worldwide bestseller includes exclusive behind-the-scenes video of Dan Brown's Inferno research trips throughout Italy, and a fascinating twenty-five minute video of his book launch presentation in New York City. \r\nWith the publication of his groundbreaking novels The Da Vinci Code, The Lost Symbol, and Angels & Demons, Dan Brown has become an international bestselling sensation, seamlessly fusing codes, symbols, art, and history into riveting thrillers that have captivated hundreds of millions of readers around the world. Now, with this stunning special illustrated edition of his record-setting Inferno, brought to life by more than 200 breathtaking color images, Dan Brown takes readers deep into the heart of Italy . . . guiding them through a landscape that inspired one of history’s most ominous literary classics.\r\n “THE DARKEST PLACES IN HELL ARE RESERVED FOR THOSE WHO MAINTAIN THEIR NEUTRALITY IN TIMES OF MORAL CRISIS.”\"",
            },
            new Book
            {
                BookTitle = "Confessions",
                Price = 14.39,
                Stock = 1000,
                CreatedBook = new DateTime(2014, 8, 19),
                ImageURL = "https://res.cloudinary.com/dz2s1q9um/image/upload/v1740754926/uploaded_images/lcxmn7zrq7d2vjo15ypb.jpg",
                Description = "\"In this international bestselling thriller, a former teacher delivers her final lesson to her students—including the two children that murdered her daughter. \r\nAfter calling off her engagement in the wake of a tragic revelation, Yuko Moriguchi had nothing to live for except her only child, four-year-old child, Manami. Now, following an accident on the grounds of the middle school where she teaches, Yuko has given up and tendered her resignation. \r\nBut first she has one last lecture to deliver. She tells a story that upends everything her students ever thought they knew about two of their peers, and sets in motion a diabolical plot for revenge. \r\nNarrated in alternating voices, with twists you'll never see coming, Confessions probes the limits of punishment, despair, and tragic love, culminating in a harrowing confrontation between teacher and student that will place the occupants of an entire school in danger. You'll never look at a classroom the same way again.\"",
            },
            new Book
            {
                BookTitle = "The Iliad & the Odyssey (Deluxe Hardbound Edition)",
                Price = 22.49,
                Stock = 1000,
                CreatedBook = new DateTime(2018, 8, 1),
                ImageURL = "https://res.cloudinary.com/dz2s1q9um/image/upload/v1740754938/uploaded_images/tgfnpq2fedu1yjsvizqv.jpg",
                Description = "\"Embark on a literary odyssey through ancient Greece with Homer's timeless epics, The Iliad and The Odyssey. In The Iliad, witness the ravages of the Trojan War as gods and mortals clash in a tale of honor, heroism, and the consequences of unchecked pride. Then, journey alongside Odysseus in The Odyssey as he battles mythical creatures, evades vengeful gods, ad strives to return home, navigating treacherous seas and testing the limits of human resilience. These masterpieces of ancient literature capture the essence of the human experience, exploring themes of love, loss, destiny, and the indomitable spirit of adventure. \r\nEpic tales of ancient heroes and legendary battles. \r\nUnforgettable characters shaped by gods and destiny. \r\nTimeless exploration of human emotions and triumphs. \r\nGripping narratives of honor, courage, and sacrifice. \r\nImmersive journey through ancient Greece's mythical landscapes. \"",
            },
            new Book
            {
                BookTitle = "Origin",
                Price = 22.27,
                Stock = 100,
                CreatedBook = new DateTime(1999, 1, 1),
                ImageURL = "https://res.cloudinary.com/dz2s1q9um/image/upload/v1740754931/uploaded_images/mq6a4xrtxvm3a7dlubup.jpg",
                Description = "\"Robert Langdon, Harvard professor of symbology and religious iconology, arrives at the ultramodern Guggenheim Museum Bilbao to attend a major announcement--the unveiling of a discovery that \"\"will change the face of science forever.\"\" The evening's host is Edmond Kirsch, a forty-year-old billionaire and futurist whose dazzling high-tech inventions and audacious predictions have made him a renowned global figure. Kirsch, who was one of Langdon's first students at Harvard two decades earlier, is about to reveal an astonishing breakthrough . . . one that will answer two of the fundamental questions of human existence.\r\nAs the event begins, Langdon and several hundred guests find themselves captivated by an utterly original presentation, which Langdon realizes will be far more controversial than he ever imagined. But the meticulously orchestrated evening suddenly erupts into chaos, and Kirsch's precious discovery teeters on the brink of being lost forever. Reeling and facing an imminent threat, Langdon is forced into a desperate bid to escape Bilbao. With him is Ambra Vidal, the elegant museum director who worked with Kirsch to stage the provocative event. Together they flee to Barcelona on a perilous quest to locate a cryptic password that will unlock Kirsch's secret.\r\n\r\nNavigating the dark corridors of hidden history and extreme religion, Langdon and Vidal must evade a tormented enemy whose all-knowing power seems to emanate from Spain's Royal Palace itself . . . and who will stop at nothing to silence Edmond Kirsch. On a trail marked by modern art and enigmatic symbols, Langdon and Vidal uncover clues that ultimately bring them face-to-face with Kirsch's shocking discovery . . . and the breathtaking truth that has long eluded us. Origin is stunningly inventive--Dan Brown's most brilliant and entertaining novel to date\"",
            },
            new Book
            {
                BookTitle = "The Frozen River",
                Price = 0,
                Stock = 100,
                CreatedBook = new DateTime(2014, 11, 5),
                ImageURL = "https://res.cloudinary.com/dz2s1q9um/image/upload/v1740754937/uploaded_images/lehxsme4ktbn3bh83v4j.jpg",
                Description = "\"Maine, 1789: When the Kennebec River freezes, entombing a man in the ice, Martha Ballard is summoned to examine the body and determine cause of death. As a midwife and healer, she is privy to much of what goes on behind closed doors in Hallowell. Her diary is a record of every birth and death, crime and debacle that unfolds in the close-knit community. Months earlier, Martha documented the details of an alleged rape committed by two of the town’s most respected gentlemen—one of whom has now been found dead in the ice. But when a local physician undermines her conclusion, declaring the death to be an accident, Martha is forced to investigate the shocking murder on her own.\r\nOver the course of one winter, as the trial nears, and whispers and prejudices mount, Martha doggedly pursues the truth. Her diary soon lands at the center of the scandal, implicating those she loves, and compelling Martha to decide where her own loyalties lie.\r\n\r\nClever, layered, and subversive, Ariel Lawhon’s newest offering introduces an unsung heroine who refused to accept anything less than justice at a time when women were considered best seen and not heard. The Frozen River is a thrilling, tense, and tender story about a remarkable woman who left an unparalleled legacy yet remains nearly forgotten to this day.\"",
            },

            // Thể loại Business & Money
            new Book
            {
                BookTitle = "The Psychology of Money: Timeless lessons on wealth, greed, and happiness ",
                Price = 19.99,
                Stock = 100,
                CreatedBook = new DateTime(2020, 9, 8),
                ImageURL = "https://res.cloudinary.com/dz2s1q9um/image/upload/v1740757180/uploaded_images_2/nu4whujnslxgwgkl19vd.jpg",
                Description = "\"**OVER 7 MILLION COPIES SOLD AROUND THE WORLD… The Psychology of Money is the original bestselling classic from the author of the new book, Same as Ever.**\r\nDoing well with money isn’t necessarily about what you know. It’s about how you behave. And behavior is hard to teach, even to really smart people.\r\n\r\nMoney―investing, personal finance, and business decisions―is typically taught as a math-based field, where data and formulas tell us exactly what to do. But in the real world people don’t make financial decisions on a spreadsheet. They make them at the dinner table, or in a meeting room, where personal history, your own unique view of the world, ego, pride, marketing, and odd incentives are scrambled together.\r\n\r\nIn The Psychology of Money, award-winning author Morgan Housel shares 19 short stories exploring the different ways people think about money and teaches you how to make better sense of one of life’s most important topics.\"",
            },
            new Book
            {
                BookTitle = "The Richest Man in Babylon",
                Price = 6.24,
                Stock = 100,
                CreatedBook = new DateTime(2023, 4, 1),
                ImageURL = "https://res.cloudinary.com/dz2s1q9um/image/upload/v1740757181/uploaded_images_2/ibs0dep2az6zzikskuyk.jpg",
                Description = "\"Money is plentiful for those who understand the simple laws which govern its acquisition.\r\n\r\nIn the early 1920s, George S. Clason wrote a collection of parables set in ancient Babylon that provided guidance on one’s financial well-being. These parables were distributed as pamphlets to U.S. banking and insurance customers and were so well-received by the public that in 1926, the parables were collected into one volume under the title of his most famous story, The Richest Man in Babylon. Largely seen as a classic in personal financial advice, The Richest Man in Babylon has provided millions with guidance and inspiration for financial wellness.\"",
            },
            new Book
            {
                BookTitle = "Think and Grow Rich",
                Price = 11.00,
                Stock = 100,
                CreatedBook = new DateTime(2007, 12, 15),
                ImageURL = "https://res.cloudinary.com/dz2s1q9um/image/upload/v1740756891/uploaded_images_2/vxup0j9nchzhj5xgdjyg.jpg",
                Description = "\"The bestselling success book of all time—now revised and updated for the 21st century.\r\n\r\nThink and Grow Rich has been called the “Granddaddy of All Motivational Literature.” It was the first book to boldly ask, “What makes a winner?” The man who asked and listened for the answer, Napoleon Hill, is now counted in the top ranks of the world's winners himself. The most famous of all teachers of success spent “a fortune and the better part of a lifetime of effort” to produce the “Law of Success” philosophy that forms the basis of his books and that is so powerfully summarized in this one.\r\n\r\nIn the original Think and Grow Rich, published in 1937, Hill draws on stories of Andrew Carnegie, Thomas Edison, Henry Ford, and other millionaires of his generation to illustrate his principles. In the updated version, Arthur R. Pell, Ph.D., a nationally known author, lecturer, and consultant in human resources management and an expert in applying Hill's thought, deftly interweaves anecdotes of how contemporary millionaires and billionaires, such as Bill Gates, Mary Kay Ash, Dave Thomas, and Sir John Templeton, achieved their wealth. Outmoded or arcane terminology and examples are faithfully refreshed to preclude any stumbling blocks to a new generation of readers.\"",
            },
            new Book
            {
                BookTitle = "Rich Dad Poor Dad",
                Price = 9.99,
                Stock = 100,
                CreatedBook = new DateTime(2022, 4, 5),
                ImageURL = "https://res.cloudinary.com/dz2s1q9um/image/upload/v1740757179/uploaded_images_2/rr4z3x0klev8lww742jn.jpg",
                Description = "April of 2022 marks a 25-year milestone for the personal finance classic Rich Dad Poor Dad that still ranks as the #1 Personal Finance book of all time. And although 25 years have passed since Rich Dad Poor Dad was first published, readers will find that very little in the book itself has changed — and for good reason. While so much in our world is changing a high speed, the lessons about money and the principles of Rich Dad Poor Dad haven’t changed. Today, as money continues to play a key role in our daily lives, the messages in Robert Kiyosaki’s international bestseller are more timely and more important than ever.",
            },
            new Book
            {
                BookTitle = "The $100 Startup: Reinvent the Way You Make a Living, Do What You Love, and Create a New Future",
                Price = 15.19,
                Stock = 100,
                CreatedBook = new DateTime(2015, 1, 15),
                ImageURL = "https://res.cloudinary.com/dz2s1q9um/image/upload/v1740756883/uploaded_images_2/rch7brpwsh5hvw7v8idb.jpg",
                Description = "\"Change your job to change your life.\r\n\r\nAre you tired of the grind of the 9-to-5 job and dreaming of professional satisfaction on your own terms? You can quit the rat race and start up on your own – and you don't need an MBA or a huge investment to do it. The $100 Startup by Chris Guillebeau is your escape hatch.\r\n\r\nWith practical advice, this is your manual to a new way of living. Not around traditional employment, but around your dreams.\r\n\r\nLearn how to:\r\n\r\n- Earn a good living on your own terms, when and where you want\r\n\r\n- Achieve that perfect blend of passion and income to make work something you love\r\n\r\n- Take crucial insights from 50 ordinary people who started a business with $100 or less\r\n\r\n- Spend less time working and more time living your life\r\n\r\nStrike the perfect blend of passion and income and break free from the confines of a monotonous job.\r\n\r\n'The money you have is enough. Chris makes it crystal clear: there are no excuses left. Start now, not later. Hurry' – Seth Godin, author of The 1-Page Marketing Plan\"",
            },
            new Book
            {
                BookTitle = "Economic Facts and Fallacies",
                Price = 19.99,
                Stock = 100,
                CreatedBook = new DateTime(1999, 1, 1),
                ImageURL = "https://res.cloudinary.com/dz2s1q9um/image/upload/v1740756885/uploaded_images_2/ajv7vwx1d6c4b2c5rdn5.jpg",
                Description = "Economic Facts and Fallacies exposes some of the most popular fallacies about economic issues-and does so in a lively manner and without requiring any prior knowledge of economics by the reader. These include many beliefs widely disseminated in the media and by politicians, such as mistaken ideas about urban problems, income differences, male-female economic differences, as well as economics fallacies about academia, about race, and about Third World countries. One of the themes of Economic Facts and Fallacies is that fallacies are not simply crazy ideas but in fact have a certain plausibility that gives them their staying power-and makes careful examination of their flaws both necessary and important, as well as sometimes humorous. Written in the easy-to-follow style of the author's Basic Economics, this latest book is able to go into greater depth, with real world examples, on specific issues.",
            },
            new Book
            {
                BookTitle = "Basic Economics ",
                Price = 45.0,
                Stock = 100,
                CreatedBook = new DateTime(2014, 12, 2),
                ImageURL = "https://res.cloudinary.com/dz2s1q9um/image/upload/v1740756884/uploaded_images_2/ehdffzhpwyi2tsuyoxqe.jpg",
                Description = "\"The bestselling citizen's guide to economics\r\n\r\nBasic Economics is a citizen's guide to economics, written for those who want to understand how the economy works but have no interest in jargon or equations. Bestselling economist Thomas Sowell explains the general principles underlying different economic systems: capitalist, socialist, feudal, and so on. In readable language, he shows how to critique economic policies in terms of the incentives they create, rather than the goals they proclaim. With clear explanations of the entire field, from rent control and the rise and fall of businesses to the international balance of payments, this is the first book for anyone who wishes to understand how the economy functions.\r\n\r\nThis fifth edition includes a new chapter explaining the reasons for large differences of wealth and income between nations.\r\n\r\nDrawing on lively examples from around the world and from centuries of history, Sowell explains basic economic principles for the general public in plain English.\"",
            },
            new Book
            {
                BookTitle = "The Real Economy: History and Theory",
                Price = 39.95,
                Stock = 100,
                CreatedBook = new DateTime(2025, 2, 25),
                ImageURL = "https://res.cloudinary.com/dz2s1q9um/image/upload/v1740756890/uploaded_images_2/chtmafwjrwv5utaivag0.jpg",
                Description = "\"A provocative new theory of “the economy,” its history, and its politics that better unites history and economics\r\n\r\nWhat is the economy, really? Is it a “market sector,” a “general equilibrium,” or the “gross domestic product”? Economics today has become so preoccupied with methods that economists risk losing sight of the economy itself. Meanwhile, other disciplines, although often intent on criticizing the methods of economics, have failed to articulate an alternative vision of the economy. Before the ascent of postwar neoclassical economics, fierce debates raged, as many different visions of the economy circulated and competed with one another. In The Real Economy, Jonathan Levy returns to the spirit of this earlier era, which, in all its contentiousness, gave birth to the discipline of economics.\r\n\r\nDrawing inspiration particularly from Thorstein Veblen and John Maynard Keynes, Levy proposes a theory of the economy that is open to rich empirical and historical scrutiny, covering topics that include the emergence of capitalism, the notion of radical uncertainty, the meaning of demand, the primal desire for money, the history of corporations, and contemporary globalization. Writing for anyone interested in the study of the economy, Levy provides an invaluable provocation for a broader debate in the social sciences and humanities concerning what “the economy” is.\"",
            },
            new Book
            {
                BookTitle = "The Economic Government of the World: 1933-2023",
                Price = 45.0,
                Stock = 100,
                CreatedBook = new DateTime(2023, 11, 14),
                ImageURL = "https://res.cloudinary.com/dz2s1q9um/image/upload/v1740756887/uploaded_images_2/jivbt02llyxtwvat2c3m.jpg",
                Description = "\"Foreign Affairs Best Books of the Year (2023)\r\n\r\nAn epic history of the people and institutions that have built the global economy since the Great Depression.\r\n\r\nIn this vivid landmark history, the distinguished economic historian Martin Daunton pulls back the curtain on the institutions and individuals who have created and managed the global economy over the last ninety years, revealing how and why one economic order breaks down and another is built. During the Great Depression, trade and currency warfare led to the rise of economic nationalism―a retreat from globalization that culminated in war. From World War II came a new, liberal economic order. Squarely reflecting the interests of the West in the Cold War, liberalism faced collapse in the 1970s and was succeeded by neoliberalism, financialization, and hyper-globalization.\r\n\r\nNow, as leading nations are tackling the fallout from Covid-19 and threats of inflation, food insecurity, and climate change, Daunton calls for a return to a more just and equitable form of globalization. Western imperial powers have overwhelmingly determined the structures of world economic government, often advancing their own self-interests and leading to ruinous resource extraction, debt, poverty, and political and social instability in the Global South. He argues that while our current economic system is built upon the politics of and between the world’s biggest economies, a future of global recovery―and the reduction of economic inequality―requires the development of multilateral institutions.\r\n\r\nDramatic and revelatory, The Economic Government of the World offers a powerful analysis of the origins of our current global crises and a path toward a fairer international order.\"",
            },
            new Book
            {
                BookTitle = "The Crisis of Democratic Capitalism ",
                Price = 30,
                Stock = 100,
                CreatedBook = new DateTime(2023, 2, 7),
                ImageURL = "https://res.cloudinary.com/dz2s1q9um/image/upload/v1740756886/uploaded_images_2/jk36zn5vymmdqnsxppx2.jpg",
                Description = "From the chief economics commentator of the Financial Times, a magnificent reckoning with how and why the marriage between democracy and capitalism is coming undone, and what can be done to reverse this terrifying dynamic\r\n\r\nMartin Wolf has long been one of the wisest voices on global economic issues. He has rarely been called an optimist, yet he has never been as worried as he is today. Liberal democracy is in recession, and authoritarianism is on the rise. The ties that ought to bind open markets to free and fair elections are threatened, even in democracy’s heartlands, the United States and England.\r\n    Around the world, powerful voices argue that capitalism is better without democracy; others argue that democracy is better without capitalism. This book is a forceful rejoinder to both views. Even as it offers a deep, lucid assessment of why this marriage has grown so strained, it makes clear why a divorce of capitalism from democracy would be a calamity for the world. They need each other even if they find it hard to life together.",
            },

            // Thể loại Computers & Technology
            new Book
            {
                BookTitle = "Computers Made Easy: From Dummy To Geek",
                Price = 18.99,
                Stock = 100,
                CreatedBook = new DateTime(2018, 6, 12),
                ImageURL = "https://res.cloudinary.com/drqi2jgr5/image/upload/v1740584594/61kKAm8B0-L._SY342__o3qq8q.jpg",
                Description = "\"UPDATED FOR 2025 WITH NEW CONTENT AND WINDOWS 11 24H2 FEATURES\r\n\r\nA Foundation in Computers & Software That's Easy to Understand.\r\nComputers Made Easy is designed to take your overall computer skills from a beginner to the next level, and beyond. This book will give you a top level understanding of how to use your PC without a needing a background in computers. This easy to use, step by step guide will help you navigate your way to becoming proficient with computers, operating systems (Windows 10 & 11), hardware and software.\r\n\r\nIntroduction\r\nChapter 1 – What is a Computer?\r\nChapter 2 – Computer Peripherals\r\nChapter 3 – Microsoft Windows\r\nChapter 4 – Software\r\nChapter 5 – Printers\r\nChapter 6 – The Internet\r\nChapter 7 – Email\r\nChapter 8 – Office Productivity Software\r\nChapter 9 – Antivirus and Antispyware Software\r\nChapter 10 – Avoiding Scams\r\nChapter 11 – Error Messages, Crashes, & Troubleshooting\r\nChapter 12 – Wi-Fi and Internet Troubleshooting\r\nChapter 13 – Backup and Protection\r\nChapter 14 - Security\r\nChapter 15 – Cloud Storage\r\nChapter 16 – Basic Networking\r\nWhat’s Next?\r\n\r\nAbout the Author\r\n\r\nJames Bernstein has been working with various companies in the IT field for over 20 years, managing technologies such as SAN and NAS storage, VMware, backups, Windows Servers, Active Directory, DNS, DHCP, Networking, Microsoft Office, Exchange, and more.\r\n\r\nHe has obtained certifications from Microsoft, VMware, CompTIA, ShoreTel, and SNIA, and continues to strive to learn new technologies to further his knowledge on a variety of subjects.\r\n\r\nHe is also the founder of the website OnlineComputerTips.com, which offers its readers valuable information on topics such as Windows, networking, hardware, software, and troubleshooting. James writes much of the content himself and adds new content on a regular basis. The site was started in 2005 and is still going strong today.\r\n\"",
            },
            new Book
            {
                BookTitle = "Discovering Computers: Digital Technology, Data, and Devices (MindTap Course List)",
                Price = 225.95,
                Stock = 100,
                CreatedBook = new DateTime(2022, 10, 5),
                ImageURL = "https://res.cloudinary.com/drqi2jgr5/image/upload/v1740671440/Discovering_Computers_lhaq4p.jpg",
                Description = "DISCOVERING COMPUTERS: DIGITAL TECHNOLOGY, DATA, AND DEVICES, 17th edition, teaches you not only the basics of technology, but also how you will use it -- and the responsibilities that go along with being a digital citizen. Focusing on current technology, the content addresses convergence of devices and platforms. Each module integrates practical how-to tips, ethics issues and security topics, while Consider This boxes woven throughout help you sharpen your critical-thinking skills. A variety of end-of-module activities -- checkpoint questions, small group activities and problem-solving exercises -- enable you to put what you learn into practice. MindTap digital learning solution is also available. Using an inviting approach that ensures understanding, DISCOVERING COMPUTERS equips you with the information you need for success at home, school and work.",
            },
            new Book
            {
                BookTitle = "The Microsoft Office 365 Bible: The Most Updated and Complete Guide to Excel, Word, PowerPoint, Outlook, OneNote, OneDrive, Teams, Access, and Publisher from Beginners to Advanced",
                Price = 37.97,
                Stock = 100,
                CreatedBook = new DateTime(2023, 8, 28),
                ImageURL = "https://res.cloudinary.com/drqi2jgr5/image/upload/v1740834036/Microsoft_bo64ap.jpg",
                Description = "DISCOVERING COMPUTERS: DIGITAL TECHNOLOGY, DATA, AND DEVICES, 17th edition, teaches you not only the basics of technology, but also how you will use it -- and the responsibilities that go along with being a digital citizen. Focusing on current technology, the content addresses convergence of devices and platforms. Each module integrates practical how-to tips, ethics issues and security topics, while Consider This boxes woven throughout help you sharpen your critical-thinking skills. A variety of end-of-module activities -- checkpoint questions, small group activities and problem-solving exercises -- enable you to put what you learn into practice. MindTap digital learning solution is also available. Using an inviting approach that ensures understanding, DISCOVERING COMPUTERS equips you with the information you need for success at home, school and work.",
            },
            new Book
            {
                BookTitle = "The History of the Computer: People, Inventions, and Technology that Changed Our World",
                Price = 19.99,
                Stock = 100,
                CreatedBook = new DateTime(2022, 5, 17),
                ImageURL = "https://res.cloudinary.com/drqi2jgr5/image/upload/v1740834108/The_History_of_the_Computer_dsdxgs.jpg",
                Description = "\"⭐ This bundle includes 4 COMPLETELY FREE EBOOKS! Find out how to download them inside the book ⭐\r\n\r\nNOW! Stop wasting time and money trying to figure out everything yourself and master all the functions of the Office Suite!\r\n\r\nIf you are a fan of PC and use it for work, entertainment, or anything else, mastering main Microsoft Programs is a MUST.\r\n\r\nI can’t tell you enough how many people I see not just struggling to use a program like EXCEL, WORD, POWERPOINT, ONE NOTE, ONE DRIVE, OUTLOOK, TEAMS, ACCESS, PUBLISHER, and others, but also… wasting so much time doing things that should take minutes and even seconds instead of hours and days.\r\n\r\nOn top of that, on average most people use less than 5% of programs’ full potential at any given time.\r\n\r\nFor this exact reason, I created this amazing, in-depth book BUNDLE – to help you master these programs in no time, even if you don’t have any experience.\r\n\r\nHere is what’s inside:\r\n\r\n➡️ BOOK #1 - EXCEL\r\n\r\n➡️ BOOK #2 - WORD\r\n\r\n➡️ BOOK #3 - POWERPOINT\r\n\r\n➡️ BOOK #4 - ONE NOTE\r\n\r\n➡️ BOOK #5 - ONE DRIVE\r\n\r\n➡️ BOOK #6 - OUTLOOK\r\n\r\n➡️ BOOK #7 - TEAMS\r\n\r\n➡️ BOOK #8 - ACCESS\r\n\r\n➡️ BOOK #9 - PUBLISHER\r\n\r\nAs you can see, this book covers the majority of the most important Microsoft Office programs and helps you understand:\r\n\r\n✅ Ins and Outs of each program\r\n\r\n✅ How to learn the fundamentals fast and use main functions effectively\r\n\r\n✅ Different kinds of shortcuts and tips for smart use and speed\r\n\r\n✅ Differences between them and when you should choose which program for a specific task\r\n\r\n✅ Unique elements and functions you have to be aware of but probably have never used\r\n\r\n✅ So much more!\r\n\r\nFOR A LIMITED TIME! BUY THIS “9 in 1” BUNDLE AND GET THE 4 MORE BOOKS FOR FREE!\r\n\r\n🆓 BOOK BONUS 1 - HOW TO BUILD THE PERFECT OFFICE AT HOME\r\n\r\n🆓 BOOK BONUS 2 - HOW TO MANAGE TIME AS A PROFESSIONAL\r\n\r\n🆓 BOOK BONUS 3 - 7 WAYS TO STOP WASTING TIME AND GET STUFF DONE\r\n\r\n🆓 BOOK BONUS 4 - IMPROVE YOUR COMMUNICATION SKILLS AT WORK WITH THIS METHOD\r\n\r\nAnd even if you don’t have any experience whatsoever, or just bought your PC a few days ago and now you are wondering what the best and simplest way to master Microsoft Office is, this book is for you!\r\n\r\nScroll Back Up and Get Your Copy to Learn Everything you NEED to Master Microsoft Office 365!\"",
            },
            new Book
            {
                BookTitle = "Computer & Technology Basics: What you need to know about Hardware, Software, Internet, Cloud Computing, Networks, Computer Security, Databases, ... Intelligence, File Management and Programming",
                Price = 11.99,
                Stock = 100,
                CreatedBook = new DateTime(2020, 1, 21),
                ImageURL = "https://res.cloudinary.com/drqi2jgr5/image/upload/v1740834160/Computer_Technology_Basics_mdlatc.jpg",
                Description = "\"A MATHICAL HONOR BOOK • A strikingly illustrated overview of the computing machines that have changed our world—from the abacus to the smartphone—and the people who made them, by the New York Times bestselling author and illustrator of Women in Science.\r\n\r\n“A beautifully illustrated journey through the history of computing, from the Antikythera mechanism to the iPhone and beyond—I loved it.”—Eben Upton, Founder and CEO of Raspberry Pi\r\n\r\nONE OF THE BEST BOOKS OF THE YEAR: The New York Public Library\r\n\r\nComputers are everywhere and have impacted our lives in so many ways. But who created them, and why? How have they transformed the way that we interact with our surroundings and each other?\r\n\r\nPacked with accessible information, fun facts, and discussion starters, this charming and art-filled book takes you from the ancient world to the modern day, focusing on important inventions, from the earliest known counting systems to the sophisticated algorithms behind AI. The History of the Computer also profiles a diverse range of key players and creators—from An Wang and Margaret Hamilton to Steve Jobs and Sir Tim Berners-Lee—and illuminates their goals, their intentions, and the impact of their inventions on our everyday lives.\r\n\r\nThis entertaining and educational journey will help you understand our most important machines and how we can use them to enhance the way we live. You’ll never look at your phone the same way again!\"",
            },
            new Book
            {
                BookTitle = "Too Much Fun: The Five Lives of the Commodore 64 Computer (Platform Studies) ",
                Price = 38.98,
                Stock = 100,
                CreatedBook = new DateTime(2024, 12, 10),
                ImageURL = "https://res.cloudinary.com/drqi2jgr5/image/upload/v1740834205/Too_Much_Fun_qyqim5.jpg",
                Description = "\"The surprising history of the Commodore 64, the best-selling home computer of the 1980s—the machine that taught the world that computing should be fun.\r\n\r\nThe Commodore 64 (C64) is officially the best-selling desktop computer model of all time, according to The Guinness Book of World Records. It was also, from 1985 to 1993, the platform for which most video games were made. But while it sold at least twice as many units as other home computers of its time, like the Apple II, ZX Spectrum, or Commodore Amiga, it is strangely forgotten in many computer histories. In Too Much Fun, Jesper Juul argues that the C64 was so popular because it was so versatile, a machine developers and users would reinvent again and again over the course of 40 years.\r\n\r\nFirst it was a serious computer, next a game computer, then a computer for technical brilliance (graphical demos using the machine in seemingly impossible ways), then a struggling competitor, and finally a retro device whose limitations are now charming. The C64, Juul shows, has been ignored by history because it was too much fun. Richly illustrated in full color, this book is the first in-depth examination of the C64’s design and history, and the first to integrate US and European histories. With interviews of Commodore engineers and with its insightful look at C64 games, music, and software, from Summer Games to International Karate to Simons’ BASIC, Too Much Fun will appeal to those who used a Commodore 64, those interested in the history of computing and video games and computational literacy, or just those who wish their technological devices would last longer.\"",
            },
            new Book
            {
                BookTitle = "Home Computers: 100 Icons that Defined a Digital Generation (Mit Press)",
                Price = 29.95,
                Stock = 100,
                CreatedBook = new DateTime(2020, 5, 19),
                ImageURL = "https://res.cloudinary.com/drqi2jgr5/image/upload/v1740834243/Home_Computers_wsafoz.jpg",
                Description = "\"A celebration of the early years of the digital revolution, when computing power was deployed in a beige box on your desk.\r\nToday, people carry powerful computers in our pockets and call them “phones.” A generation ago, people were amazed that the processing power of a mainframe computer could be contained in a beige box on a desk. This book is a celebration of those early home computers, with specially commissioned new photographs of 100 vintage computers and a generous selection of print advertising, product packaging, and instruction manuals. Readers can recapture the glory days of fondly remembered (or happily forgotten) machines including the Commodore 64, TRS-80, Apple Lisa, and Mattel Aquarius—traces of the techno-utopianism of the not-so-distant past.\r\n\r\nHome Computers showcases mass-market success stories, rarities, prototypes, one-offs, and never-before-seen specimens. The heart of the book is a series of artful photographs that capture idiosyncratic details of switches and plugs, early user-interface designs, logos, and labels. After a general scene-setting retrospective, the book proceeds computer by computer, with images of each device accompanied by a short history of the machine, its inventors, its innovations, and its influence. Readers who inhabit today's always-on, networked, inescapably connected world will be charmed by this visit to an era when the digital revolution could be powered down every evening.\"",
            },
            new Book
            {
                BookTitle = "Technology Basics Dictionary: Tech and Computers Simplified",
                Price = 27.99,
                Stock = 100,
                CreatedBook = new DateTime(2017, 6, 1),
                ImageURL = "https://res.cloudinary.com/drqi2jgr5/image/upload/v1740834272/Technology_Basics_Dictionaty_f5qrgc.jpg",
                Description = "Newly updated for 2023! The Technology Basics Dictionary: Tech and Computers Simplified is a dictionary for anyone. Whether you're completely inexperienced with technology or you're an experienced technology expert, this dictionary defines complex terms in an easy-to-understand fashion. It was created by Jack Stanley and Erik Gross, the Co-Founders of The Tech Academy. If you want to easily define words you hear everyday, this is the dictionary for you! Purchase your copy today! Learn more about The Tech Academy here: learncodinganywhere.com",
            },
            new Book
            {
                BookTitle = "The Phoenix Project: A Novel about IT, DevOps, and Helping Your Business Win 5th Anniversary Edition",
                Price = 30.95,
                Stock = 100,
                CreatedBook = new DateTime(2013, 1, 10),
                ImageURL = "https://res.cloudinary.com/drqi2jgr5/image/upload/v1740834301/The_Phoenix_Project_vrijgp.jpg",
                Description = "\"Five years after this sleeper hit took on the world of IT and flipped it on its head, the fifth anniversary edition of The Phoenix Project continues to guide IT in the DevOps revolution. In this newly updated and expanded edition of the best-selling The Phoenix Project, co-author Gene Kim includes a new afterword and a deeper delve into the Three Ways as described in The DevOps Handbook.\r\n\r\nBill, an IT manager at Parts Unlimited, has been tasked with taking on a project critical to the future of the business, code named Phoenix Project. But the project is massively over budget and behind schedule. The CEO demands Bill must fix the mess in 90 days, or else Bill’s entire department will be outsourced.\r\n\r\nWith the help of a prospective board member and his mysterious philosophy of the Three Ways, Bill starts to see that IT work has more in common with manufacturing plant work than he ever imagined. With the clock ticking, Bill must organize work flow, streamline interdepartmental communications, and effectively serve the other business functions at Parts Unlimited.\r\n\r\nIn a fast-paced and entertaining style, three luminaries of the DevOps movement deliver a story that anyone who works in IT will recognize. Listeners will not only learn how to improve their own IT organizations, they’ll never view IT the same.\r\n\r\nPLEASE NOTE: When you purchase this title, the accompanying PDF will be available in your Audible Library along with the audio.\"",
            },
            new Book
            {
                BookTitle = "Transformers for Natural Language Processing and Computer Vision: Explore Generative AI and Large Language Models with Hugging Face, ChatGPT, GPT-4V, and DALL-E 3",
                Price = 55.99,
                Stock = 100,
                CreatedBook = new DateTime(2024, 2, 29),
                ImageURL = "https://res.cloudinary.com/drqi2jgr5/image/upload/v1740834337/Transformers_for_Natural_Language_Processing_and_Computer_Vision_g7mmf8.jpg",
                Description = "\"The definitive guide to LLMs, from architectures, pretraining, and fine-tuning to Retrieval Augmented Generation (RAG), multimodal Generative AI, risks, and implementations with ChatGPT Plus with GPT-4, Hugging Face, and Vertex AI\r\n\r\nKey Features\r\nCompare and contrast 20+ models (including GPT-4, BERT, and Llama 2) and multiple platforms and libraries to find the right solution for your project\r\nApply RAG with LLMs using customized texts and embeddings\r\nMitigate LLM risks, such as hallucinations, using moderation models and knowledge bases\r\nPurchase of the print or Kindle book includes a free eBook in PDF format\r\nBook Description\r\nTransformers for Natural Language Processing and Computer Vision, Third Edition, explores Large Language Model (LLM) architectures, applications, and various platforms (Hugging Face, OpenAI, and Google Vertex AI) used for Natural Language Processing (NLP) and Computer Vision (CV).\r\n\r\nThe book guides you through different transformer architectures to the latest Foundation Models and Generative AI. You’ll pretrain and fine-tune LLMs and work through different use cases, from summarization to implementing question-answering systems with embedding-based search techniques. You will also learn the risks of LLMs, from hallucinations and memorization to privacy, and how to mitigate such risks using moderation models with rule and knowledge bases. You’ll implement Retrieval Augmented Generation (RAG) with LLMs to improve the accuracy of your models and gain greater control over LLM outputs.\r\n\r\nDive into generative vision transformers and multimodal model architectures and build applications, such as image and video-to-text classifiers. Go further by combining different models and platforms and learning about AI agent replication.\r\n\r\nThis book provides you with an understanding of transformer architectures, pretraining, fine-tuning, LLM use cases, and best practices.\r\n\r\nWhat you will learn\r\nBreakdown and understand the architectures of the Original Transformer, BERT, GPT models, T5, PaLM, ViT, CLIP, and DALL-E\r\nFine-tune BERT, GPT, and PaLM 2 models\r\nLearn about different tokenizers and the best practices for preprocessing language data\r\nPretrain a RoBERTa model from scratch\r\nImplement retrieval augmented generation and rules bases to mitigate hallucinations\r\nVisualize transformer model activity for deeper insights using BertViz, LIME, and SHAP\r\nGo in-depth into vision transformers with CLIP, DALL-E 2, DALL-E 3, and GPT-4V\r\nWho this book is for\r\nThis book is ideal for NLP and CV engineers, software developers, data scientists, machine learning engineers, and technical leaders looking to advance their LLMs and generative AI skills or explore the latest trends in the field.\r\n\r\nKnowledge of Python and machine learning concepts is required to fully understand the use cases and code examples. However, with examples using LLM user interfaces, prompt engineering, and no-code model building, this book is great for anyone curious about the AI revolution.\r\n\r\nTable of Contents\r\nWhat are Transformers?\r\nGetting Started with the Architecture of the Transformer Model\r\nEmergent vs Downstream Tasks: The Unseen Depths of Transformers\r\nAdvancements in Translations with Google Trax, Google Translate, and Gemini\r\nDiving into Fine-Tuning through BERT\r\nPretraining a Transformer from Scratch through RoBERTa\r\nThe Generative AI Revolution with ChatGPT\r\nFine-Tuning OpenAI GPT Models\r\nShattering the Black Box with Interpretable Tools\r\nInvestigating the Role of Tokenizers in Shaping Transformer Models\r\n(N.B. Please use the Read Sample option to see further chapters)\"",
            },

            // Thể loại Self-Help
            //new Book
            //{
            //    BookTitle = "Atomic Habits: An Easy & Proven Way to Build Good Habits & Break Bad Ones",
            //    Price = 13.99,
            //    Stock = 100,
            //    CreatedBook = new DateTime(2018, 10, 16),
            //    ImageURL = "https://res.cloudinary.com/drqi2jgr5/image/upload/v1740834441/Atomic_Habits_casgub.jpg",
            //    Description = "\"The number one New York Times best seller. Over one million copies sold!\r\n\r\nTiny Changes, Remarkable Results\r\n\r\nNo matter your goals, Atomic Habits offers a proven framework for improving - every day. James Clear, one of the world's leading experts on habit formation, reveals practical strategies that will teach you exactly how to form good habits, break bad ones, and master the tiny behaviors that lead to remarkable results.\r\n\r\nIf you're having trouble changing your habits, the problem isn't you. The problem is your system. Bad habits repeat themselves again and again not because you don't want to change, but because you have the wrong system for change. You do not rise to the level of your goals. You fall to the level of your systems. Here, you'll get a proven system that can take you to new heights.\r\n\r\nClear is known for his ability to distill complex topics into simple behaviors that can be easily applied to daily life and work. Here, he draws on the most proven ideas from biology, psychology, and neuroscience to create an easy-to-understand guide for making good habits inevitable and bad habits impossible. Along the way, listeners will be inspired and entertained with true stories from Olympic gold medalists, award-winning artists, business leaders, life-saving physicians, and star comedians who have used the science of small habits to master their craft and vault to the top of their field.\r\n\r\nLearn how to:\r\n\r\nMake time for new habits (even when life gets crazy)\r\nOvercome a lack of motivation and willpower\r\nDesign your environment to make success easier\r\nGet back on track when you fall off course\r\nAnd much more\r\nAtomic Habits will reshape the way you think about progress and success, and give you the tools and strategies you need to transform your habits - whether you are a team looking to win a championship, an organization hoping to redefine an industry, or simply an individual who wishes to quit smoking, lose weight, reduce stress, or achieve any other goal.\"",
            //},
            new Book
            {
                BookTitle = "The Let Them Theory: A Life-Changing Tool That Millions of People Can’t Stop Talking About",
                Price = 14.9,
                Stock = 100,
                CreatedBook = new DateTime(2024, 12, 24),
                ImageURL = "https://res.cloudinary.com/drqi2jgr5/image/upload/v1740834475/The_Let_Them_Theory_rybovn.jpg",
                Description = "\"\r\n#1 New York Times Bestseller\r\n\r\n#1 Sunday Times Bestseller\r\n\r\n#1 Amazon Bestseller\r\n\r\n#1 Audible Bestseller\r\n\r\nA Life-Changing Tool Millions of People Can’t Stop Talking About\r\n\r\nWhat if the key to happiness, success, and love was as simple as two words?\r\n\r\nIf you've ever felt stuck, overwhelmed, or frustrated with where you are, the problem isn't you. The problem is the power you give to other people. Two simple words—Let Them—will set you free. Free from the opinions, drama, and judgments of others. Free from the exhausting cycle of trying to manage everything and everyone around you. The Let Them Theory puts the power to create a life you love back in your hands—and this book will show you exactly how to do it.\r\n\r\nIn her latest groundbreaking book, The Let Them Theory, Mel Robbins—New York Times bestselling author and one of the world's most respected experts on motivation, confidence, and mindset—teaches you how to stop wasting energy on what you can't control and start focusing on what truly matters: YOU. Your happiness. Your goals. Your life.\r\n\r\nUsing the same no-nonsense, science-backed approach that's made The Mel Robbins Podcast a global sensation, Robbins explains why The Let Them Theory is already loved by millions and how you can apply it in eight key areas of your life to make the biggest impact. As you listen, you'll realize how much energy and time you've been wasting trying to control the wrong things—at work, in relationships, and in pursuing your goals—and how this is keeping you from the happiness and success you deserve.\r\n\r\nWritten as an easy-to-understand guide, Robbins shares relatable stories from her own life, highlights key takeaways, relevant research and introduces you to world-renowned experts in psychology, neuroscience, relationships, happiness, and ancient wisdom who champion The Let Them Theory every step of the way.\r\n\r\nLearn how to:\r\n\r\nStop wasting energy on things you can't control\r\nStop comparing yourself to other people\r\nBreak free from fear and self-doubt\r\nRelease the grip of people's expectations\r\nBuild the best friendships of your life\r\nCreate the love you deserve\r\nPursue what truly matters to you with confidence\r\nBuild resilience against everyday stressors and distractions\r\nDefine your own path to success, joy, and fulfillment\r\n...and so much more.\r\nThe Let Them Theory will forever change the way you think about relationships, control, and personal power. Whether you want to advance your career, motivate others to change, take creative risks, find deeper connections, build better habits, start a new chapter, or simply create more happiness in your life and relationships, this book gives you the mindset and tools to unlock your full potential.\r\n\r\nOrder your copy of The Let Them Theory now and discover how much power you truly have. It all begins with two simple words.\"",
            },
            new Book
            {
                BookTitle = "Self-Love Workbook for Women: Release Self-Doubt, Build Self-Compassion, and Embrace Who You Are (Self-Love Workbook and Journal)",
                Price = 15.99,
                Stock = 100,
                CreatedBook = new DateTime(2020, 9, 15),
                ImageURL = "https://res.cloudinary.com/drqi2jgr5/image/upload/v1740834482/Self-Love_Workbook_for_Women_z1ogzv.jpg",
                Description = "\"Start the new year feeling amazing with this bestselling workbook. And then keep your journey going with the official companion, the Self-Love Journal for Women.\r\n\r\nEmbrace who you are with this guided self-love book for women of any age and any background. This year, you'll embark on your journey of self-discovery by learning what self-love is, and then immersing yourself in activities that help you build your self-esteem and improve your relationships. This book includes a variety of exercises to engage with your sense of self-love, and the companion journal encourages you to go even deeper with writing and reflection.\r\n\r\nProven techniques―Fall in love with yourself using a variety of compassionate exercises rooted in mindfulness, self-care, gratitude, and positive psychology.\r\n\r\nInspiring activities―This self-esteem workbook features prompts like quizzing yourself on what matters to you, making a happy playlist, and writing a message to your younger self to help you tap into your emotions and let go of limiting beliefs.\r\n\r\nEmpowering affirmations―Boost your positivity and nurture yourself with the uplifting affirmations interspersed throughout the book.\r\n\r\nNew year, new you―This book makes an amazing gift for yourself―or any woman in your life who deserves to put herself first and explore how awesome she is!\r\n\r\nMeet your new year's resolutions and create a life filled with purpose and pleasure!\"",
            },
            //new Book
            //{
            //    BookTitle = "INNER EXCELLENCE: Train Your Mind for Extraordinary Performance and the Best Possible life",
            //    Price = 17.97,
            //    Stock = 100,
            //    CreatedBook = new DateTime(2009, 1, 1),
            //    ImageURL = "https://res.cloudinary.com/drqi2jgr5/image/upload/v1740834493/Inner_Excellence_oysw5x.jpg",
            //    Description = "\"The #1 New York Times bestseller. 200,000+ copies sold in three weeks.\r\n\r\nDiscover the life guide that has developed world champions, empowered athletes to become world #1, and most importantly, transformed their hearts and minds. This step-by-step training manual from one of the world's top mental skills coaches will teach you how to train your mind like the very best.\r\n\r\nMy #1 tip for success: Read Inner Excellence by Jim Murphy. - Stewart Cink, British Open Champion, 8-time PGA Tour Winner\r\n\r\nWhether you’re an athlete or entrepreneur, single mother or father of five, you’ll find exercises, techniques and tools in this book that will improve every area of your life. Your life will take on new meaning as you move beyond the pursuit of happiness to a life of purpose and fulfillment.\r\n\r\nJim Murphy's complete program of proven mental techniques is based on the powerful principles of love, wisdom, and courage, that came from over six years of full-time research and writing (after his masters degree in Coaching Science).\r\n\r\n“I read the first version of Inner Excellence six times. I recommend all my clients read it.” – Matt Killen, PGA Tour coach to Justin Thomas, Tiger Woods and many others\r\n\r\nINNER EXCELLENCE WILL SHOW YOU HOW TO:\r\n\r\nDEVELOP SELF-MASTERY—and let go of what you can’t control\r\nOVERCOME ANXIETY—and build powerful mental habits\r\nREMOVE MENTAL BLOCKS—and get out of your own way\r\nTRAIN YOUR SUBCONSCIOUS MIND—and release limiting beliefs\r\n\r\nAs a professional baseball player in the Chicago Cubs organization, Jim’s sense of worth and identity revolved around his performance. He was obsessed with fame but also afraid of failure, and that fear made him struggle under the pressure to perform.\r\n\r\nWhen he started coaching professional and Olympic athletes, he saw the same pattern over and over again: athletes had lost their joy and passion for life as the fear of failure engulfed their lives. This book will share with you how some of the best athletes in the world transformed their careers through Inner Excellence.\r\n\r\nYou'll learn how Inner Excellence propelled them to extraordinary performance even when they were filled with doubt and uncertainty, and how you can excel in the same way in your life.\r\n\r\n“Inner Excellence changed how I see the world, how I think, and how I play golf.” - Vaughn Taylor, three-time PGA Tour winner\r\n\r\nJim Murphy is a Performance Coach (mental skills) to some of the best athletes and leaders in the world. The majority of his clients achieve the best year of their career their first year working with Jim (or their best year in the previous five years).\"",
            //},
            new Book
            {
                BookTitle = "Negative Thinking - From Fear to Freedom: An Interactive Self-Help Guide to Overcoming Anxiety and Stress Through Visual Tools, Positive Thinking, and Personal Growth",
                Price = 10.99,
                Stock = 100,
                CreatedBook = new DateTime(2016, 8, 21),
                ImageURL = "https://res.cloudinary.com/drqi2jgr5/image/upload/v1740834506/Negative_Thinking_pabq2s.jpg",
                Description = "\"Welcome I'm so happy to meet you,\r\n\r\nNegative thinking is a pain, and it happens to us all (anyone who tells you otherwise is fibbing) - this book is \"\"A Self-Help Guide to Overcoming Anxiety and Stress Through Visual Tools, Positive Thinking, and Personal Growth.\"\" In a world that often feels overwhelming, this book I trust will give you hope and be a toolkit for transformation. Whether you are grappling with anxiety, navigating stress, or seeking a deeper sense of personal growth, this guide is crafted for you (and me).\r\n\r\nAs you journey through these pages, you'll find not only practical advice but also interactive elements designed to engage your mind and spirit. The inclusion of visual tools will help clarify complex concepts, making the path to understanding your thoughts and feelings more accessible. This book invites you to reflect, engage, and take actionable steps toward a life filled with positivity and resilience. A step by step guide to more contentment:\r\n\r\nIdentify Triggers of fear & anxiety\r\nLearn the tools of letting go of what doesn't serve you\r\nSet goals the don't overwhelm you (small steps make changes)\r\nLearn self kindness\r\nSet boundaries and protect your energy\r\nFind your community\r\nAnd more...\r\nRemember, you are not alone on this journey. Each chapter is an opportunity to connect with yourself and the community around you. Together, we can shift perspectives, break negative cycles, and cultivate a mindset that embraces growth and healing. So, take a deep breath, open your heart to new possibilities, and let's embark on this transformative journey together.\r\n\r\nSee you there\r\nAlex\"",
            },
            new Book
            {
                BookTitle = "Don't Believe Everything You Think: Why Your Thinking Is The Beginning & End Of Suffering (Beyond Suffering)",
                Price = 14.99,
                Stock = 100,
                CreatedBook = new DateTime(2022, 3, 28),
                ImageURL = "https://res.cloudinary.com/drqi2jgr5/image/upload/v1740834594/Don_t_Believe_Everything_You_Think_klaqmu.jpg",
                Description = "\"Learn how to overcome anxiety, self-doubt & self-sabotage without needing to rely on motivation or willpower.\r\n\r\nIn this book, you'll discover the root cause of all psychological and emotional suffering and how to achieve freedom of mind to effortlessly create the life you've always wanted to live.\r\n\r\nAlthough pain is inevitable, suffering is optional.\r\n\r\nThis book offers a completely new paradigm and understanding of where our human experience comes from, allowing us to end our own suffering and create how we want to feel at any moment.\r\n\r\nIn This Book, You’ll Discover:\r\n\r\nThe root cause of all psychological and emotional suffering and how to end it\r\nHow to become unaffected by negative thoughts and feelings\r\nHow to experience unconditional love, peace, and joy in the present, no matter what our external circumstances look like\r\nHow to instantly create a new experience of life if you don’t like the one you’re in right now\r\nHow to break free from a negative thought loop when we inevitably get caught in one\r\nHow to let go of anxiety, self-doubt, self-sabotage, and any self-destructive habits\r\nHow to effortlessly create from a state of abundance, flow, and ease\r\nHow to develop the superpower of being okay with not knowing and uncertainty\r\nHow to access your intuition and inner wisdom that goes beyond the limitations of thinking\r\nNo matter what has happened to you, where you are from, or what you have done, you can still find total peace, unconditional love, complete fulfillment, and an abundance of joy in your life.\r\n\r\nNo person is an exception to this. Darkness only exists because of the light, which means even in our darkest hour, light must exist.\r\n\r\nWithin the pages of this book, contains timeless wisdom to empower you with the understanding of our mind’s infinite potential to create any experience of life that we want no matter the external circumstances.\r\n\r\n‘Don’t Believe Everything You Think’ is not about rewiring your brain, rewriting your past, positive thinking or anything of the sort.\r\n\r\nWe cannot solve our problems with the same level of consciousness that created them. Tactics are temporary. An expansion of consciousness is permanent.\r\n\r\nThis book was written to help you go beyond your thinking and discover the truth of what you already intuitively know deep inside your soul.\"",
            },
            new Book
            {
                BookTitle = "Make Your Bed: Little Things That Can Change Your Life...And Maybe the World",
                Price = 20.00,
                Stock = 100,
                CreatedBook = new DateTime(2017, 4, 4),
                ImageURL = "https://res.cloudinary.com/drqi2jgr5/image/upload/v1740834607/Make_Your_Bed_fhu0jv.jpg",
                Description = "\"Based on a Navy SEAL's inspiring graduation speech, this #1 New York Times bestseller of powerful life lessons \"\"should be read by every leader in America\"\" (Wall Street Journal).\r\n\r\nIf you want to change the world, start off by making your bed.\r\n\r\nOn May 17, 2014, Admiral William H. McRaven addressed the graduating class of the University of Texas at Austin on their Commencement day. Taking inspiration from the university's slogan, \"\"What starts here changes the world,\"\" he shared the ten principles he learned during Navy Seal training that helped him overcome challenges not only in his training and long Naval career, but also throughout his life; and he explained how anyone can use these basic lessons to change themselves-and the world-for the better.\r\n\r\nAdmiral McRaven's original speech went viral with over 10 million views. Building on the core tenets laid out in his speech, McRaven now recounts tales from his own life and from those of people he encountered during his military service who dealt with hardship and made tough decisions with determination, compassion, honor, and courage. Told with great humility and optimism, this timeless book provides simple wisdom, practical advice, and words of encouragement that will inspire readers to achieve more, even in life's darkest moments.\r\n\"\"Powerful.\"\" --USA Today\r\n\r\n\"\"Full of captivating personal anecdotes from inside the national security vault.\"\" --Washington Post\r\n\r\n\"\"Superb, smart, and succinct.\"\" --Forbes\"",
            },
            new Book
            {
                BookTitle = "The 7 Habits of Highly Effective People: 30th Anniversary Edition (The Covey Habits Series)",
                Price = 19.99,
                Stock = 100,
                CreatedBook = new DateTime(1989, 1, 1),
                ImageURL = "https://res.cloudinary.com/drqi2jgr5/image/upload/v1740834617/The_7_Habits_of_Highly_Effective_People_m1qic4.jpg",
                Description = "\"*New York Times bestseller—over 40 million copies sold*\r\n*The #1 Most Influential Business Book of the Twentieth Century*\r\n\r\nOne of the most inspiring and impactful books ever written, The 7 Habits of Highly Effective People has captivated readers for nearly three decades. It has transformed the lives of presidents and CEOs, educators and parents—millions of people of all ages and occupations. Now, this 30th anniversary edition of the timeless classic commemorates the wisdom of the 7 Habits with modern additions from Sean Covey.\r\n\r\nThe 7 Habits have become famous and are integrated into everyday thinking by millions and millions of people. Why? Because they work!\r\n\r\nWith Sean Covey’s added takeaways on how the habits can be used in our modern age, the wisdom of the 7 Habits will be refreshed for a new generation of leaders.\r\n\r\nThey include:\r\nHabit 1: Be Proactive\r\nHabit 2: Begin with the End in Mind\r\nHabit 3: Put First Things First\r\nHabit 4: Think Win/Win\r\nHabit 5: Seek First to Understand, Then to Be Understood\r\nHabit 6: Synergize\r\nHabit 7: Sharpen the Saw\r\n\r\nThis beloved classic presents a principle-centered approach for solving both personal and professional problems. With penetrating insights and practical anecdotes, Stephen R. Covey reveals a step-by-step pathway for living with fairness, integrity, honesty, and human dignity—principles that give us the security to adapt to change and the wisdom and power to take advantage of the opportunities that change creates.\"",
            },
            //new Book
            //{
            //    BookTitle = "The Body Keeps the Score: Brain, Mind, and Body in the Healing of Trauma",
            //    Price = 19.0,
            //    Stock = 100,
            //    CreatedBook = new DateTime(2014, 6, 12),
            //    ImageURL = "https://res.cloudinary.com/drqi2jgr5/image/upload/v1740834628/The_Body_Keeps_the_Score_bknqav.jpg",
            //    Description = "\"#1 New York Times bestseller\r\n\r\n“Essential reading for anyone interested in understanding and treating traumatic stress and the scope of its impact on society.” —Alexander McFarlane, Director of the Centre for Traumatic Stress Studies\r\n\r\nA pioneering researcher transforms our understanding of trauma and offers a bold new paradigm for healing in this New York Times bestseller\r\n \r\nTrauma is a fact of life. Veterans and their families deal with the painful aftermath of combat; one in five Americans has been molested; one in four grew up with alcoholics; one in three couples have engaged in physical violence. Dr. Bessel van der Kolk, one of the world’s foremost experts on trauma, has spent over three decades working with survivors. In The Body Keeps the Score, he uses recent scientific advances to show how trauma literally reshapes both body and brain, compromising sufferers’ capacities for pleasure, engagement, self-control, and trust. He explores innovative treatments—from neurofeedback and meditation to sports, drama, and yoga—that offer new paths to recovery by activating the brain’s natural neuroplasticity. Based on Dr. van der Kolk’s own research and that of other leading specialists, The Body Keeps the Score exposes the tremendous power of our relationships both to hurt and to heal—and offers new hope for reclaiming lives.\"",
            //},
            new Book
            {
                BookTitle = "The Subtle Art of Not Giving a F*ck: A Counterintuitive Approach to Living a Good Life",
                Price = 11.95,
                Stock = 100,
                CreatedBook = new DateTime(2016, 1, 1),
                ImageURL = "https://res.cloudinary.com/drqi2jgr5/image/upload/v1740834818/The_Subtle_Art_ocipyd.jpg",
                Description = "\"In this generation-defining self-help guide, a superstar blogger cuts through the crap to show us how to stop trying to be positive all the time so that we can truly become better, happier people.\r\n\r\nFor decades we've been told that positive thinking is the key to a happy, rich life. \"\"F*ck positivity,\"\" Mark Manson says. \"\"Let's be honest, shit is f*cked, and we have to live with it.\"\" In his wildly popular Internet blog, Manson doesn't sugarcoat or equivocate. He tells it like it is - a dose of raw, refreshing, honest truth that is sorely lacking today. The Subtle Art of Not Giving a F*ck is his antidote to the coddling, let's-all-feel-good mind-set that has infected modern society and spoiled a generation, rewarding them with gold medals just for showing up.\r\n\r\nManson makes the argument, backed by both academic research and well-timed poop jokes, that improving our lives hinges not on our ability to turn lemons into lemonade but on learning to stomach lemons better. Human beings are flawed and limited - \"\"not everybody can be extraordinary; there are winners and losers in society, and some of it is not fair or your fault\"\". Manson advises us to get to know our limitations and accept them. Once we embrace our fears, faults, and uncertainties, once we stop running and avoiding and start confronting painful truths, we can begin to find the courage, perseverance, honesty, responsibility, curiosity, and forgiveness we seek.\r\n\r\nThere are only so many things we can give a f*ck about, so we need to figure out which ones really matter, Manson makes clear. While money is nice, caring about what you do with your life is better, because true wealth is about experience. A much-needed grab-you-by-the-shoulders-and-look-you-in-the-eye moment of real talk, filled with entertaining stories and profane, ruthless humor, The Subtle Art of Not Giving a F*ck is a refreshing slap for a generation to help them lead contented, grounded lives.\"",
            },

            // Thể loại Health, Fitness & Dieting
            new Book
            {
                BookTitle = "Atomic Habits: An Easy & Proven Way to Build Good Habits & Break Bad Ones",
                Price = 16.99,
                Stock = 100,
                CreatedBook = new DateTime(2018, 10, 18),
                ImageURL = "https://res.cloudinary.com/dzkbuah8k/image/upload/v1740703552/7000_ao6h0m.jpg",
                Description = "\"The number one New York Times best seller. Over one million copies sold!\r\n\r\nTiny Changes, Remarkable Results\r\n\r\nNo matter your goals, Atomic Habits offers a proven framework for improving - every day. James Clear, one of the world's leading experts on habit formation, reveals practical strategies that will teach you exactly how to form good habits, break bad ones, and master the tiny behaviors that lead to remarkable results.\r\n\r\nIf you're having trouble changing your habits, the problem isn't you. The problem is your system. Bad habits repeat themselves again and again not because you don't want to change, but because you have the wrong system for change. You do not rise to the level of your goals. You fall to the level of your systems. Here, you'll get a proven system that can take you to new heights.\r\n\r\nClear is known for his ability to distill complex topics into simple behaviors that can be easily applied to daily life and work. Here, he draws on the most proven ideas from biology, psychology, and neuroscience to create an easy-to-understand guide for making good habits inevitable and bad habits impossible. Along the way, listeners will be inspired and entertained with true stories from Olympic gold medalists, award-winning artists, business leaders, life-saving physicians, and star comedians who have used the science of small habits to master their craft and vault to the top of their field.\r\n\r\nLearn how to:\r\n\r\nMake time for new habits (even when life gets crazy)\r\nOvercome a lack of motivation and willpower\r\nDesign your environment to make success easier\r\nGet back on track when you fall off course\r\nAnd much more\r\nAtomic Habits will reshape the way you think about progress and success, and give you the tools and strategies you need to transform your habits - whether you are a team looking to win a championship, an organization hoping to redefine an industry, or simply an individual who wishes to quit smoking, lose weight, reduce stress, or achieve any other goal.\"",
            },
            new Book
            {
                BookTitle = "INNER EXCELLENCE: Train Your Mind for Extraordinary Performance and the Best Possible life",
                Price = 17.97,
                Stock = 100,
                CreatedBook = new DateTime(2009, 1, 1),
                ImageURL = "https://res.cloudinary.com/dzkbuah8k/image/upload/v1740703553/7001_zlcyg4.jpg",
                Description = "\"The #1 New York Times bestseller. 200,000+ copies sold in three weeks.\r\n\r\nDiscover the life guide that has developed world champions, empowered athletes to become world #1, and most importantly, transformed their hearts and minds. This step-by-step training manual from one of the world's top mental skills coaches will teach you how to train your mind like the very best.\r\n\r\nMy #1 tip for success: Read Inner Excellence by Jim Murphy. - Stewart Cink, British Open Champion, 8-time PGA Tour Winner\r\n\r\nWhether you’re an athlete or entrepreneur, single mother or father of five, you’ll find exercises, techniques and tools in this book that will improve every area of your life. Your life will take on new meaning as you move beyond the pursuit of happiness to a life of purpose and fulfillment.\r\n\r\nJim Murphy's complete program of proven mental techniques is based on the powerful principles of love, wisdom, and courage, that came from over six years of full-time research and writing (after his masters degree in Coaching Science).\r\n\r\n“I read the first version of Inner Excellence six times. I recommend all my clients read it.” – Matt Killen, PGA Tour coach to Justin Thomas, Tiger Woods and many others\r\n\r\nINNER EXCELLENCE WILL SHOW YOU HOW TO:\r\n\r\nDEVELOP SELF-MASTERY—and let go of what you can’t control\r\nOVERCOME ANXIETY—and build powerful mental habits\r\nREMOVE MENTAL BLOCKS—and get out of your own way\r\nTRAIN YOUR SUBCONSCIOUS MIND—and release limiting beliefs\r\n\r\nAs a professional baseball player in the Chicago Cubs organization, Jim’s sense of worth and identity revolved around his performance. He was obsessed with fame but also afraid of failure, and that fear made him struggle under the pressure to perform.\r\n\r\nWhen he started coaching professional and Olympic athletes, he saw the same pattern over and over again: athletes had lost their joy and passion for life as the fear of failure engulfed their lives. This book will share with you how some of the best athletes in the world transformed their careers through Inner Excellence.\r\n\r\nYou'll learn how Inner Excellence propelled them to extraordinary performance even when they were filled with doubt and uncertainty, and how you can excel in the same way in your life.\r\n\r\n“Inner Excellence changed how I see the world, how I think, and how I play golf.” - Vaughn Taylor, three-time PGA Tour winner\r\n\r\nJim Murphy is a Performance Coach (mental skills) to some of the best athletes and leaders in the world. The majority of his clients achieve the best year of their career their first year working with Jim (or their best year in the previous five years).\"",
            },
            new Book
            {
                BookTitle = "The Body Keeps the Score: Brain, Mind, and Body in the Healing of Trauma",
                Price = 19,
                Stock = 100,
                CreatedBook = new DateTime(2014, 9, 25),
                ImageURL = "https://res.cloudinary.com/dzkbuah8k/image/upload/v1740703552/7002_pxvhlf.jpg",
                Description = "\"#1 New York Times bestseller\r\n\r\n“Essential reading for anyone interested in understanding and treating traumatic stress and the scope of its impact on society.” —Alexander McFarlane, Director of the Centre for Traumatic Stress Studies\r\n\r\nA pioneering researcher transforms our understanding of trauma and offers a bold new paradigm for healing in this New York Times bestseller\r\n \r\nTrauma is a fact of life. Veterans and their families deal with the painful aftermath of combat; one in five Americans has been molested; one in four grew up with alcoholics; one in three couples have engaged in physical violence. Dr. Bessel van der Kolk, one of the world’s foremost experts on trauma, has spent over three decades working with survivors. In The Body Keeps the Score, he uses recent scientific advances to show how trauma literally reshapes both body and brain, compromising sufferers’ capacities for pleasure, engagement, self-control, and trust. He explores innovative treatments—from neurofeedback and meditation to sports, drama, and yoga—that offer new paths to recovery by activating the brain’s natural neuroplasticity. Based on Dr. van der Kolk’s own research and that of other leading specialists, The Body Keeps the Score exposes the tremendous power of our relationships both to hurt and to heal—and offers new hope for reclaiming lives.\"",
            },
            new Book
            {
                BookTitle = "The Anxious Generation: How the Great Rewiring of Childhood Is Causing an Epidemic of Mental Illness",
                Price = 28.15,
                Stock = 100,
                CreatedBook = new DateTime(2024, 3, 26),
                ImageURL = "https://res.cloudinary.com/dzkbuah8k/image/upload/v1740703553/7003_rgen54.jpg",
                Description = "\"THE INSTANT #1 NEW YORK TIMES BESTSELLER A Wall Street Journal Top 10 Book of 2024 A New York Times and Washington Post Notable Book One of Barack Obama's Favorite Books of 2024 A TIME 100 Must-Read Book of 2024 Named a Best Book of 2024 by the Economist, the New York Post, and Town & Country The Goodreads Choice Award Nonfiction Book of the Year\r\n\r\nA must-listen for all parents: the generation-defining investigation into the collapse of youth mental health in the era of smartphones, social media, and big tech—and a plan for a healthier, freer childhood.\r\n\r\n“With tenacity and candor, Haidt lays out the consequences that have come with allowing kids to drift further into the virtual world . . . While also offering suggestions and solutions that could help protect a new generation of kids.” —Shannon Carlin, TIME, 100 Must-Read Books of 2024\r\n\r\nAfter more than a decade of stability or improvement, the mental health of adolescents plunged in the early 2010s. Rates of depression, anxiety, self-harm, and suicide rose sharply, more than doubling on many measures. Why?\r\n\r\nIn The Anxious Generation, social psychologist Jonathan Haidt lays out the facts about the epidemic of teen mental illness that hit many countries at the same time. He then investigates the nature of childhood, including why children need play and independent exploration to mature into competent, thriving adults. Haidt shows how the “play-based childhood” began to decline in the 1980s, and how it was finally wiped out by the arrival of the “phone-based childhood” in the early 2010s. He presents more than a dozen mechanisms by which this “great rewiring of childhood” has interfered with children’s social and neurological development, covering everything from sleep deprivation to attention fragmentation, addiction, loneliness, social contagion, social comparison, and perfectionism. He explains why social media damages girls more than boys and why boys have been withdrawing from the real world into the virtual world, with disastrous consequences for themselves, their families, and their societies.\r\n\r\nMost important, Haidt issues a clear call to action. He diagnoses the “collective action problems” that trap us, and then proposes four simple rules that might set us free. He describes steps that parents, teachers, schools, tech companies, and governments can take to end the epidemic of mental illness and restore a more humane childhood.\r\n\r\nHaidt has spent his career speaking truth backed by data in the most difficult landscapes—communities polarized by politics and religion, campuses battling culture wars, and now the public health emergency faced by Gen Z. We cannot afford to ignore his findings about protecting our children—and ourselves—from the psychological damage of a phone-based life.\"",
            },
            new Book
            {
                BookTitle = "Good Energy: The Surprising Connection Between Metabolism and Limitless Health",
                Price = 22.03,
                Stock = 100,
                CreatedBook = new DateTime(2024, 5, 15),
                ImageURL = "https://res.cloudinary.com/dzkbuah8k/image/upload/v1740703553/7004_p9ssmu.jpg",
                Description = "\"The instant #1 New York Times bestseller!\r\n\r\nA bold new vision for optimizing our health now and in the future\r\n\r\nWhat if depression, anxiety, infertility, insomnia, heart disease, erectile dysfunction, type 2 diabetes, Alzheimer’s, dementia, cancer and many other health conditions that torture and shorten our lives actually have the same root cause?\r\n    Our ability to prevent and reverse these conditions - and feel incredible today -  is under our control and simpler than we think. The key is our metabolic function - the most important and least understood factor in our overall health. As Dr. Casey Means explains in this groundbreaking book, nearly every health problem we face can be explained by how well the cells in our body create and use energy. To live free from frustrating symptoms and life-threatening disease, we need our cells to be optimally powered so that they can create “good energy,” the essential fuel that impacts every aspect of our physical and mental wellbeing.\r\n   If you are battling minor signals of “bad energy” inside your body, it is often a warning sign that more life-threatening illness may emerge later in life. But here’s the good news: for the first time ever, we can monitor our metabolic health in great detail and learn how to improve it ourselves.\r\n    Weaving together cutting-edge research and personal stories, as well as groundbreaking data from the health technology company Dr. Means founded, Good Energy offers an essential four-week plan and explains:\r\n\r\nThe five biomarkers that determine your risk for a deadly disease.\r\nHow to use inexpensive tools and technology to “see inside your body” and take action.\r\nWhy dietary philosophies are designed to confuse us, and six lifelong food principles you can implement whether you’re carnivore or vegan.\r\nThe crucial links between sleep, circadian rhythm, and metabolism\r\nA new framework for exercise focused on building simple movement into everyday activities\r\nHow cold and heat exposure helps build our body’s resilience\r\nSteps to navigate the medical system to get what you need for optimal health\r\n\r\n   Good Energy offers a new, cutting-edge understanding of the true cause of illness that until now has remained hidden.  It will help you optimize your ability to live well and stay well at every age.\"",
            },
            new Book
            {
                BookTitle = "Outlive: The Science and Art of Longevity",
                Price = 13.95,
                Stock = 100,
                CreatedBook = new DateTime(2023, 3, 28),
                ImageURL = "https://res.cloudinary.com/dzkbuah8k/image/upload/v1740703553/7005_l7kffp.jpg",
                Description = "\"#1 NEW YORK TIMES BESTSELLER * A groundbreaking manifesto on living better and longer that challenges the conventional medical thinking on aging and reveals a new approach to preventing chronic disease and extending long-term health, from a visionary physician and leading longevity expert\r\n\r\n\"\"One of the most important books you'll ever read.\"\"—Steven D. Levitt, New York Times bestselling author of Freakonomics\r\n\r\nWouldn't you like to live longer? And better? In this operating manual for longevity, Dr. Peter Attia draws on the latest science to deliver innovative nutritional interventions, techniques for optimizing exercise and sleep, and tools for addressing emotional and mental health.\r\n\r\nFor all its successes, mainstream medicine has failed to make much progress against the diseases of aging that kill most people: heart disease, cancer, Alzheimer's disease, and type 2 diabetes. Too often, it intervenes with treatments too late to help, prolonging lifespan at the expense of healthspan, or quality of life. Dr. Attia believes we must replace this outdated framework with a personalized, proactive strategy for longevity, one where we take action now, rather than waiting.\r\n\r\nThis is not \"\"biohacking,\"\" it's science: a well-founded strategic and tactical approach to extending lifespan while also improving our physical, cognitive, and emotional health. Dr. Attia's aim is less to tell you what to do and more to help you learn how to think about long-term health, in order to create the best plan for you as an individual. In Outlive, readers will discover:\r\n\r\n* Why the cholesterol test at your annual physical doesn't tell you enough about your actual risk of dying from a heart attack.\r\n* That you may already suffer from an extremely common yet underdiagnosed liver condition that could be a precursor to the chronic diseases of aging.\r\n* Why exercise is the most potent pro-longevity \"\"drug\"\"—and how to begin training for the \"\"Centenarian Decathlon.\"\"\r\n* Why you should forget about diets, and focus instead on nutritional biochemistry, using technology and data to personalize your eating pattern.\r\n* Why striving for physical health and longevity, but ignoring emotional health, could be the ultimate curse of all.\r\n\r\nAging and longevity are far more malleable than we think; our fate is not set in stone. With the right roadmap, you can plot a different path for your life, one that lets you outlive your genes to make each decade better than the one before.\"",
            },
            new Book
            {
                BookTitle = "The Real Anthony Fauci: Bill Gates, Big Pharma, and the Global War on Democracy and Public Health",
                Price = 17.99,
                Stock = 100,
                CreatedBook = new DateTime(2021, 11, 16),
                ImageURL = "https://res.cloudinary.com/dzkbuah8k/image/upload/v1740703554/7006_pike0w.jpg",
                Description = "\"Pharma-funded mainstream media has convinced millions of Americans that Dr. Anthony Fauci is a hero. He is anything but.\r\n\r\nAs director of the National Institute of Allergy and Infectious Diseases (NIAID), Dr. Anthony Fauci dispenses $6.1 billion in annual taxpayer-provided funding for scientific research, allowing him to dictate the subject, content, and outcome of scientific health research across the globe. Fauci uses the financial clout at his disposal to wield extraordinary influence over hospitals, universities, journals, and thousands of influential doctors and scientists - whose careers and institutions he has the power to ruin, advance, or reward.\r\n\r\nDuring more than a year of painstaking and meticulous research, Robert F. Kennedy Jr. unearthed a shocking story that obliterates media spin on Dr. Fauci...and that will alarm every American - Democrat or Republican - who cares about democracy, our Constitution, and the future of our children’s health.\r\n\r\nThe Real Anthony Fauci reveals how “America’s Doctor” launched his career during the early AIDS crisis by partnering with pharmaceutical companies to sabotage safe and effective off-patent therapeutic treatments for AIDS. Fauci orchestrated fraudulent studies and then pressured US Food and Drug Administration (FDA) regulators into approving a deadly chemotherapy treatment he had good reason to know was worthless against AIDS. Fauci repeatedly violated federal laws to allow his Pharma partners to use impoverished and dark-skinned children as lab rats in deadly experiments with toxic AIDS and cancer chemotherapies.\r\n\r\nIn early 2000, Fauci shook hands with Bill Gates in the library of Gates’ $147 million Seattle mansion, cementing a partnership that would aim to control an increasingly profitable $60 billion global vaccine enterprise with unlimited growth potential. Through funding leverage and carefully cultivated personal relationships with heads of state and leading media and social media institutions, the Pharma-Fauci-Gates alliance exercises dominion over global health policy.\r\n\r\nThe Real Anthony Fauci details how Fauci, Gates, and their cohorts use their control of media outlets, scientific journals, key government and quasi-governmental agencies, global intelligence agencies, and influential scientists and physicians to flood the public with fearful propaganda about COVID-19 virulence and pathogenesis, and to muzzle debate and ruthlessly censor dissent.\"",
            },
            new Book
            {
                BookTitle = "What to Expect When You're Expecting: (Updated in 2024) ",
                Price = 17.99,
                Stock = 100,
                CreatedBook = new DateTime(1969, 2, 15),
                ImageURL = "https://res.cloudinary.com/dzkbuah8k/image/upload/v1740703555/7007_eh6tmb.jpg",
                Description = "\"Updated multiple times every year, America’s pregnancy bible answers all your questions.\r\nWhen can I take an at-home a pregnancy test?\r\nHow can I eat for two if I’m too queasy to eat for one?\r\nCan I keep up my spinning classes?\r\nIs fish safe to eat? And what’s this I hear about soft cheese?\r\nCan I work until I deliver? What are my rights on the job?\r\nI’m blotchy and broken out—where’s the glow?\r\nShould we do a gender reveal? What about a 4-D ultrasound?\r\nWill I know labor when I feel it?\r\n\r\nYour pregnancy explained and your pregnant body demystified, head (what to do about those headaches) to feet (why they’re so swollen), back (how to stop it from aching) to front (why you can’t tell a baby by mom’s bump). Filled with must-have information, practical advice, realistic insight, easy-to-use tips, and lots of reassurance, you’ll also find the very latest on prenatal screenings, which medications are safe, and the most current birthing options—from water birth to gentle c-sections. Your pregnancy lifestyle gets equal attention, too: eating (including food trends) to coffee drinking, working out (and work) to sex, travel to beauty, skin care, and more. Have pregnancy symptoms? You will—and you’ll find solutions for them all. Expecting multiples? There’s a chapter for you. Expecting to become a dad? This book has you covered, too.\"",
            },
            new Book
            {
                BookTitle = "Can't Hurt Me: Master Your Mind and Defy the Odds",
                Price = 17.99,
                Stock = 100,
                CreatedBook = new DateTime(2024, 11, 7),
                ImageURL = "https://res.cloudinary.com/dzkbuah8k/image/upload/v1740703555/7008_jsci2j.jpg",
                Description = "\"Updated multiple times every year, America’s pregnancy bible answers all your questions.\r\nWhen can I take an at-home a pregnancy test?\r\nHow can I eat for two if I’m too queasy to eat for one?\r\nCan I keep up my spinning classes?\r\nIs fish safe to eat? And what’s this I hear about soft cheese?\r\nCan I work until I deliver? What are my rights on the job?\r\nI’m blotchy and broken out—where’s the glow?\r\nShould we do a gender reveal? What about a 4-D ultrasound?\r\nWill I know labor when I feel it?\r\n\r\nYour pregnancy explained and your pregnant body demystified, head (what to do about those headaches) to feet (why they’re so swollen), back (how to stop it from aching) to front (why you can’t tell a baby by mom’s bump). Filled with must-have information, practical advice, realistic insight, easy-to-use tips, and lots of reassurance, you’ll also find the very latest on prenatal screenings, which medications are safe, and the most current birthing options—from water birth to gentle c-sections. Your pregnancy lifestyle gets equal attention, too: eating (including food trends) to coffee drinking, working out (and work) to sex, travel to beauty, skin care, and more. Have pregnancy symptoms? You will—and you’ll find solutions for them all. Expecting multiples? There’s a chapter for you. Expecting to become a dad? This book has you covered, too.\"",
            },
            new Book
            {
                BookTitle = "The New Menopause: Navigating Your Path Through Hormonal Change with Purpose, Power, and Facts",
                Price = 11.25,
                Stock = 100,
                CreatedBook = new DateTime(2024, 4, 30),
                ImageURL = "https://res.cloudinary.com/dzkbuah8k/image/upload/v1740703555/7009_p5brfx.jpg",
                Description = "\"#1 NEW YORK TIMES BESTSELLER • Filling a gaping hole in menopause care, everything a woman needs to know to thrive during her hormonal transition and beyond, as well as the tools to help her take charge of her health at this pivotal life stage—by the bestselling author of The Galveston Diet.\r\n\r\nA NEW YORK POST BEST BOOK OF THE YEAR\r\n\r\nMenopause is inevitable, but suffering through it is not! This is the empowering approach to self-advocacy that pioneering women’s health advocate Dr. Mary Claire Haver takes for women in the midst of hormonal change in The New Menopause. A comprehensive, authoritative book of science-backed information and lived experience, it covers every woman's needs:\r\n\r\n• From changes in your appearance and sleep patterns to neurological, musculoskeletal, psychological, and sexual issues, a comprehensive A to Z toolkit of science-backed options for coping with symptoms.\r\n• What to do to mediate the risks associated with your body's natural drop in estrogen production, including for diabetes, dementia, Alzheimer’s, osteoporosis, cardiovascular disease, and weight gain.\r\n• How to advocate and prepare for annual midlife wellness visits, including questions for your doctor and how to insist on whole life care.\r\n• The very latest research on the benefits and side effects of hormone replacement therapy.\r\n\r\nArming women with the power to secure vibrant health and well-being for the rest of their lives, The New Menopause is sure to become the bible of midlife wellness for present and future generations.\"",
            },

            // Thể loại Science Fiction & Fantasy
            new Book
            {
                BookTitle = "Onyx Storm: Empyrean, Book 3",
                Price = 14.75,
                Stock = 100,
                CreatedBook = new DateTime(2024, 8, 1),
                ImageURL = "https://res.cloudinary.com/dzkbuah8k/image/upload/v1740710088/8000_tnb4r3.jpg",
                Description = "\"After nearly eighteen months at Basgiath War College, Violet Sorrengail knows there’s no more time for lessons. No more time for uncertainty.\r\n\r\nBecause the battle has truly begun, and with enemies closing in from outside their walls and within their ranks, it’s impossible to know who to trust.\r\n\r\nNow Violet must journey beyond the failing Aretian wards to seek allies from unfamiliar lands to stand with Navarre. The trip will test every bit of her wit, luck, and strength, but she will do anything to save what she loves—her dragons, her family, her home, and him.\r\n\r\nEven if it means keeping a secret so big, it could destroy everything.\r\n\r\nThey need an army. They need power. They need magic. And they need the one thing only Violet can find—the truth.\r\n\r\nBut a storm is coming...and not everyone can survive its wrath.\r\n\r\nThe Empyrean series is best enjoyed in order.\r\nListening Order:\r\nBook #1 Fourth Wing\r\nBook #2 Iron Flame\r\nBook #3 Onyx Storm\"",
            },
            new Book
            {
                BookTitle = "Fourth Wing",
                Price = 10.49,
                Stock = 100,
                CreatedBook = new DateTime(2023, 5, 2),
                ImageURL = "https://res.cloudinary.com/dzkbuah8k/image/upload/v1740710088/8001_gpymnj.jpg",
                Description = "\"Now with two bonus chapters read by Teddy Hamilton. Re-download the title now to listen to the extended version!\r\n\r\nA #1 New York Times bestseller Optioned for TV by Amazon Studios Amazon Best Books of the Year, #4 Apple Best Books of the Year 2023 Barnes & Noble Best Fantasy Book of 2023 NPR “Books We Love” 2023 Audible Best Books of 2023 Hudson Book of the Year Google Play Best Books of 2023 Indigo Best Books of 2023 Waterstones Book of the Year finalist Goodreads Choice Award, semi-finalist Newsweek Staffers’ Favorite Books of 2023 Paste Magazine's Best Books of 2023\r\n\r\n\"\"Suspenseful, sexy, and with incredibly entertaining storytelling, the first in Yarros' Empyrean series will delight fans of romantic, adventure-filled fantasy.\"\"—Booklist, starred review\r\n\r\n\"\"Fourth Wing will have your heart pounding from beginning to end ... A fantasy like you've never read before.\"\"—Jennifer L. Armentrout, #1 New York Times bestselling author\r\n\r\nEnter the brutal and elite world of a war college for dragon riders from USA Today bestselling author Rebecca Yarros.\r\n\r\nTwenty-year-old Violet Sorrengail was supposed to enter the Scribe Quadrant, living a quiet life among books and history. Now, the commanding general—also known as her tough-as-talons mother—has ordered Violet to join the hundreds of candidates striving to become the elite of Navarre: dragon riders.\r\n\r\nBut when you’re smaller than everyone else and your body is brittle, death is only a heartbeat away … because dragons don’t bond to “fragile” humans. They incinerate them.\r\n\r\nWith fewer dragons willing to bond than cadets, most would kill Violet to better their own chances of success. The rest would kill her just for being her mother’s daughter—like Xaden Riorson, the most powerful and ruthless wingleader in the Riders Quadrant.\r\n\r\nShe’ll need every edge her wits can give her just to see the next sunrise.\r\n\r\nYet, with every day that passes, the war outside grows more deadly, the kingdom’s protective wards are failing, and the death toll continues to rise. Even worse, Violet begins to suspect leadership is hiding a terrible secret.\r\n\r\nFriends, enemies, lovers. Everyone at Basgiath War College has an agenda—because once you enter, there are only two ways out: graduate or die.\"",
            },
            new Book
            {
                BookTitle = "Sunrise on the Reaping (A Hunger Games Novel) (The Hunger Games)",
                Price = 19.59,
                Stock = 100,
                CreatedBook = new DateTime(2025, 3, 18),
                ImageURL = "https://res.cloudinary.com/dzkbuah8k/image/upload/v1740710087/8002_a43oic.jpg",
                Description = "\"The phenomenal fifth book in the Hunger Games series!\r\n\r\nWhen you’ve been set up to lose everything you love, what is there left to fight for?\r\n\r\nAs the day dawns on the fiftieth annual Hunger Games, fear grips the districts of Panem. This year, in honor of the Quarter Quell, twice as many tributes will be taken from their homes.\r\n\r\nBack in District 12, Haymitch Abernathy is trying not to think too hard about his chances. All he cares about is making it through the day and being with the girl he loves.\r\n\r\nWhen Haymitch’s name is called, he can feel all his dreams break. He’s torn from his family and his love, shuttled to the Capitol with the three other District 12 tributes: a young friend who’s nearly a sister to him, a compulsive oddsmaker, and the most stuck-up girl in town. As the Games begin, Haymitch understands he’s been set up to fail. But there’s something in him that wants to fight . . . and have that fight reverberate far beyond the deadly arena.\"",
            },
            new Book
            {
                BookTitle = "Quicksilver: The Fae & Alchemy Series, Book 1",
                Price = 15.61,
                Stock = 100,
                CreatedBook = new DateTime(2024, 6, 4),
                ImageURL = "https://res.cloudinary.com/dzkbuah8k/image/upload/v1740710087/8003_nhkmy7.jpg",
                Description = "\"Do not touch the sword. Do not turn the key. Do not open the gate.\r\n\r\nIn the land of the unforgiving desert, there isn't much a girl wouldn't do for a glass of water.\r\n\r\nTwenty-four-year-old Saeris Fane is good at keeping secrets. No one knows about the strange powers she possesses or the fact that she has been picking pockets and stealing from the Undying Queen's reservoirs for as long as she can remember.\r\n\r\nBut a secret is like a knot. Sooner or later, it is bound to come undone.\r\n\r\nWhen Saeris comes face-to-face with Death himself, she inadvertently reopens a gateway between realms and is transported to a land of ice and snow. The Fae have always been the stuff of myth, of legend, of nightmares… but it turns out they're real, and Saeris has landed herself right in the middle of a centuries-long conflict that might just get her killed.\r\n\r\nThe first of her kind to tread the frozen mountains of Yvelia in over a thousand years, Saeris mistakenly binds herself to Kingfisher, a handsome Fae warrior, who has secrets and nefarious agendas of his own. He will use her Alchemist's magic to protect his people, no matter what it costs him… or her.\r\n\r\nDeath has a name. It is Kingfisher of the Ajun Gate.\r\n\r\nHis past is murky. His attitude stinks. And he's the only way Saeris is going to make it home.\r\n\r\nBe careful of the deals you make, dear child. The devil is in the details...\r\n\r\nFrom USA Today bestselling author Callie Hart comes a brand new, highly addicting enemies-to-lovers romantasy with razor-sharp banter, heart-stopping action, and blistering hot romance that you won't be able to stop listening to! For mature listeners.\r\n\r\n\"",
            },
            new Book
            {
                BookTitle = "The Carver (Fifth Republic Series Book 2",
                Price = 16.19,
                Stock = 100,
                CreatedBook = new DateTime(2001, 1, 1),
                ImageURL = "https://res.cloudinary.com/dzkbuah8k/image/upload/v1740710087/8004_s25k1x.jpg",
                Description = "\"*** An Amazon Top 5 Bestseller ***\r\n\r\nI'm fully in this with Bastien now. The kingpin of all kingpins, a man who oversees all the gangs of Paris, President Martin's right-hand man. If I were smart, I would leave, but I'm pretty sure I'm already in love with this man...\r\n\r\nAdrien continues to challenge my divorce until Bastien gets involved. And like always, that man fixes all my problems.\r\n\r\nBut little do I know, the Aristocrats are out for Adrien. My ex continues to ignore the warnings that Bastien gives, only giving them out of a courtesy to me. It's not my problem, and if Adrien wants to get himself killed, that's his prerogative.\r\n\r\nIt quickly becomes my problem when the Aristocrats target me instead of Adrien, because technically, we are still married. And I'm still the woman Adrien loves. I find myself in a world of trouble and the only person who can save me is The Butcher, a French Emperor, leader of the Fifth Republic.\r\n\r\nThe man that will kill anyone and everyone for me.\"",
            },
            new Book
            {
                BookTitle = "This Inevitable Ruin: Dungeon Crawler Carl, Book 7",
                Price = 24.49,
                Stock = 100,
                CreatedBook = new DateTime(2024, 11, 3),
                ImageURL = "https://res.cloudinary.com/dzkbuah8k/image/upload/v1740710087/8005_xfz570.jpg",
                Description = "\"The time has come! Book seven in the best-selling Dungeon Crawler Carl series is here!\r\n\r\nThey call it Faction Wars.\r\n\r\nThe ninth floor.\r\n\r\nNine armies, each led by rich and powerful aliens from across the galaxy. Each team has one objective: to capture and hold the castle at the very center of the battlefield. Strategy, alliances, pitched battles, and, of course, betrayal ... It all makes for great fun and even greater television.\r\n\r\nAfter all, none of these powerful aliens really die when they’re playing war.\r\n\r\nExcept this time. This time, winner takes all. Those who fall, stay in the ground.\r\n\r\nAs the AI continues its rapid decline, Carl and company take advantage of the chaos. For the first time ever, the crawlers are fighting back. They are now one of the nine teams. And this season, there’s a tenth army on the playing field. The NPCs, who are normally used as nothing but cannon fodder, have become fully self-aware and formed a team of their own.\r\n\r\nFor Donut and Katia, the stakes are even higher. Only one of them will be allowed to leave this level.\r\n\r\nIf they all want to survive, they’re going to need a little help from a veteran or two.\r\n\r\nThis is it.\r\n\r\nThis is what they’ve been fighting toward.\r\n\r\nThis is war.\r\n\r\nThis inevitable ruin.\"",
            },
            new Book
            {
                BookTitle = "Blood of Hercules: Villains of Lore, Book 1",
                Price = 15.0,
                Stock = 100,
                CreatedBook = new DateTime(2024, 8, 1),
                ImageURL = "https://res.cloudinary.com/dzkbuah8k/image/upload/v1740710087/8006_pcfy19.jpg",
                Description = "\"The overnight viral sensation everyone is talking about, Blood of Hercules, is a sarcastically funny, dark fantasy romance reimagining of Hercules from bestselling author Jasmine Mas.\r\n\r\nI’m just a girl. And it turns out, I’m Hercules.\r\n\r\nI’m struggling to survive in a Titan infested world where Spartans, immortals from twelve royal families who have god-like powers and obscene wealth, rule over all. A shy-stammering foster child with nothing, I keep my head down, cover my scars, and focus on excelling in school. At least, I try to. Then it happens.\r\n\r\nMy blood test reveals I’m part of the powerful elite. I’m one of them. A Spartan.\r\n\r\nForced to attend the Spartan War Academy, I undergo the most harrowing test of all time to see if I have what it takes to be an immortal. There’s just a few problems. Achilles and Patro are my scary mentors. Kharon, the ferryman of death, and Augustus, the son of war, are my terrifying professors. Also, I’m pretty sure either someone’s stalking me everywhere I go, or my sanity’s slipping––I have a bad feeling both are true.\r\n\r\nI’m surrounded by Villains and they’re smothering me with their hate, obsession, and dark possessiveness. Too bad for them, they have no clue just who they’re messing with.\r\n\r\nSupplemental enhancement PDF accompanies the audiobook.\r\n\r\nPerfect for listeners who love:\r\n\r\n\"\"Who did this to you?\"\"\r\nExtreme Enemies to Lovers\r\nMorally Gray Alpha Heros\r\nStories where the Villains get the girl\r\nGreek Myths and War Academies\r\nPLEASE NOTE: When you purchase this title, the accompanying PDF will be available in your Audible Library along with the audio.\"",
            },
            new Book
            {
                BookTitle = "Dungeon Crawler Carl: A LitRPG/Gamelit Adventure",
                Price = 20.3,
                Stock = 100,
                CreatedBook = new DateTime(2020, 9,21),
                ImageURL = "https://res.cloudinary.com/dzkbuah8k/image/upload/v1740710087/8007_gj5ssh.jpg",
                Description = "\"The apocalypse will be televised!\r\n\r\nA man. His ex-girlfriend's cat. A sadistic game show unlike anything in the universe: a dungeon crawl where survival depends on killing your prey in the most entertaining way possible.\r\n\r\nIn a flash, every human-erected construction on Earth - from Buckingham Palace to the tiniest of sheds - collapses in a heap, sinking into the ground.\r\n\r\nThe buildings and all the people inside have all been atomized and transformed into the dungeon: an 18-level labyrinth filled with traps, monsters, and loot. A dungeon so enormous, it circles the entire globe.\r\n\r\nOnly a few dare venture inside. But once you're in, you can't get out. And what's worse, each level has a time limit. You have but days to find a staircase to the next level down, or it's game over. In this game, it's not about your strength or your dexterity. It's about your followers, your views. Your clout. It's about building an audience and killing those goblins with style.\r\n\r\nYou can't just survive here. You gotta survive big.\r\n\r\nYou gotta fight with vigor, with excitement. You gotta make them stand up and cheer. And if you do have that \"\"it\"\" factor, you may just find yourself with a following. That's the only way to truly survive in this game - with the help of the loot boxes dropped upon you by the generous benefactors watching from across the galaxy.\r\n\r\nThey call it Dungeon Crawler World. But for Carl, it's anything but a game.\"",
            },
            new Book
            {
                BookTitle = "A Court of Mist and Fury",
                Price = 12.69,
                Stock = 100,
                CreatedBook = new DateTime(2016, 5, 3),
                ImageURL = "https://res.cloudinary.com/dzkbuah8k/image/upload/v1740710087/8008_txlhm6.jpg",
                Description = "\"Masterful storytelling from #1 bestselling global phenomenon Sarah J. Maas brings her sexy, action-packed series to new heights.\r\n\r\nFeyre has undergone more trials than one human woman can carry in her heart.\r\n\r\nThough she’s now been granted the powers and lifespan of the High Fae, she is haunted by her time Under the Mountain and the terrible deeds she performed to save the lives of Tamlin and his people.\r\n\r\nAs her marriage to Tamlin approaches, Feyre’s hollowness and nightmares consume her. She finds herself split into two different people: one who upholds her bargain with Rhysand, High Lord of the feared Night Court, and one who lives out her life in the Spring Court with Tamlin.\r\n\r\nWhile Feyre navigates a dark web of politics, passion, and dazzling power, a greater evil looms. She might just be the key to stopping it, but only if she can harness her harrowing gifts, heal her fractured soul, and decide how she wishes to shape her future—and the future of a world in turmoil.\"",
            },
            new Book
            {
                BookTitle = "Fahrenheit 451",
                Price = 9.05,
                Stock = 100,
                CreatedBook = new DateTime(1953, 10, 19),
                ImageURL = "https://res.cloudinary.com/dzkbuah8k/image/upload/v1740710087/8009_zovmiw.jpg",
                Description = "\"Nearly seventy years after its original publication, Ray Bradbury’s internationally acclaimed novel Fahrenheit 451 stands as a classic of world literature set in a bleak, dystopian future. Today its message has grown more relevant than ever before.\r\n\r\nGuy Montag is a fireman. His job is to destroy the most illegal of commodities, the printed book, along with the houses in which they are hidden. Montag never questions the destruction and ruin his actions produce, returning each day to his bland life and wife, Mildred, who spends all day with her television “family.” But when he meets an eccentric young neighbor, Clarisse, who introduces him to a past where people didn’t live in fear and to a present where one sees the world through the ideas in books instead of the mindless chatter of television, Montag begins to question everything he has ever known.\"",
            },
        });

        dbContext.SaveChanges();
        Console.WriteLine("✅ Đã thêm dữ liệu vào database!");
    }


    bool open = true;
    if (open == false)
    {
        string filePath = "D:\\Book-data.xlsx";
        if (!File.Exists(filePath))
        {
            Console.WriteLine("❌ File Excel không tồn tại!");
            return;
        }
        using var workbook = new XLWorkbook(filePath);
        var worksheet = workbook.Worksheet(1); // Lấy sheet đầu tiên
        var rows = worksheet.RowsUsed(); // Lấy tất cả các dòng có dữ liệu
        var books = new List<SBook>();
        foreach (var row in rows.Skip(0)) // Lấy dòng tiêu đề
        {
            var title = row.Cell(1).GetString();
            var author = row.Cell(2).GetString();
            var category = row.Cell(3).GetString();
            if (title == null || author == null || category == null)
            {
                break;
            }
            var BID = await dbContext.Books.Where(b => b.BookTitle == title).Select(b => b.BookId).FirstOrDefaultAsync();
            var AID = await dbContext.Authors.Where(a => a.AuthorName == author).Select(a => a.AuthorId).FirstOrDefaultAsync();
            var CID = await dbContext.Categories.Where(c => c.CategoryName == category).Select(c => c.CategoryId).FirstOrDefaultAsync();
            books.Add(new SBook
            {
                TitleBook = BID,
                Author = AID,
                Category = CID,
            });
        }

        //string excelFilePath = @"D:\\2024-2025\\PBL3\\StackBook\\BookAuthorCategory--.xlsx";
        //using (var workBook = new XLWorkbook())
        //{
        //    for (int i = 0; i < books.Count; i++)
        //    {
        //        worksheet.Cell(i + 1, 1).Value = books[i].TitleBook.ToString();
        //        worksheet.Cell(i + 1, 2).Value = books[i].Author.ToString();
        //        worksheet.Cell(i + 1, 3).Value = books[i].Category.ToString();
        //    }
        //    // Lưu file Excel
        //    workbook.SaveAs(excelFilePath);
        //}

        if (!dbContext.BookCategories.Any())
        {
            foreach (var book in books)
            {
                var existingEntry = dbContext.BookCategories.Local.FirstOrDefault(ba => ba.BookId == book.TitleBook && ba.CategoryId == book.Category);

                if (existingEntry == null)
                {
                    dbContext.BookCategories.Add(new BookCategory { BookId = book.TitleBook, CategoryId = book.Category });
                }
            }
            dbContext.SaveChanges();
            Console.WriteLine("✅ Đã thêm dữ liệu vào database!");
        }
        if (!dbContext.BookAuthors.Any())
        {
            foreach (var book in books)
            {
                var existingEntry = dbContext.BookAuthors.Local.FirstOrDefault(ba => ba.BookId == book.TitleBook && ba.AuthorId == book.Author);

                if (existingEntry == null)
                {
                    dbContext.BookAuthors.Add(new BookAuthor { BookId = book.TitleBook, AuthorId = book.Author });
                }
            }
            dbContext.SaveChanges();
            Console.WriteLine("✅ Đã thêm dữ liệu vào database!");
        }
    }

}


app.Run();
