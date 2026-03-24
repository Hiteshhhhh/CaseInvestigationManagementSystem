namespace CaseInvestigationManagementSystem.Models
{
    public class DocumentModel
    {
        public int doc_id { get; set; }
        public int case_id { get; set; }
        public string? file_name { get; set; }
        public string? file_path { get; set; }
        public IFormFile? file { get; set; }
        public int uploaded_by { get; set; }
        public DateTime uploaded_at { get; set; }
    }
}