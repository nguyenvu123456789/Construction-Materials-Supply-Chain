using Application.Common.Pagination;
using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Services.Implementations;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _users;

        public UsersController(IUserService users)
        {
            _users = users;
        }

        [HttpGet]
        public ActionResult<IEnumerable<UserDto>> GetUsers()
        {
            var result = _users.GetAll();
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public ActionResult<UserDto> GetUser(int id)
        {
            var user = _users.GetById(id);
            if (user == null) return NotFound();
            return Ok(user);
        }

        [HttpPost]
        public ActionResult<UserDto> CreateUser([FromBody] UserCreateDto dto)
        {
            var created = _users.Create(dto);
            return CreatedAtAction(nameof(GetUser), new { id = created.UserId }, created);
        }

        [HttpPut("{id:int}")]
        public IActionResult UpdateUser(int id, [FromBody] UserUpdateDto dto)
        {
            _users.Update(id, dto);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public IActionResult DeleteUser(int id)
        {
            _users.Delete(id);
            return NoContent();
        }

        [HttpGet("filter")]
        public ActionResult<PagedResultDto<UserDto>> GetUsersFiltered([FromQuery] UserPagedQueryDto query)
        {
            var page = _users.GetUsersFiltered(query);
            return Ok(page);
        }
    }
}
