using CaseInvestigationManagementSystem.Models;

namespace CaseInvestigationManagementSystem.Repositories
{
    public interface ICaseRepository
    {
        public void AddCase(CaseModel Case);
        public void UpdateCase(CaseModel Case);
        public void DeleteCase(int id);
        public List<CaseModel> GetALLCase(int? userId = null);
        public CaseModel GetCaseById(int id);
        public void AssignCase(int caseid, int investigatorId);
        public void SetPriority(int caseId, string priority);
        public void ChangeStatus(int caseId, string status);
        public List<CaseModel> GetAssignedCases(int investigatorId);
        public Dictionary<string, int> GetCaseStats();
        public Dictionary<string, int> GetCasePriorityWise();
    }
}