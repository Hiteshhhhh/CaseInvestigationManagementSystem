
using CaseInvestigationManagementSystem.Models;
using CaseInvestigationManagementSystem.Repositories;
using Microsoft.AspNetCore.Mvc;


public class InvestigatorController : Controller
{
    private readonly ICaseRepository _cases;
    private readonly ICommentRepository _comment;
    private readonly IAuditRepository audit;
    public InvestigatorController(ICaseRepository caseRepository, ICommentRepository commentRepository, IAuditRepository auditRepository)
    {
        _cases = caseRepository;
        _comment = commentRepository;
        audit = auditRepository;
    }

    public IActionResult Index()
    {
        if (HttpContext.Session.GetString("role") != "Investigator")
            return RedirectToAction("Login", "User");

        int investigatorId = HttpContext.Session
                .GetInt32("user_id") ?? 0;
        var assignedCases = _cases.GetAssignedCases(investigatorId);
        return View(assignedCases);
    }

    public IActionResult CaseDetails(int id)
    {
        if (HttpContext.Session.GetString("role") != "Investigator")
            return RedirectToAction("Login", "User");

        var caseDetail = _cases.GetCaseById(id);
        ViewBag.Comment = _comment.GetCommentsByCaseId(id);
        return View(caseDetail);
    }

    [HttpPost]
    public IActionResult UpdateStatus(int caseId, string status)
    {
        var caseModel = _cases.GetCaseById(caseId);
        var oldstatus = caseModel.status;
        _cases.ChangeStatus(caseId, status);
        audit.AddAudit(new AuditTrailModel
        {
            case_id = caseId,
            user_id = HttpContext.Session
                 .GetInt32("user_id") ?? 0,
            action = "Status Changed to" + status,
            old_status = oldstatus,
            new_status = status
        });
        return RedirectToAction("CaseDetails", new { id = caseId });
    }

    [HttpPost]
    public IActionResult AddComment(int caseId, string comment)
    {
        var Comments = new CommentModel
        {
            case_id = caseId,
            user_id = HttpContext.Session.GetInt32("user_id") ?? 0,
            comment = comment
        };
        _comment.AddComment(Comments);
        var caseModel = _cases.GetCaseById(caseId);
        var status = caseModel.status;
        audit.AddAudit(new AuditTrailModel
        {
            case_id = caseId,
            user_id = HttpContext.Session
                 .GetInt32("user_id") ?? 0,
            action = "Comment Added" + comment,
            old_status = status,
            new_status = status
        });
        return RedirectToAction("CaseDetails", new { id = caseId });
    }
}