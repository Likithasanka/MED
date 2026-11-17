
using MED.Models;
using MED.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MED.Controllers;

[Authorize(Roles = "Employee")]
public class SharingController(MedStore store) : Controller
{
    private int UserId =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

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
            d.Users
                .Where(u => u.Role == "Customer")
                .ToList()));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Share(int findingId, int customerId)
    {
        var ok = store.Change(d =>
        {
            if (!d.Findings.Any(f =>
                    f.Id == findingId &&
                    f.EmployeeId == UserId) ||
                !d.Users.Any(u =>
                    u.Id == customerId &&
                    u.Role == "Customer"))
            {
                return false;
            }

            if (!d.Shares.Any(s =>
                s.FindingId == findingId &&
                s.CustomerId == customerId))
            {
                d.Shares.Add(new FindingShare
                {
                    Id = MedStore.Next(d.Shares, s => s.Id),
                    FindingId = findingId,
                    CustomerId = customerId
                });
            }

            return true;
        });

        return ok
            ? RedirectToAction(
                "Details",
                "Findings",
                new { id = findingId })
            : BadRequest();
    }
}
