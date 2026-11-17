
using MED.Services;
using MED.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MED.Controllers;

[Authorize(Roles = "Employee")]
public class NoticeManagementController(MedStore store) : Controller
{
    private int UserId =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    public IActionResult Details(int id)
    {
        var n = store.Read(d =>
            d.Notices.FirstOrDefault(x => x.Id == id));

        return n == null ? NotFound() : View(n);
    }

    public IActionResult Selected()
    {
        var r = store.Read(d =>
            d.Reviews
                .Where(x => x.EmployeeId == UserId)
                .ToList());

        return View(r);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Add(int noticeId, string reviewTitle)
    {
        if (string.IsNullOrWhiteSpace(reviewTitle))
        {
            return BadRequest("Review title is required.");
        }

        var ok = store.Change(d =>
        {
            if (!d.Notices.Any(n => n.Id == noticeId))
            {
                return false;
            }

            var r = d.Reviews.FirstOrDefault(x =>
                x.EmployeeId == UserId &&
                x.Title == reviewTitle.Trim());

            if (r == null)
            {
                r = new Review
                {
                    Id = MedStore.Next(d.Reviews, x => x.Id),
                    EmployeeId = UserId,
                    Title = reviewTitle.Trim()
                };

                d.Reviews.Add(r);
            }

            if (!r.NoticeIds.Contains(noticeId))
            {
                r.NoticeIds.Add(noticeId);
            }

            r.UpdatedAt = DateTime.UtcNow;

            return true;
        });

        return ok
            ? RedirectToAction("Selected")
            : NotFound();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Remove(int reviewId, int noticeId)
    {
        var ok = store.Change(d =>
        {
            var r = d.Reviews.FirstOrDefault(x =>
                x.Id == reviewId &&
                x.EmployeeId == UserId);

            if (r == null)
            {
                return false;
            }

            r.NoticeIds.Remove(noticeId);

            r.UpdatedAt = DateTime.UtcNow;

            return true;
        });

        return ok
            ? RedirectToAction("Selected")
            : NotFound();
    }
}
