using MED.Models;
using MED.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MED.Controllers;

[Authorize(Roles = "Employee")]
public class CommentsController(MedStore store) : Controller
{
    private int UserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    public IActionResult Index(int noticeId)
    {
        var notice = store.Read(d => d.Notices.FirstOrDefault(n => n.Id == noticeId));

        if (notice == null)
            return NotFound();

        ViewBag.Notice = notice;

        return View(store.Read(d => d.NoticeComments.Where(c => c.NoticeId == noticeId && c.EmployeeId == UserId).OrderByDescending(c => c.CreatedAt).ToList()));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Add(int noticeId, string text)
    {
        if (string.IsNullOrWhiteSpace(text) || text.Length > 1000)
            return BadRequest();

        var ok = store.Change(d =>
        {
            if (!d.Reviews.Any(r => r.EmployeeId == UserId && r.NoticeIds.Contains(noticeId)))
                return false;

            d.NoticeComments.Add(new NoticeComment
            {
                Id = MedStore.Next(d.NoticeComments, c => c.Id),
                EmployeeId = UserId,
                NoticeId = noticeId,
                Text = text.Trim()
            });

            return true;
        });

        return ok ? RedirectToAction("Index", new { noticeId }) : Forbid();
    }
}