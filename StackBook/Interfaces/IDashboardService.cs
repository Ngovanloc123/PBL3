using StackBook.Models;

namespace StackBook.Interfaces
{
    public interface IDashboardService
    {
        Task<List<double>> GetYearlyRevenueByStatusAsync(int status, int year);
    }
}