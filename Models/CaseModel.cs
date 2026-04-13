using System.ComponentModel.DataAnnotations;
namespace CaseInvestigationManagementSystem.Models
{
    public class CaseModel
    {
        [Key]
        public int case_id { get; set; }
        public string? title { get; set; }
        public string? description { get; set; }
        public string? priority { get; set; }
        public string? status { get; set; }
        public int created_by { get; set; }
        public int? assigned_to { get; set; }
        public DateTime created_at { get; set; }
        public DateTime? deadline { get; set; }
        public string? investigator_name { get; set; }
        public string? created_at_formatted { get; set; }
    }
}