using Application.Common.Pagination;
using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

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

        //[HttpPost]
        //public ActionResult<UserDto> CreateUser([FromBody] UserCreateDto dto)
        //{
        //    var created = _users.Create(dto);
        //    return CreatedAtAction(nameof(GetUser), new { id = created.UserId }, created);
        //}

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

        [HttpPost("{id:int}/restore")]
        public IActionResult RestoreUser(int id, [FromQuery] string status = "Active")
        {
            _users.Restore(id, status);
            return NoContent();
        }

        [HttpGet("filter")]
        public ActionResult<PagedResultDto<UserDto>> GetUsersFiltered([FromQuery] UserPagedQueryDto query, [FromQuery] List<string>? statuses)
        {
            var page = _users.GetUsersFiltered(query, statuses);
            return Ok(page);
        }

        [HttpGet("filter-all")]
        public ActionResult<PagedResultDto<UserDto>> GetUsersFilteredIncludeDeleted([FromQuery] UserPagedQueryDto query, [FromQuery] List<string>? statuses)
        {
            var page = _users.GetUsersFilteredIncludeDeleted(query, statuses);
            return Ok(page);
        }
    }
}
