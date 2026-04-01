using Microsoft.AspNetCore.Mvc;
using SimpleBookkeeping.Api.Data;
using SimpleBookkeeping.Api.Models;

namespace SimpleBookkeeping.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CreditsController : ControllerBase
{
    private readonly InMemoryDbContext _context;

    public CreditsController(InMemoryDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult GetCredits([FromQuery] string userId = "demo")
    {
        var credits = _context.GetCredits(userId);
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
        
        var created = _context.AddCredit(credit);
        return CreatedAtAction(nameof(GetCredits), new { id = created.Id }, created);
    }

    [HttpPost("{id}/payment")]
    public IActionResult RecordPayment(int id, [FromQuery] string userId = "demo", [FromBody] PaymentRequest request)
    {
        var updated = _context.UpdateCreditPayment(id, userId, request.Amount);
        if (updated != null)
        {
            return Ok(updated);
        }
        return NotFound();
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteCredit(int id, [FromQuery] string userId = "demo")
    {
        if (_context.DeleteCredit(id, userId))
        {
            return NoContent();
        }
        return NotFound();
    }

    [HttpGet("summary")]
    public IActionResult GetSummary([FromQuery] string userId = "demo")
    {
        var credits = _context.GetCredits(userId);
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
