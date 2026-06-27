using ExamenII_PrograWeb.DTOs;
using ExamenII_PrograWeb.Models;

namespace ExamenII_PrograWeb.Services;

public class ExpenseService
{
    private readonly FirebaseService _firebaseService;

    public ExpenseService(FirebaseService firestoreService)
    {
        _firebaseService = firestoreService;
    }

    public async Task<Expense> CreateExpense(ExpenseCreateDto dto, string userId)
    {
        var expense = new Expense
        {
            Id = Guid.NewGuid().ToString(),
            Description = dto.Description,
            Amount = dto.Amount,
            Category = dto.Category,
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };

        var collection = _firebaseService.GetCollection("expenses");

        await collection.Document(expense.Id).SetAsync(new Dictionary<string, object>
        {
            { "Id", expense.Id },
            { "Description", expense.Description },
            { "Amount", expense.Amount },
            { "Category", expense.Category },
            { "UserId", expense.UserId },
            { "CreatedAt", expense.CreatedAt }
        });

        return expense;
    }

    public async Task<List<Expense>> GetExpensesByUser(string userId)
    {
        var collection = _firebaseService.GetCollection("expenses");

        var snapshot = await collection
            .WhereEqualTo("UserId", userId)
            .GetSnapshotAsync();

        var expenses = new List<Expense>();

        foreach (var doc in snapshot.Documents)
        {
            var data = doc.ToDictionary();

            var expense = new Expense
            {
                Id = data["Id"].ToString()!,
                Description = data["Description"].ToString()!,
                Amount = Convert.ToDecimal(data["Amount"]),
                Category = data["Category"].ToString()!,
                UserId = data["UserId"].ToString()!,
                CreatedAt = ((Google.Cloud.Firestore.Timestamp)data["CreatedAt"]).ToDateTime()
            };

            expenses.Add(expense);
        }

        return expenses;
    }
}