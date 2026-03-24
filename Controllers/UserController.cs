
using Microsoft.AspNetCore.Mvc;
using CaseInvestigationManagementSystem.Repositories;
using CaseInvestigationManagementSystem.Models;
using Microsoft.AspNetCore.SignalR;
using CaseInvestigationManagementSystem.Hubs;
public class UserController : Controller
{
    private readonly IUserRepository user;
    private readonly ICaseRepository _cases;
    private readonly IDocumentRepository documents;
    private readonly IWebHostEnvironment _environment;
    private readonly ICommentRepository _comment;
    private readonly IAuditRepository audit;
    private readonly IHubContext<NotificationHub> _hub;
    public UserController(IUserRepository userRepository, ICaseRepository caseRepository, IDocumentRepository documentRepository, IWebHostEnvironment web, IAuditRepository auditRepository, ICommentRepository commentRepository, IHubContext<NotificationHub> hub)
    {
        user = userRepository;
        _cases = caseRepository;
        documents = documentRepository;
        _environment = web;
        audit = auditRepository;
        _comment = commentRepository;
        _hub = hub;
    }
    public IActionResult Index()
    {
        if (HttpContext.Session.GetString("role") == null)
            return RedirectToAction("Login");

        int userId = HttpContext.Session.GetInt32("user_id") ?? 0;
        var cases = _cases.GetALLCase(userId);
        return View(cases);
    }

    public IActionResult CaseDetails(int id)
    {
        if (HttpContext.Session.GetString("role") == null)
            return RedirectToAction("Login");
        var caseDetail = _cases.GetCaseById(id);
        ViewBag.Comments = _comment
                      .GetCommentsByCaseId(id);
        return View(caseDetail);
    }

    [HttpGet]
    public IActionResult AddCase()
    {
        if (HttpContext.Session.GetString("role") == null)
            return RedirectToAction("Login");
        return View();
    }

    // [HttpPost]
    // public IActionResult AddCase(CaseModel model)
    // {
    //     if (HttpContext.Session.GetString("role") == null)
    //         return RedirectToAction("Login");

    //     model.created_by = HttpContext.Session
    //                .GetInt32("user_id") ?? 0;
    //     model.status = "Open";
    //     model.assigned_to = null;
    //     model.priority = "Medium";
    //     Case.AddCase(model);
    //     audit.AddAudit(new AuditTrailModel
    //     {
    //         case_id = model.case_id,
    //         user_id = HttpContext.Session
    //             .GetInt32("user_id") ?? 0,
    //         action = "Case Submitted",
    //         old_status = null,
    //         new_status = "Open"
    //     });

    //     return RedirectToAction("Index");
    // }
    [HttpPost]
    public async Task<IActionResult> AddCase(
        CaseModel model)
    {
        model.created_by = HttpContext.Session
                           .GetInt32("user_id") ?? 0;
        model.status = "Open";
        model.priority = "Medium";

        _cases.AddCase(model);
        audit.AddAudit(new AuditTrailModel
        {
            case_id = model.case_id,
            user_id = HttpContext.Session
              .GetInt32("user_id") ?? 0,
            action = "Case Submitted",
            old_status = null,
            new_status = "Open"
        });
        // SignalR — Admin ko notify karo!
        await _hub.Clients.Group("Admin").SendAsync("NewCaseAlert", new
        {
            title = model.title,
            submittedBy = HttpContext.Session
                                   .GetString("username"),
            time = DateTime.Now
                             .ToString("dd-MM-yyyy HH:mm")
        });

        return RedirectToAction("Index");
    }
    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Register(UserModel users)
    {
        if (string.IsNullOrEmpty(users.email) || user.IsUserExist(users.email))
        {
            ViewBag.msg = "*User already exist please use different email address";
            return View();
        }
        else
        {
            TempData["smsg"] = "Your are successfully registered login to continue";
            user.Register(users);
            return RedirectToAction("Login");
        }
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Login(UserModel users)
    {
        if (user.Login(users))
        {
            if (HttpContext.Session.GetString("role") == "User")
            {
                return RedirectToAction("Index", "User");
            }
            else if (HttpContext.Session.GetString("role") == "Admin")
            {
                return RedirectToAction("Index", "Admin");
            }
            else
            {
                return RedirectToAction("Index", "Investigator");
            }
        }
        TempData["msg"] = "Username or password is wrong";
        return View();
    }
    [HttpPost]
    public IActionResult UploadDocument(
     DocumentModel document, int caseId)
    {
        if (document.file != null &&
           document.file.Length > 0)
        {
            // 1. Uploads folder check karo
            string uploadsFolder = Path.Combine(
                _environment.WebRootPath, "uploads");

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            // 2. Unique file naam banao
            string file_name = Guid.NewGuid() +
                               document.file.FileName;

            // 3. File save karo
            string path = Path.Combine(
                uploadsFolder, file_name);

            using (var stream = new FileStream(
                path, FileMode.Create))
            {
                document.file.CopyTo(stream);
            }

            document.file_name = file_name;
            document.file_path = "uploads/" + file_name;
            document.case_id = caseId;
            document.uploaded_by = HttpContext.Session
                                   .GetInt32("user_id") ?? 0;

            documents.UploadDocument(document);
            audit.AddAudit(new AuditTrailModel
            {
                case_id = caseId,
                user_id = HttpContext.Session
                     .GetInt32("user_id") ?? 0,
                action = "Document Uploaded: " + file_name,
                old_status = null,
                new_status = null
            });
        }
        return RedirectToAction("CaseDetails",
                                new { id = caseId });
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Remove("username");
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }
}