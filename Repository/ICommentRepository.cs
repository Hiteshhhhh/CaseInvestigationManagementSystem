using CaseInvestigationManagementSystem.Models;

namespace CaseInvestigationManagementSystem.Repositories
{
    public interface ICommentRepository
    {
        public void AddComment(CommentModel comment);
        public void DeleteComment(int comment_id);
        public List<CommentModel> GetCommentsByCaseId(int caseId);
    }
}