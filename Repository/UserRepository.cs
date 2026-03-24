using Npgsql;
using CaseInvestigationManagementSystem.Models;
using CaseInvestigationManagementSystem.Repositories;
using Microsoft.AspNetCore.Identity;

public class UserRepository : IUserRepository
{
    private readonly NpgsqlConnection connection;
    private readonly IHttpContextAccessor _accessor;
    public UserRepository(IConfiguration config,
                          IHttpContextAccessor accessor)
    {
        connection = new NpgsqlConnection(config.GetConnectionString("DefaultConnection"));
        _accessor = accessor;
    }

    public void Register(UserModel user)
    {
        try
        {
            var hasher = new PasswordHasher<UserModel>();
            user.password_hash = hasher.HashPassword(user, user.password_hash);
            connection.Open();
            string query = @"insert into public.t_users(username,password_hash,email,role,is_active) values(@un,@pass,@email,@role,@isa)";
            var cmd = new NpgsqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@un", user.username);
            cmd.Parameters.AddWithValue("@pass", user.password_hash);
            cmd.Parameters.AddWithValue("@email", user.email);
            cmd.Parameters.AddWithValue("@role", "User");
            cmd.Parameters.AddWithValue("@isa", true);
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
    public bool Login(UserModel user)
    {
        try
        {
            connection.Open();
            string query = @"select * from public.t_users where email = @email";
            var cmd = new NpgsqlCommand(query,connection);
            cmd.Parameters.AddWithValue("@email",user.email);
            var row = cmd.ExecuteReader();

            if (row.Read())
            {
                if(VerifyPassword(row["password_hash"].ToString(),user.password_hash))
                {
                    string username = row["username"].ToString();
                    string role = row["role"].ToString();
                    _accessor.HttpContext.Session.SetInt32("user_id",row.GetInt32(0));
                    _accessor.HttpContext.Session.SetString("username",username);
                    _accessor.HttpContext.Session.SetString("role",role);
                    _accessor.HttpContext.Session.SetString("email",user.email);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

        }
        catch(Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        finally
        {
            connection.Close();
        }
        return false;
    }
    public bool IsUserExist(string email)
    {
        try
        {
            connection.Open();
            string query = @"select * from public.t_users where email = @email";
            var cmd = new NpgsqlCommand(query,connection);
            cmd.Parameters.AddWithValue("@email",email);
            var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        catch(Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        finally
        {
            connection.Close();
        }
        return false;
    }
    public bool VerifyPassword(string storedHash,
                       string providedPassword)
    {
         var Pass_hasher = new PasswordHasher<UserModel>();
         var result = Pass_hasher.VerifyHashedPassword(null,storedHash,providedPassword);
         return result == PasswordVerificationResult.Success;
    }

    public List<UserModel> GetInvestigators()
    {
        List<UserModel> users = new List<UserModel>();
        try
        {
            connection.Open();
            string query = @"select user_id, username from public.t_users where role = 'Investigator'";
            var cmd = new NpgsqlCommand(query,connection);
            var reader = cmd.ExecuteReader();

            while(reader.Read())
            {
                users.Add(new UserModel
                {
                   user_id = reader.GetInt32(0),
                   username = reader.GetString(1) 
                });
            }

        }
        catch(Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        return users;
    }
}
