using System.ComponentModel.DataAnnotations;
namespace CaseInvestigationManagementSystem.Models
{
    public class AuditTrailModel
    {
        [Key]
        public int audit_id {get;set;}
        public int case_id {get;set;}
        public int user_id {get;set;}
        public string? action {get;set;}
        public string? old_status {get;set;}
        public string? new_status {get;set;}
        public DateTime created_at {get;set;}
    }
}