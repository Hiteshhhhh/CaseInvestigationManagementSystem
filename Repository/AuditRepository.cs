using Npgsql;
using CaseInvestigationManagementSystem.Repositories;
using CaseInvestigationManagementSystem.Models;
public class AuditRepository : IAuditRepository
{
    private readonly NpgsqlConnection _conn;

    public AuditRepository(IConfiguration configuration)
    {
        _conn = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection"));
    }

    public void AddAudit(AuditTrailModel audit)
    {
        try
        {
            _conn.Open();
            string query = @"insert into public.t_audit_trail(case_id,user_id,action,old_status,new_status) values(@cid,@uid,@a,@o,@ns)";
            var cmd = new NpgsqlCommand(query, _conn);
            cmd.Parameters.AddWithValue("@cid", audit.case_id);
            cmd.Parameters.AddWithValue("@uid", audit.user_id);
            cmd.Parameters.AddWithValue("@a", audit.action ?? "");
            cmd.Parameters.AddWithValue("@o",
                (object?)audit.old_status ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ns",
                (object?)audit.new_status ?? DBNull.Value);
            cmd.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        finally
        {
            _conn.Close();
        }
    }
    public List<AuditTrailModel> GetAuditTrailByCaseId(int caseId)
    {
        List<AuditTrailModel> auditTrails = new List<AuditTrailModel>();
        try
        {
            _conn.Open();
            string query = "select * from public.t_audit_trail where case_id = @cid";
            var cmd = new NpgsqlCommand(query, _conn);
            cmd.Parameters.AddWithValue("@cid", caseId);
            var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                auditTrails.Add(new AuditTrailModel()
                {
                    audit_id   = reader.GetInt32(0),
                        case_id    = reader.GetInt32(1),
                        user_id    = reader.GetInt32(2),
                        action     = reader.GetString(3),
                        old_status = reader.IsDBNull(4) ? null : reader.GetString(4),
                        new_status = reader.IsDBNull(5)? null : reader.GetString(5),
                        created_at = reader.GetDateTime(6)
                });
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        finally
        {
            _conn.Close();
        }
        return auditTrails;
    }
}