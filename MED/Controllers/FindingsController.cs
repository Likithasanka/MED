using MED.Models;
using MED.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MED.Controllers;

[Authorize(Roles = "Employee")]
public class FindingsController(MedStore store) : Controller
{
    private int UserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    public IActionResult Index() => View(store.Read(d => d.Findings.Where(f => f.EmployeeId == UserId).OrderByDescending(f => f.UpdatedAt).ToList()));

    public IActionResult Create(int reviewId)
    {
        var r = store.Read(d => d.Reviews.FirstOrDefault(x => x.Id == reviewId && x.EmployeeId == UserId));

        if (r == null)
            return NotFound();

        ViewBag.Review = r;

        return View();
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Create(int reviewId, string title, string summary)
    {
        if (string.IsNullOrWhiteSpace(title) || title.Length > 120 || summary?.Length > 3000)
            return BadRequest();

        var ok = store.Change(d =>
        {
            if (!d.Reviews.Any(r => r.Id == reviewId && r.EmployeeId == UserId))
                return false;

            d.Findings.Add(new Finding
            {
                Id = MedStore.Next(d.Findings, f => f.Id),
                EmployeeId = UserId,
                ReviewId = reviewId,
                Title = title.Trim(),
                Summary = summary?.Trim() ?? ""
            });

            return true;
        });

        return ok ? RedirectToAction("Index") : Forbid();
    }

    public IActionResult Details(int id)
    {
        var f = store.Read(d => d.Findings.FirstOrDefault(x => x.Id == id && x.EmployeeId == UserId));

        if (f == null)
            return NotFound();

        return View(f);
    }
}