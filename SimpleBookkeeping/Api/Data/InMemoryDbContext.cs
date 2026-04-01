using System.Collections.Concurrent;
using SimpleBookkeeping.Api.Models;

namespace SimpleBookkeeping.Api.Data;

public class InMemoryDbContext
{
    private static readonly ConcurrentDictionary<int, Transaction> _transactions = new();
    private static readonly ConcurrentDictionary<int, CreditRecord> _credits = new();
    private static int _transactionIdCounter = 1;
    private static int _creditIdCounter = 1;

    public List<Transaction> GetTransactions(string userId)
    {
        return _transactions.Values.Where(t => t.UserId == userId).OrderByDescending(t => t.Date).ToList();
    }

    public Transaction AddTransaction(Transaction transaction)
    {
        transaction.Id = Interlocked.Increment(ref _transactionIdCounter);
        _transactions[transaction.Id] = transaction;
        return transaction;
    }

    public bool DeleteTransaction(int id, string userId)
    {
        if (_transactions.TryGetValue(id, out var transaction) && transaction.UserId == userId)
        {
            return _transactions.TryRemove(id, out _);
        }
        return false;
    }

    public List<CreditRecord> GetCredits(string userId)
    {
        return _credits.Values.Where(c => c.UserId == userId).OrderByDescending(c => c.Date).ToList();
    }

    public CreditRecord AddCredit(CreditRecord credit)
    {
        credit.Id = Interlocked.Increment(ref _creditIdCounter);
        _credits[credit.Id] = credit;
        return credit;
    }

    public CreditRecord? UpdateCreditPayment(int id, string userId, decimal paidAmount)
    {
        if (_credits.TryGetValue(id, out var credit) && credit.UserId == userId)
        {
            credit.PaidAmount += paidAmount;
            if (credit.PaidAmount >= credit.Amount)
            {
                credit.Status = "paid";
            }
            else if (credit.PaidAmount > 0)
            {
                credit.Status = "partial";
            }
            _credits[id] = credit;
            return credit;
        }
        return null;
    }

    public bool DeleteCredit(int id, string userId)
    {
        if (_credits.TryGetValue(id, out var credit) && credit.UserId == userId)
        {
            return _credits.TryRemove(id, out _);
        }
        return false;
    }
}
