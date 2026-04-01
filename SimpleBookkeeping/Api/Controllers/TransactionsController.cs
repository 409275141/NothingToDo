using Microsoft.AspNetCore.Mvc;
using SimpleBookkeeping.Api.Data;
using SimpleBookkeeping.Api.Models;

namespace SimpleBookkeeping.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransactionsController : ControllerBase
{
    private readonly InMemoryDbContext _context;

    public TransactionsController(InMemoryDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult GetTransactions([FromQuery] string userId = "demo")
    {
        var transactions = _context.GetTransactions(userId);
        return Ok(transactions);
    }

    [HttpPost]
    public IActionResult CreateTransaction([FromBody] Transaction transaction)
    {
        if (string.IsNullOrEmpty(transaction.UserId))
        {
            transaction.UserId = "demo";
        }
        
        var created = _context.AddTransaction(transaction);
        return CreatedAtAction(nameof(GetTransactions), new { id = created.Id }, created);
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteTransaction(int id, [FromQuery] string userId = "demo")
    {
        if (_context.DeleteTransaction(id, userId))
        {
            return NoContent();
        }
        return NotFound();
    }

    [HttpGet("summary")]
    public IActionResult GetSummary([FromQuery] string userId = "demo")
    {
        var transactions = _context.GetTransactions(userId);
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
