using CashFlow.Domain.Entities;
using CashFlow.Domain.Repositories.Expenses;
using Microsoft.EntityFrameworkCore;

namespace CashFlow.Infrastructure.DataAccess.Repositories;

internal class ExpensesRepository : IExpensesWriteOnlyRepository, IExpensesReadOnlyRepository, IExpensesDeleteOnlyRepository, IExpensesUpdateOnlyRepository
{
    private readonly CashFlowDbContext _dbContext;

    public ExpensesRepository(CashFlowDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    #region WriteOnly

    public async Task AddAsync(Expense expense)
    {
        await _dbContext.Expenses.AddAsync(expense);
    }

    #endregion

    #region ReadOnly

    public async Task<List<Expense>> GetAllAsync(User loggedUser)
    {
        return await _dbContext.Expenses.AsNoTracking()
                                        .Where(expense => expense.UserId == loggedUser.Id)
                                        .ToListAsync();
    }

    async Task<Expense?> IExpensesReadOnlyRepository.GetByIdAsync(User loggedUser, long id)
    {
        return await _dbContext.Expenses.AsNoTracking()
                                        .FirstOrDefaultAsync(expense => expense.Id == id && expense.UserId == loggedUser.Id);
    }

    public async Task<List<Expense>> FilterByMonthAsync(User loggedUser, DateOnly date)
    {
        var startDate = new DateTime(year: date.Year, month: date.Month, day: 1).Date;
        var daysInMonth = DateTime.DaysInMonth(date.Year, date.Month);
        var endDate = new DateTime(year: date.Year, month: date.Month, day: daysInMonth, hour: 23, minute: 59, second: 59);

        return await _dbContext.Expenses.AsNoTracking()
                                        .Where(expense => expense.UserId == loggedUser.Id && expense.Date >= startDate && expense.Date <= endDate)
                                        .OrderBy(expense => expense.Date)
                                        .ThenBy(expense => expense.Title)
                                        .ToListAsync();
    }

    #endregion

    #region DeleteOnly

    public async Task DeleteByIdAsync(long id)
    {
        var result = await _dbContext.Expenses.FindAsync(id);

        _dbContext.Expenses.Remove(result!);
    }

    #endregion

    #region UpdateOnly

    async Task<Expense?> IExpensesUpdateOnlyRepository.GetByIdAsync(User loggedUser, long id)
    {
        return await _dbContext.Expenses.FirstOrDefaultAsync(expense => expense.Id == id && expense.UserId == loggedUser.Id);
    }

    public void Update(Expense expense)
    {
        _dbContext.Expenses.Update(expense);
    }

    #endregion
}
