
using System.ComponentModel.DataAnnotations;
using CaseInvestigationManagementSystem.Repositories;
using Npgsql;
using CaseInvestigationManagementSystem.Models;

public class CaseRepository : ICaseRepository
{
    private readonly NpgsqlConnection connection;
    public CaseRepository(IConfiguration configuration)
    {
        connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection"));
    }
    
    public void AddCase(CaseModel Case)
    {
        try
        {
            connection.Open();
            string query = @"insert into public.t_cases (title,description,priority,status,created_by,assigned_to) values(@t,@desc,@p,@s,@c_by,@ass_to) RETURNING case_id";
            var cmd = new NpgsqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@t", Case.title);
            cmd.Parameters.AddWithValue("@desc", Case.description);
            cmd.Parameters.AddWithValue("@p", Case.priority);
            cmd.Parameters.AddWithValue("@s", Case.status);
            cmd.Parameters.AddWithValue("@c_by", Case.created_by);
            cmd.Parameters.AddWithValue("@ass_to", (object?)Case.assigned_to ?? DBNull.Value);
            int newCaseId = Convert.ToInt32(
                cmd.ExecuteScalar());
            Case.case_id = newCaseId;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        finally
        {
            connection.Close();
        }
    }
    public void UpdateCase(CaseModel Case)
    {
        try
        {
            connection.Open();
            string query = @"update public.t_cases set title=@t,
                                description=@desc where case_id = @id";
            var cmd = new NpgsqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@t", Case.title);
            cmd.Parameters.AddWithValue("@desc", Case.description);
            cmd.Parameters.AddWithValue("@id", Case.case_id);
            cmd.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        finally
        {
            connection.Close();
        }
    }
    public void DeleteCase(int id)
    {
        try
        {
            connection.Open();
            string query = @"delete from public.t_cases where case_id = @id";
            var cmd = new NpgsqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        finally
        {
            connection.Close();
        }
    }
    public List<CaseModel> GetALLCase(int? userId = null)
    {
        List<CaseModel> List_cases = new List<CaseModel>();
        try
        {
            connection.Open();
            string query;
            if (userId == null)
            {
                query = @"select case_id, title, description,
                      priority, status,deadline 
                      from public.t_cases";
            }
            else
            {
                query = @"select case_id, title, description,
                      priority, status,deadline 
                      from public.t_cases
                      where created_by = @uid";
            }
            var cmd = new NpgsqlCommand(query, connection);
            if (userId != null)
                cmd.Parameters.AddWithValue("@uid", userId);
            var reader = cmd.ExecuteReader();


            while (reader.Read())
            {
                List_cases.Add(new CaseModel
                {
                    case_id = reader.GetInt32(0),
                    title = reader.GetString(1),
                    description = reader.GetString(2),
                    priority = reader.GetString(3),
                    status = reader.GetString(4),
                    deadline = reader.IsDBNull(5) ?
           null : reader.GetDateTime(5)
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
        return List_cases;
    }
    public CaseModel GetCaseById(int id)
    {
        CaseModel c = new CaseModel();
        try
        {
            connection.Open();
            string query = @"select c.case_id, c.title, c.description,
            c.priority, c.status, c.created_at,
            c.deadline,
            u.username as investigator_name
            from public.t_cases c
            left join public.t_users u 
            on c.assigned_to = u.user_id
            where c.case_id = @id";
            var cmd = new NpgsqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@id", id);
            var reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                c = new CaseModel
                {
                    case_id = reader.GetInt32(0),
                    title = reader.GetString(1),
                    description = reader.GetString(2),
                    priority = reader.GetString(3),
                    status = reader.GetString(4),
                    created_at = reader.GetDateTime(5),
                    deadline = reader.IsDBNull(6) ? null : reader.GetDateTime(6),
                    investigator_name = reader.IsDBNull(7) ? null : reader.GetString(7)
                };
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
        return c;
    }
    public void AssignCase(int caseid, int investigatorId)
    {
        try
        {
            connection.Open();
            string query = @"update public.t_cases set assigned_to = @inv
            where case_id = @id";
            var cmd = new NpgsqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@id", caseid);
            cmd.Parameters.AddWithValue("@inv", investigatorId);
            cmd.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        finally
        {
            connection.Close();
        }
    }

    public Dictionary<string, int> GetCaseStats()
    {
        var stats = new Dictionary<string, int>
        {
          {"Open",0},
          {"InReview",0},
          {"Resolved",0},
          {"Closed",0}
        };
        try
        {
            connection.Open();
            string query = @"select status, count(*) 
                            from public.t_cases
                            group by status";
            var cmd = new NpgsqlCommand(query, connection);
            var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                string status = reader.GetString(0);
                int count = reader.GetInt32(1);
                if (stats.ContainsKey(status))
                {
                    stats[status] = count;
                }
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
        return stats;
    }

    public Dictionary<string, int> GetCasePriorityWise()
    {
        var Priorities = new Dictionary<string, int>
        {
            {"Low",0},
            {"Medium",0},
            {"High",0},
            {"Critical",0}
        };
        try
        {
            connection.Open();
            string query = @"select priority, count(*) 
                        from public.t_cases
                        group by priority";
            var cmd = new NpgsqlCommand(query, connection);
            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                string priority = reader.GetString(0);
                int count = reader.GetInt32(1);
                if (Priorities.ContainsKey(priority))
                {
                    Priorities[priority] = count;
                }
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
        return Priorities;
    }
    public List<CaseModel> GetAssignedCases(int investigatorId)
    {
        List<CaseModel> cases = new List<CaseModel>();
        try
        {
            connection.Open();
            string query = @"select case_id, title, 
                 description, priority, status,deadline
                 from public.t_cases 
                 where assigned_to = @inv";
            var cmd = new NpgsqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@inv", investigatorId);
            var rows = cmd.ExecuteReader();
            while (rows.Read())
            {
                cases.Add(new CaseModel
                {
                    case_id = rows.GetInt32(0),
                    title = rows.GetString(1),
                    description = rows.GetString(2),
                    priority = rows.GetString(3),
                    status = rows.GetString(4),
                    deadline = rows.IsDBNull(5) ?
           null : rows.GetDateTime(5)
                });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        return cases;
    }

    public void SetPriority(int caseId, string priority)
    {
        try
        {
            connection.Open();
            string query = @"update public.t_cases set priority = @p, deadline = @d  where case_id = @c_id";
            var cmd = new NpgsqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@p", priority);
            cmd.Parameters.AddWithValue("@c_id", caseId);

            DateTime deadline = priority switch
            {
                "Critical" => DateTime.Now.AddDays(1),
                "High" => DateTime.Now.AddDays(3),
                "Medium" => DateTime.Now.AddDays(7),
                "Low" => DateTime.Now.AddDays(14),
                _ => DateTime.Now.AddDays(7)
            };
            cmd.Parameters.AddWithValue("@d", deadline);
            cmd.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        finally
        {
            connection.Close();
        }
    }
    public void ChangeStatus(int caseId, string status)
    {
        try
        {
            connection.Open();
            string query = @"update public.t_cases set status = @s where case_id = @c_id";
            var cmd = new NpgsqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@s", status);
            cmd.Parameters.AddWithValue("@c_id", caseId);
            cmd.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        finally
        {
            connection.Close();
        }
    }
}