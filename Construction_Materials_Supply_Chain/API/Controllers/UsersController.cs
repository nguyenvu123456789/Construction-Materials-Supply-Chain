using Application.DTOs;
using Application.Common.Pagination;
using Application.Interfaces;
using AutoMapper;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        private readonly IMapper _mapper;

        public UsersController(IUserService userService, IRoleService roleService, IMapper mapper)
        {
            _userService = userService;
            _roleService = roleService;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult<IEnumerable<UserDto>> GetUsers()
        {
            var users = _userService.GetAll();
            var result = _mapper.Map<IEnumerable<UserDto>>(users);
            return Ok(result);
        }

        [HttpGet("roles")]
        public ActionResult<IEnumerable<RoleDto>> GetRoles()
        {
            var roles = _roleService.GetAll();
            var result = _mapper.Map<IEnumerable<RoleDto>>(roles);
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public ActionResult<UserDto> GetUser(int id)
        {
            var user = _userService.GetById(id);
            if (user == null) return NotFound();
            return Ok(_mapper.Map<UserDto>(user));
        }

        [HttpPost]
        public IActionResult PostUser(UserDto userDto)
        {
            var user = _mapper.Map<User>(userDto);
            _userService.Create(user);
            return CreatedAtAction(nameof(GetUser), new { id = user.UserId }, _mapper.Map<UserDto>(user));
        }

        [HttpPut("{id:int}")]
        public IActionResult UpdateUser(int id, UserDto userDto)
        {
            var existing = _userService.GetById(id);
            if (existing == null) return NotFound();

            var user = _mapper.Map<User>(userDto);
            user.UserId = id;
            _userService.Update(user);

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public IActionResult DeleteUser(int id)
        {
            var existing = _userService.GetById(id);
            if (existing == null) return NotFound();

            _userService.Delete(id);
            return NoContent();
        }

        // GET: api/users/filter?SearchTerm=...&PageNumber=1&PageSize=10&Roles=Admin&Roles=Manager
        [HttpGet("filter")]
        public ActionResult<PagedResultDto<UserDto>> GetUsersFiltered([FromQuery] UserPagedQueryDto queryParams)
        {
            var users = _userService.GetUsersFiltered(queryParams.SearchTerm, queryParams.Roles, queryParams.PageNumber, queryParams.PageSize, out var totalCount);

            var result = new PagedResultDto<UserDto>
            {
                Data = _mapper.Map<IEnumerable<UserDto>>(users),
                TotalCount = totalCount,
                PageNumber = queryParams.PageNumber,
                PageSize = queryParams.PageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)queryParams.PageSize)
            };

            return Ok(result);
        }
    }
}
