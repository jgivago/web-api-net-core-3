using System.Collections.Generic;
using System.Linq;
using WebApi.Data;
using WebApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Repositories
{
    public interface IUsersRepository
    {
        public User CreateNew(User user);
        public IEnumerable<User> GetAll();
        public User GetById(int id);
        public User GetByUsernameAndPassword(string username, string password);
        public User Edit(User user);
        public User Delete(User user);
        public User UpdatePassword(User user);
    }
    
    public class UsersRepository : IUsersRepository
    {
        private readonly DatabaseContext _context;

        public UsersRepository(DatabaseContext dbcontext)
        {
            _context = dbcontext;
        }

        public User CreateNew(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();

            return user;
        }

        public IEnumerable<User> GetAll()
        {
            return _context.Users.AsNoTracking().ToList();
        }

        public User GetById(int id)
        {
            return _context.Users.AsNoTracking().FirstOrDefault(x => x.Id == id);
        }

        public User GetByUsernameAndPassword(string username, string password)
        {
            return _context.Users.AsNoTracking().Where(x => x.Username.Equals(username) && x.Password.Equals(password)).FirstOrDefault();
        }

        public User Edit(User user)
        {
            if (GetById(user.Id) != null) {
                _context.Entry<User>(user).State = EntityState.Modified;
                _context.Entry<User>(user).Property(x => x.Username).IsModified = false;
                _context.Entry<User>(user).Property(x => x.Password).IsModified = false;
                _context.SaveChanges();

                return user;
            }

            return null;
        }

        public User UpdatePassword(User user)
        {
            _context.Entry<User>(user).State = EntityState.Modified;
            _context.SaveChanges();

            return user;
        }

        public User Delete(User user)
        {
            if (GetById(user.Id) != null) {
                _context.Users.Remove(user);
                _context.SaveChanges();

                return user;
            }

            return null;
        }
    }
}