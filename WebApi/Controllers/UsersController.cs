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
                return NotFound(new { message = "Usuário não encontrado" });

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
            if (string.IsNullOrEmpty(user.FirstName))
                ModelState.AddModelError("message", "O nome é obrigatório");
            
            if (string.IsNullOrEmpty(user.LastName))
                ModelState.AddModelError("message", "O sobrenome é obrigatório");

            if (string.IsNullOrEmpty(user.Username))
                ModelState.AddModelError("message", "O usuário é obrigatório");
            
            if (string.IsNullOrEmpty(user.Password))
                ModelState.AddModelError("message", "A senha é obrigatória");

            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
                
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

        [Authorize]
        [Route("updatepassword")]
        [HttpPut]
        public IActionResult UpdatePassword([FromBody]UpdatePassword model)
        {
            var user = _userService.GetByUsernameAndPassword(model.UserName, model.CurrentPassword);
                
            // validações
            if (user == null)
                return NotFound(new { message = "Usuário não cadastrado" });

            if (string.IsNullOrEmpty(model.NewPassword))
                ModelState.AddModelError("message", "A nova senha é obrigatória");
            
            if (!model.NewPassword.Equals(model.ConfirmNewPassword))
                ModelState.AddModelError("message", "A confirmação e a nova senha devem ser iguais");

            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

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
