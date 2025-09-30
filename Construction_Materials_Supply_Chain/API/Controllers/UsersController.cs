using API.DTOs;
using AutoMapper;
using BusinessObjects;
using Microsoft.AspNetCore.Mvc;
using Repositories.Interface;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _repository;
        private readonly IMapper _mapper;

        public UsersController(IUserRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        // GET: api/Users
        [HttpGet]
        public ActionResult<IEnumerable<UserDto>> GetUsers()
        {
            var users = _repository.GetUsers();
            return Ok(_mapper.Map<IEnumerable<UserDto>>(users));
        }

        // GET: api/Users/Search?keyword=abc
        [HttpGet("Search")]
        public ActionResult<IEnumerable<UserDto>> SearchUsers(string keyword)
        {
            var users = _repository.SearchUsers(keyword);
            return Ok(_mapper.Map<IEnumerable<UserDto>>(users));
        }

        // POST: api/Users
        [HttpPost]
        public IActionResult PostUser(UserDto userDto)
        {
            var user = _mapper.Map<User>(userDto);
            _repository.SaveUser(user);
            return NoContent();
        }

        // PUT: api/Users/5
        [HttpPut("{id}")]
        public IActionResult UpdateUser(int id, UserDto userDto)
        {
            var existing = _repository.GetUserById(id);
            if (existing == null) return NotFound();

            var user = _mapper.Map<User>(userDto);
            user.UserId = id;
            _repository.UpdateUser(user);

            return NoContent();
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public IActionResult DeleteUser(int id)
        {
            var existing = _repository.GetUserById(id);
            if (existing == null) return NotFound();

            _repository.DeleteUserById(id);
            return NoContent();
        }
    }
}
