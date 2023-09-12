using DotnetAPI.Models;

namespace DotnetAPI.Data
{
    public class UserRepository : IUserRepository


    {
        private DataContextEF _entityFramework;
        public UserRepository(IConfiguration config)
        {
            _entityFramework = new DataContextEF(config);
        }

        public bool SaveChanges()
        {
            return _entityFramework.SaveChanges() > 0;
        }

        public IEnumerable<User> GetUsers()
        {
            return _entityFramework.Users.ToList<User>();
        }

        public User GetUser(int userId)
        {
            User? user = _entityFramework.Users
                .Where(u => u.UserId == userId)
                .FirstOrDefault<User>();
            if (user != null)
            {
                return user;
            }
            throw new Exception("Failed to Get User");
        }

        // public void AddEntity<T>(T entityToAdd){
        //     if(entityToAdd != null){
        //         _entityFramework.Add(entityToAdd);
        //     }
        // }

        public void AddEntity<T>(T entityToAdd)
        {
            if (entityToAdd != null)
            {
                _entityFramework.Add(entityToAdd);
            }
            return;
        }

        public void RemoveEntity<T>(T entityToAdd)
        {
            if (entityToAdd != null)
            {
                _entityFramework.Remove(entityToAdd);
            }
        }
    }
}