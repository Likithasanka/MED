
using MED.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MED.Controllers;

[Authorize(Roles = "Customer")]
public class CustomerController(MedStore store) : Controller
{
    private int UserId =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    public IActionResult Index()
    {
        var shares = store.Read(d =>
            d.Shares
                .Where(s => s.CustomerId == UserId)
                .ToList());

        ViewBag.Findings = store.Read(d =>
            d.Findings.ToList());

        return View(shares);
    }

    public IActionResult Details(int id)
    {
        var share = store.Read(d =>
            d.Shares.FirstOrDefault(s =>
                s.Id == id &&
                s.CustomerId == UserId));

        if (share == null)
        {
            return NotFound();
        }

        ViewBag.Finding = store.Read(d =>
            d.Findings.FirstOrDefault(f =>
                f.Id == share.FindingId));

        return View(share);
    }
}
