
using MED.Models;
using MED.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MED.Controllers;

[Authorize]
public class FeedbackController(MedStore store) : Controller
{
    private int UserId =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [Authorize(Roles = "Customer")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Add(int shareId, string text)
    {
        if (string.IsNullOrWhiteSpace(text) || text.Length > 1000)
        {
            return BadRequest();
        }

        var ok = store.Change(d =>
        {
            if (!d.Shares.Any(s =>
                s.Id == shareId && s.CustomerId == UserId))
            {
                return false;
            }

            d.CustomerComments.Add(new CustomerComment
            {
                Id = MedStore.Next(
                    d.CustomerComments,
                    c => c.Id
                ),
                ShareId = shareId,
                CustomerId = UserId,
                Text = text.Trim()
            });

            return true;
        });

        return ok
            ? RedirectToAction(
                "Details",
                "Customer",
                new { id = shareId }
              )
            : Forbid();
    }

    [Authorize(Roles = "Employee")]
    public IActionResult Index(int findingId)
    {
        if (!store.Read(d =>
            d.Findings.Any(f =>
                f.Id == findingId &&
                f.EmployeeId == UserId)))
        {
            return NotFound();
        }

        ViewBag.FindingId = findingId;

        return View(store.Read(d =>
            d.CustomerComments
                .Where(c => d.Shares.Any(s =>
                    s.Id == c.ShareId &&
                    s.FindingId == findingId))
                .OrderByDescending(c => c.CreatedAt)
                .ToList()
        ));
    }
}
