using MED.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MED.Controllers;

[Authorize(Roles = "Employee")]
public class NoticesController(MedStore store) : Controller
{
    public IActionResult Index(
        string? keyword,
        string? type)
    {
        var notices = store.Read(d =>
            d.Notices
                .Where(n =>
                    (
                        string.IsNullOrWhiteSpace(keyword)
                        || n.Title.Contains(
                            keyword,
                            StringComparison.OrdinalIgnoreCase
                        )
                        || n.Device.Contains(
                            keyword,
                            StringComparison.OrdinalIgnoreCase
                        )
                        || n.Description.Contains(
                            keyword,
                            StringComparison.OrdinalIgnoreCase
                        )
                    )
                    &&
                    (
                        string.IsNullOrWhiteSpace(type)
                        || n.Type == type
                    )
                )
                .OrderByDescending(n => n.Date)
                .ToList()
        );

        ViewBag.Keyword = keyword;
        ViewBag.Type = type;

        return View(notices);
    }
}