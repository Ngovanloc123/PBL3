namespace StackBook.Interfaces
{
    public interface IBookService
    {
        Task UpdateBookQuantity(Guid bookId, int quantity, string status);
    }
}