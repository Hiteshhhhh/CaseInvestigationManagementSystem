

using CaseInvestigationManagementSystem.Models;
using CaseInvestigationManagementSystem.Repositories;
using Microsoft.AspNetCore.Mvc;

public class AdminController : Controller
{
    private readonly ICaseRepository Case;
    private readonly IUserRepository user;
    private readonly IAuditRepository audit;
    
    //Constructor injection for repositories
    public AdminController(ICaseRepository caseRepository, IUserRepository userRepository, IAuditRepository auditRepository)
    {
        Case = caseRepository;
        user = userRepository;
        audit = auditRepository;
    }

    public IActionResult Index()
    {
        if (HttpContext.Session.GetString("role") != "Admin")
        {
            return RedirectToAction("Login", "User");
        }
        var Allcases = Case.GetALLCase(null);
        return View(Allcases);
    }
    public IActionResult CaseDetails(int id)
    {
        if (HttpContext.Session.GetString("role") != "Admin")
            return RedirectToAction("Login", "User");

        Console.WriteLine("CaseDetails called with id: " + id);

        var auditData = audit.GetAuditTrailByCaseId(id);
        Console.WriteLine("Audit Count: " + auditData.Count);

        ViewBag.AuditTrail = auditData;
        var caseDetail = Case.GetCaseById(id);
        ViewBag.Investigators = user.GetInvestigators();
        // ViewBag.AuditTrail = audit.GetAuditTrailByCaseId(id);
        return View(caseDetail);
    }

    [HttpPost]
    public IActionResult AssignInvestigator(int caseId, int investigatorId)
    {
        Case.AssignCase(caseId, investigatorId);

        audit.AddAudit(new AuditTrailModel
        {
            case_id = caseId,
            user_id = HttpContext.Session
                     .GetInt32("user_id") ?? 0,
            action = "Investigator Assigned",
            old_status = null,
            new_status = null
        });
        return RedirectToAction("CaseDetails", new { id = caseId });
    }

    [HttpPost]
    public IActionResult SetPriority(int caseId, string priority)
    {
        Case.SetPriority(caseId, priority);
        audit.AddAudit(new AuditTrailModel
        {
            case_id = caseId,
            user_id = HttpContext.Session.GetInt32("user_id") ?? 0,
            action = "Priority Set to " + priority,
            old_status = null,
            new_status = null
        });
        return RedirectToAction("CaseDetails", new { id = caseId });
    }

    [HttpPost]
    public IActionResult ChangeStatus(int caseId, string status)
    {
        var currentCase = Case.GetCaseById(caseId);
        string oldStatus = currentCase.status ?? "";
        Case.ChangeStatus(caseId, status);
        audit.AddAudit(new AuditTrailModel
        {
            case_id = caseId,
            user_id = HttpContext.Session.GetInt32("user_id") ?? 0,
            action = "Status Changed",
            old_status = oldStatus,
            new_status = status
        });
        return RedirectToAction("CaseDetails", new { id = caseId });
    }

    public IActionResult Dashboard()
    {
        if (HttpContext.Session.GetString("role") != "Admin")
            return RedirectToAction("Login", "User");

        var stats = Case.GetCaseStats();
        var priorityStats = Case.GetCasePriorityWise();
        ViewBag.TotalCases = stats.Values.Sum();
        ViewBag.TotalOpen = stats.GetValueOrDefault("Open", 0);
        ViewBag.TotalInReview = stats.GetValueOrDefault("InReview", 0);
        ViewBag.TotalResolved = stats.GetValueOrDefault("Resolved", 0);
        ViewBag.TotalClosed = stats.GetValueOrDefault("Closed", 0);
        ViewBag.PriorityStats = priorityStats;
        return View();
    }

    [HttpGet]
    public IActionResult GetStatusStats()
    {
        var stats = Case.GetCaseStats();
        return Json(stats);
    }

    [HttpGet]
    public IActionResult GetPriorityStats()
    {
        var stats = Case.GetCasePriorityWise();
        return Json(stats);
    }
}