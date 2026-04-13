using System.ComponentModel.DataAnnotations;
namespace CaseInvestigationManagementSystem.Models
{
    public class CommentModel
    {
        [Key]
        public int comment_id {get;set;}
        public int case_id {get;set;}
        public int user_id {get;set;}
        public string? comment{get;set;}
        public DateTime created_at {get;set;}
        public string? username { get; set; }
    }
}