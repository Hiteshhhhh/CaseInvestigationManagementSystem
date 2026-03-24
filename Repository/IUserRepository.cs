using CaseInvestigationManagementSystem.Models;

namespace CaseInvestigationManagementSystem.Repositories
{
    public interface IUserRepository
    {
        public void Register(UserModel user);
        public bool Login(UserModel user);
        public bool IsUserExist(string email);
        public bool VerifyPassword(string storedHash, 
                           string providedPassword);
        public List<UserModel> GetInvestigators();
    }
}