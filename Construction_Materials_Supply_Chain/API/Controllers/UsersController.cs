using API.DTOs;
using API.Helper.Paging;
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
        private readonly IRoleRepository _roleRepository;
        private readonly IMapper _mapper;

        public UsersController(IUserRepository repository, IRoleRepository roleRepository, IMapper mapper)
        {
            _repository = repository;
            _roleRepository = roleRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult<IEnumerable<UserDto>> GetUsers()
        {
            var users = _repository.GetUsers();
            var result = _mapper.Map<IEnumerable<UserDto>>(users);
            return Ok(result);
        }

        [HttpGet("roles")]
        public ActionResult<IEnumerable<RoleDto>> GetRoles()
        {
            var roles = _roleRepository.GetRoles();
            var result = _mapper.Map<IEnumerable<RoleDto>>(roles);
            return Ok(result);
        }

        [HttpPost]
        public IActionResult PostUser(UserDto userDto)
        {
            var user = _mapper.Map<User>(userDto);
            _repository.SaveUser(user);
            return NoContent();
        }

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

        [HttpDelete("{id}")]
        public IActionResult DeleteUser(int id)
        {
            var existing = _repository.GetUserById(id);
            if (existing == null) return NotFound();

            _repository.DeleteUserById(id);
            return NoContent();
        }

        [HttpGet("filter")]
        public ActionResult<PagedResultDto<UserDto>> GetUsersFiltered([FromQuery] UserPagedQueryDto queryParams)
        {
            var users = _repository.GetUsersPaged(queryParams.SearchTerm, queryParams.Roles, queryParams.PageNumber, queryParams.PageSize);
            var totalCount = _repository.GetTotalUsersCount(queryParams.SearchTerm, queryParams.Roles);

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