
using Npgsql;
using CaseInvestigationManagementSystem.Repositories;
using CaseInvestigationManagementSystem.Models;
public class CommentRepository : ICommentRepository
{
    private readonly NpgsqlConnection connection;

    public CommentRepository(IConfiguration configuration)
    {
        connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection"));
    }

    public void AddComment(CommentModel comment)
    {
        try
        {
            connection.Open();
            string query = @"insert into public.t_comments(case_id,user_id,comment) values(@cid,@uid,@c)";
            var cmd = new NpgsqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@cid", comment.case_id);
            cmd.Parameters.AddWithValue("@uid", comment.user_id);
            cmd.Parameters.AddWithValue("@c", comment.comment);
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

    public void DeleteComment(int comment_id)
    {
        try
        {
            connection.Open();
            string query = @"delete from public.t_comments where comment_id = @cid";
            var cmd = new NpgsqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@cid", comment_id);
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
    public List<CommentModel> GetCommentsByCaseId(int caseId)
    {
        List<CommentModel> comments = new List<CommentModel>();
        try
        {
            connection.Open();
            string query = @"select c.comment_id, c.case_id,
                            c.user_id, c.comment, c.created_at,
                            u.username
                            from public.t_comments c
                            join public.t_users u
                            on c.user_id = u.user_id
                            where c.case_id = @cid
                            order by c.created_at asc";
            var cmd = new NpgsqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@cid", caseId);

            var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                comments.Add(new CommentModel()
                {
                    comment_id = reader.GetInt32(0),
                    case_id = reader.GetInt32(1),
                    user_id = reader.GetInt32(2),
                    comment = reader.GetString(3),
                    created_at = reader.GetDateTime(4)
                });
            }
        }
        catch(Exception ex)
        {
            Console.Write(ex.Message);
        }
        finally
        {
            connection.Close();
        }
        return comments;
    }
}