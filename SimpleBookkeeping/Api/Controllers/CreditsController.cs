using Microsoft.AspNetCore.Mvc;
using SimpleBookkeeping.Api.Data;
using SimpleBookkeeping.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace SimpleBookkeeping.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CreditsController : ControllerBase
{
    private readonly SqliteDbContext _context;

    public CreditsController(SqliteDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult GetCredits([FromQuery] string userId = "demo")
    {
        var credits = _context.Credits
            .Where(c => c.UserId == userId)
            .OrderByDescending(c => c.Date)
            .ToList();
        return Ok(credits);
    }

    [HttpPost]
    public IActionResult CreateCredit([FromBody] CreditRecord credit)
    {
        if (string.IsNullOrEmpty(credit.UserId))
        {
            credit.UserId = "demo";
        }
        
        if (credit.Status == "unpaid")
        {
            credit.PaidAmount = 0;
        }
        
        _context.Credits.Add(credit);
        _context.SaveChanges();
        return CreatedAtAction(nameof(GetCredits), new { id = credit.Id }, credit);
    }

    [HttpPost("{id}/payment")]
    public IActionResult RecordPayment(int id, [FromQuery] string userId = "demo", [FromBody] PaymentRequest request)
    {
        var credit = _context.Credits.Find(id);
        if (credit != null && credit.UserId == userId)
        {
            credit.PaidAmount += request.Amount;
            if (credit.PaidAmount >= credit.Amount)
            {
                credit.Status = "paid";
            }
            else if (credit.PaidAmount > 0)
            {
                credit.Status = "partial";
            }
            _context.SaveChanges();
            return Ok(credit);
        }
        return NotFound();
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteCredit(int id, [FromQuery] string userId = "demo")
    {
        var credit = _context.Credits.Find(id);
        if (credit != null && credit.UserId == userId)
        {
            _context.Credits.Remove(credit);
            _context.SaveChanges();
            return NoContent();
        }
        return NotFound();
    }

    [HttpGet("summary")]
    public IActionResult GetSummary([FromQuery] string userId = "demo")
    {
        var credits = _context.Credits
            .Where(c => c.UserId == userId)
            .ToList();
        var totalAmount = credits.Sum(c => c.Amount);
        var totalPaid = credits.Sum(c => c.PaidAmount);
        var unpaidCount = credits.Count(c => c.Status == "unpaid");
        
        return Ok(new
        {
            TotalReceivable = totalAmount,
            TotalReceived = totalPaid,
            Outstanding = totalAmount - totalPaid,
            UnpaidCount = unpaidCount,
            TotalRecords = credits.Count
        });
    }
}

public class PaymentRequest
{
    public decimal Amount { get; set; }
}
