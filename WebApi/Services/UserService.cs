using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using WebApi.Entities;
using WebApi.Helpers;
using WebApi.Models;
using WebApi.Repositories;

namespace WebApi.Services
{
    public interface IUserService
    {
        AuthenticateResponse Authenticate(AuthenticateRequest model);
        IEnumerable<User> GetAll();
        User GetById(int id);
        User CreateNew(User user);
        User Edit(User user);
        User Delete(User user);
        User GetByUsernameAndPassword(string username, string password);
        User UpdatePassword(User user);
    }

    public class UserService : IUserService
    {
        private readonly AppSettings _appSettings;

        private readonly IUsersRepository _usersRepository;

        public UserService(IOptions<AppSettings> appSettings, IUsersRepository usersRepository)
        {
            _appSettings = appSettings.Value;
            _usersRepository = usersRepository;
        }

        public AuthenticateResponse Authenticate(AuthenticateRequest model)
        {
            var user = _usersRepository.GetByUsernameAndPassword(model.Username, model.Password);

            // return null if user not found
            if (user == null)
                return null;

            // authentication successful so generate jwt token
            var token = GenerateJwtToken(user);

            return new AuthenticateResponse(user, token);
        }

        public IEnumerable<User> GetAll()
        {
            return _usersRepository.GetAll();
        }

        public User GetById(int id)
        {
            return _usersRepository.GetById(id);
        }

        public User GetByUsernameAndPassword(string username, string password)
        {
            return _usersRepository.GetByUsernameAndPassword(username, password);
        }

        public User CreateNew(User user)
        {
            return _usersRepository.CreateNew(user);
        }

        public User Edit(User user)
        {
            return _usersRepository.Edit(user);
        }

        public User Delete(User user)
        {
            return _usersRepository.Delete(user);
        }
        public User UpdatePassword(User user)
        {
            return _usersRepository.UpdatePassword(user);
        }

        private string GenerateJwtToken(User user)
        {
            // generate token that is valid for 7 days
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("id", user.Id.ToString()) }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}