using CaseInvestigationManagementSystem.Models;
using Npgsql;
using CaseInvestigationManagementSystem.Repositories;

public class DocumentRepository : IDocumentRepository
{
    private readonly NpgsqlConnection connection;
    public DocumentRepository(IConfiguration configuration)
    {
        connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection"));
    }

    public void UploadDocument(DocumentModel document)
    {
        try
        {
            connection.Open();
            string query = @"insert into public.t_documents
            (case_id, file_name, file_path, uploaded_by)
            values(@cid, @fn, @fp, @ub)";
            var cmd = new NpgsqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@cid", document.case_id);
            cmd.Parameters.AddWithValue("@fn", document.file_name);
            cmd.Parameters.AddWithValue("@fp", document.file_path);
            cmd.Parameters.AddWithValue("ub", document.uploaded_by);
            cmd.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    public void DeleteDocument(int document_id)
    {
        try
        {
            connection.Open();
            string query = @"delete from public.t_documents where doc_id = @docid";
            var cmd = new NpgsqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@docid", document_id);
            cmd.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
    public List<DocumentModel> GetDocumentByCaseId(int caseId)
    {
        List<DocumentModel> documents = new List<DocumentModel>();
        try
        {
            connection.Open();
            string query = @"select * from public.t_documents 
                         where case_id = @caseid";
            var cmd = new NpgsqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@caseid", caseId);
            var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                documents.Add(new DocumentModel
                {
                    doc_id = reader.GetInt32(0),
                    case_id = reader.GetInt32(1),
                    file_name = reader.GetString(2),
                    file_path = reader.GetString(3),
                    uploaded_by = reader.GetInt32(4)
                });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        finally
        {
            connection.Close();
        }
        return documents;
    }
}
