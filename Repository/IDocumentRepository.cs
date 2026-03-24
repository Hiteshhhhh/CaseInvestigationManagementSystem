using CaseInvestigationManagementSystem.Models;

namespace CaseInvestigationManagementSystem.Repositories
{
    public interface IDocumentRepository
    {
        public void UploadDocument(DocumentModel document);
        public void DeleteDocument(int document_id);
        public List<DocumentModel> GetDocumentByCaseId(int caseId);        
    }
}