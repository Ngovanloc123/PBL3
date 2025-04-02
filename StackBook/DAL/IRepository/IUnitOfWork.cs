namespace StackBook.DAL.IRepository
{
    public interface IUnitOfWork
    {
        ICategoryRepository Category { get; }
        IAuthorRepository Author { get; }
        IBookDetailRepository BookDetail { get; }
        

        void Save();
    }
}
