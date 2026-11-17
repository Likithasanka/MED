
using MED.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MED.Controllers;

[Authorize(Roles = "Employee")]
public class ManagementController(MedStore store) : Controller
{
    private int UserId =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    public IActionResult Edit(int id)
    {
        var f = store.Read(d =>
            d.Findings.FirstOrDefault(f =>
                f.Id == id &&
                f.EmployeeId == UserId));

        return f == null
            ? NotFound()
            : View(f);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, string title, string summary)
    {
        if (string.IsNullOrWhiteSpace(title) ||
            title.Length > 120 ||
            summary?.Length > 3000)
        {
            return BadRequest();
        }

        var ok = store.Change(d =>
        {
            var f = d.Findings.FirstOrDefault(f =>
                f.Id == id &&
                f.EmployeeId == UserId);

            if (f == null)
            {
                return false;
            }

            f.Title = title.Trim();
            f.Summary = summary?.Trim() ?? "";
            f.UpdatedAt = DateTime.UtcNow;

            return true;
        });

        return ok
            ? RedirectToAction(
                "Details",
                "Findings",
                new { id }
              )
            : NotFound();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Delete(int id)
    {
        var ok = store.Change(d =>
        {
            var f = d.Findings.FirstOrDefault(f =>
                f.Id == id &&
                f.EmployeeId == UserId);

            if (f == null)
            {
                return false;
            }

            var shareIds = d.Shares
                .Where(s => s.FindingId == id)
                .Select(s => s.Id)
                .ToHashSet();

            d.CustomerComments.RemoveAll(c =>
                shareIds.Contains(c.ShareId));

            d.Shares.RemoveAll(s =>
                s.FindingId == id);

            d.Findings.Remove(f);

            return true;
        });

        return ok
            ? RedirectToAction("Index", "Findings")
            : NotFound();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult EditComment(int commentId, string text)
    {
        if (string.IsNullOrWhiteSpace(text) ||
            text.Length > 1000)
        {
            return BadRequest();
        }

        var id = store.Change(d =>
        {
            var c = d.NoticeComments.FirstOrDefault(c =>
                c.Id == commentId &&
                c.EmployeeId == UserId);

            if (c == null)
            {
                return 0;
            }

            c.Text = text.Trim();

            return c.NoticeId;
        });

        return id > 0
            ? RedirectToAction(
                "Index",
                "Comments",
                new { noticeId = id }
              )
            : NotFound();
    }
}
