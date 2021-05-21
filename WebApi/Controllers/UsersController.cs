using Microsoft.AspNetCore.Mvc;
using WebApi.Models;
using WebApi.Services;
using WebApi.Entities;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("authenticate")]
        public IActionResult Authenticate(AuthenticateRequest model)
        {
            var response = _userService.Authenticate(model);

            if (response == null)
                return BadRequest(new { message = "Usuário ou senha incorretos" });

            return Ok(response);
        }

        [Authorize]
        public IActionResult GetAll()
        {
            var users = _userService.GetAll();
            return Ok(users);
        }

        [Authorize]
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var user = _userService.GetById(id);
            if (user == null)
                return NotFound();

            return Ok(user);
        }

        [Route("new")]
        [HttpPost]
        public ActionResult<User> CreateNew([FromBody]User user)
        {
            return _userService.CreateNew(user);
        }

        [Authorize]
        [Route("edit")]
        [HttpPut]
        public ActionResult<User> Edit([FromBody]User user)
        {
            var editedUser = _userService.Edit(user);
            if (editedUser == null)
                return NotFound();

            return editedUser;
        }

        [Route("updatepassword")]
        [HttpPut]
        public IActionResult UpdatePassword([FromBody]UpdatePassword model)
        {
            var user = _userService.GetByUsernameAndPassword(model.UserName, model.CurrentPassword);
                
            // validações
            if (user == null)
                return NotFound(new { message = "Usuário não cadastrado" });

            if (string.IsNullOrEmpty(model.NewPassword))
                return BadRequest(new { message = "A nova senha não pode ser vazia" });
            
            if (!model.NewPassword.Equals(model.ConfirmNewPassword))
                return BadRequest(new { message = "A confirmação e a nova senha devem ser iguais" });

            // nova senha
            user.Password = model.NewPassword;

            return Ok(_userService.UpdatePassword(user));  
        }

        [Authorize]
        [Route("delete")]
        [HttpDelete]
        public ActionResult<User> Delete([FromBody]User user)
        {
            var deletedUser = _userService.Delete(user);
            if (deletedUser == null)
                return NotFound();

            return deletedUser;
        }
    }
}
