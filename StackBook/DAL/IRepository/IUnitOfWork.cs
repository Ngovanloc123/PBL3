namespace StackBook.DAL.IRepository
{
    public interface IUnitOfWork
    {
        ICategoryRepository Category { get; }
        IAuthorRepository Author { get; }
        IBookRepository Book { get; }
        IUserRepository User { get; }


        Task SaveAsync();
    }
}
