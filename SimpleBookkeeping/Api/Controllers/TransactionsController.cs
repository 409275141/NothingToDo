using Microsoft.AspNetCore.Mvc;
using SimpleBookkeeping.Api.Data;
using SimpleBookkeeping.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace SimpleBookkeeping.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransactionsController : ControllerBase
{
    private readonly SqliteDbContext _context;

    public TransactionsController(SqliteDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult GetTransactions([FromQuery] string userId = "demo")
    {
        var transactions = _context.Transactions
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.Date)
            .ToList();
        return Ok(transactions);
    }

    [HttpPost]
    public IActionResult CreateTransaction([FromBody] Transaction transaction)
    {
        if (string.IsNullOrEmpty(transaction.UserId))
        {
            transaction.UserId = "demo";
        }
        
        _context.Transactions.Add(transaction);
        _context.SaveChanges();
        return CreatedAtAction(nameof(GetTransactions), new { id = transaction.Id }, transaction);
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteTransaction(int id, [FromQuery] string userId = "demo")
    {
        var transaction = _context.Transactions.Find(id);
        if (transaction != null && transaction.UserId == userId)
        {
            _context.Transactions.Remove(transaction);
            _context.SaveChanges();
            return NoContent();
        }
        return NotFound();
    }

    [HttpGet("summary")]
    public IActionResult GetSummary([FromQuery] string userId = "demo")
    {
        var transactions = _context.Transactions
            .Where(t => t.UserId == userId)
            .ToList();
        var income = transactions.Where(t => t.Type == "income").Sum(t => t.Amount);
        var expense = transactions.Where(t => t.Type == "expense").Sum(t => t.Amount);
        
        return Ok(new
        {
            TotalIncome = income,
            TotalExpense = expense,
            Balance = income - expense,
            TransactionCount = transactions.Count
        });
    }
}
