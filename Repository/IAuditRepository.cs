using CaseInvestigationManagementSystem.Models;

namespace CaseInvestigationManagementSystem.Repositories
{
    public interface IAuditRepository
    {
        public void AddAudit(AuditTrailModel audit);
        public List<AuditTrailModel> GetAuditTrailByCaseId(int caseId);
    }
}