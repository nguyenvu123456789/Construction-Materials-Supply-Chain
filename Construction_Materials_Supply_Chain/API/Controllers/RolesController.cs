using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RolesController : ControllerBase
    {
        private readonly IRoleService _roles;

        public RolesController(IRoleService roles)
        {
            _roles = roles;
        }

        [HttpGet]
        public ActionResult<IEnumerable<RoleDto>> GetRoles()
        {
            var result = _roles.GetAll();
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public ActionResult<RoleDto> GetRole(int id)
        {
            var role = _roles.GetById(id);
            if (role == null) return NotFound();
            return Ok(role);
        }

        [HttpPost]
        public ActionResult<RoleDto> CreateRole([FromBody] RoleCreateDto dto)
        {
            var created = _roles.Create(dto);
            return CreatedAtAction(nameof(GetRole), new { id = created.RoleId }, created);
        }

        [HttpPut("{id:int}")]
        public IActionResult UpdateRole(int id, [FromBody] RoleUpdateDto dto)
        {
            _roles.Update(id, dto);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public IActionResult DeleteRole(int id)
        {
            _roles.Delete(id);
            return NoContent();
        }
    }
}
