using Application.Common.Pagination;
using Application.Constants.Messages;
using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Interface;
using Domain.Models;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Implements
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepo;
        private readonly IRoleRepository _roleRepo;
        private readonly IMapper _mapper;
        private readonly IValidator<UserCreateDto> _createValidator;
        private readonly IValidator<UserUpdateDto> _updateValidator;
        private readonly IValidator<UserProfileUploadDto> _profileValidator;

        public UserService(
            IUserRepository userRepo,
            IRoleRepository roleRepo,
            IMapper mapper,
            IValidator<UserCreateDto> createValidator,
            IValidator<UserUpdateDto> updateValidator,
            IValidator<UserProfileUploadDto> profileValidator)
        {
            _userRepo = userRepo;
            _roleRepo = roleRepo;
            _mapper = mapper;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _profileValidator = profileValidator;
        }

        public List<UserDto> GetAll() =>
            _userRepo.QueryWithRoles()
                  .AsNoTracking()
                  .Where(u => u.Status != "Deleted")
                  .ProjectTo<UserDto>(_mapper.ConfigurationProvider)
                  .ToList();

        public UserDto? GetById(int id) =>
            _userRepo.QueryWithRoles()
                  .AsNoTracking()
                  .Where(u => u.UserId == id && u.Status != "Deleted")
                  .ProjectTo<UserDto>(_mapper.ConfigurationProvider)
                  .FirstOrDefault();

        public List<UserDto> GetAllWithRoles() =>
            _userRepo.QueryWithRoles()
                  .Where(u => u.Status != "Deleted")
                  .ProjectTo<UserDto>(_mapper.ConfigurationProvider)
                  .ToList();

        public UserDto? GetByIdWithRoles(int id) =>
            _userRepo.QueryWithRoles()
                  .Where(u => u.UserId == id && u.Status != "Deleted")
                  .ProjectTo<UserDto>(_mapper.ConfigurationProvider)
                  .FirstOrDefault();

        public UserDto? GetByIdWithPartner(int id)
        {
            var user = _userRepo.GetByIdWithPartner(id);
            return user == null ? null : _mapper.Map<UserDto>(user);
        }

        public string? GetAvatarBase64(int id)
        {
            var user = _userRepo.GetById(id);
            return user?.AvatarBase64;
        }

        public UserDto Create(UserCreateDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto), UserMessages.REQUEST_NULL);

            var validationResult = _createValidator.Validate(dto);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            if (_userRepo.QueryWithRolesIncludeDeleted().Any(u => u.UserName == dto.UserName))
                throw new InvalidOperationException(string.Format(UserMessages.USERNAME_EXISTED, dto.UserName));

            if (_userRepo.QueryWithRolesIncludeDeleted().Any(u => u.Email == dto.Email))
                throw new InvalidOperationException(string.Format(UserMessages.EMAIL_EXISTED, dto.Email));

            var userEntity = _mapper.Map<User>(dto);
            userEntity.CreatedAt = DateTime.Now;
            userEntity.Status = "Active";

            if (dto.AvatarFile != null)
            {
                using var memoryStream = new MemoryStream();
                dto.AvatarFile.CopyTo(memoryStream);
                userEntity.AvatarBase64 = Convert.ToBase64String(memoryStream.ToArray());
            }

            _userRepo.Add(userEntity);

            if (dto.RoleIds != null && dto.RoleIds.Any())
            {
                _userRepo.AssignRoles(userEntity.UserId, dto.RoleIds);
            }

            var createdUser = _userRepo.QueryWithRolesIncludeDeleted().First(u => u.UserId == userEntity.UserId);
            return _mapper.Map<UserDto>(createdUser);
        }

        public void Update(int id, UserUpdateDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto), UserMessages.REQUEST_NULL);

            var validationResult = _updateValidator.Validate(dto);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var existingUser = _userRepo.QueryWithRolesIncludeDeleted().FirstOrDefault(u => u.UserId == id);
            if (existingUser == null)
                throw new KeyNotFoundException(UserMessages.USER_NOT_FOUND);

            if (existingUser.Status == "Deleted")
                throw new InvalidOperationException(UserMessages.CANNOT_UPDATE_DELETED);

            if (!string.IsNullOrEmpty(dto.Email) && dto.Email != existingUser.Email)
            {
                if (_userRepo.QueryWithRolesIncludeDeleted().Any(u => u.Email == dto.Email && u.UserId != id))
                    throw new InvalidOperationException(string.Format(UserMessages.EMAIL_EXISTED, dto.Email));
            }

            _mapper.Map(dto, existingUser);

            if (dto.AvatarFile != null)
            {
                using var memoryStream = new MemoryStream();
                dto.AvatarFile.CopyTo(memoryStream);
                existingUser.AvatarBase64 = Convert.ToBase64String(memoryStream.ToArray());
            }

            existingUser.UpdatedAt = DateTime.Now;
            _userRepo.Update(existingUser);

            if (dto.RoleIds != null)
            {
                _userRepo.AssignRoles(existingUser.UserId, dto.RoleIds);
            }
        }

        public void UpdateProfile(int id, UserProfileUploadDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto), UserMessages.REQUEST_NULL);

            var validationResult = _profileValidator.Validate(dto);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var existingUser = _userRepo.GetById(id);
            if (existingUser == null)
                throw new KeyNotFoundException(UserMessages.USER_NOT_FOUND);

            if (existingUser.Status == "Deleted")
                throw new InvalidOperationException(UserMessages.CANNOT_UPDATE_DELETED);

            if (dto.FullName != null) existingUser.FullName = dto.FullName;
            if (dto.Phone != null) existingUser.Phone = dto.Phone;

            if (dto.AvatarFile != null)
            {
                using var memoryStream = new MemoryStream();
                dto.AvatarFile.CopyTo(memoryStream);
                existingUser.AvatarBase64 = Convert.ToBase64String(memoryStream.ToArray());
            }

            existingUser.UpdatedAt = DateTime.Now;
            _userRepo.Update(existingUser);
        }

        public void Delete(int id)
        {
            var user = _userRepo.GetById(id);
            if (user == null || user.Status == "Deleted") return;

            user.Status = "Deleted";
            user.UpdatedAt = DateTime.Now;
            _userRepo.Update(user);
        }

        public void Restore(int id, string status)
        {
            var normalizedStatus = (status ?? string.Empty).Trim();

            if (!string.Equals(normalizedStatus, "Active", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(normalizedStatus, "Inactive", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException(UserMessages.STATUS_INVALID);
            }

            var user = _userRepo.QueryWithRolesIncludeDeleted().FirstOrDefault(x => x.UserId == id);

            if (user == null)
                throw new KeyNotFoundException(UserMessages.USER_NOT_FOUND);

            if (user.Status != "Deleted") return;

            user.Status = char.ToUpperInvariant(normalizedStatus[0]) + normalizedStatus.Substring(1).ToLowerInvariant();
            user.UpdatedAt = DateTime.Now;
            _userRepo.Update(user);
        }

        public PagedResultDto<UserDto> GetUsersFiltered(UserPagedQueryDto query, List<string>? statuses = null)
        {
            var queryable = _userRepo.QueryWithRoles().AsNoTracking().Where(u => u.Status != "Deleted");
            return FilterAndPageUsers(queryable, query, statuses, includeDeleted: false);
        }

        public PagedResultDto<UserDto> GetUsersFilteredIncludeDeleted(UserPagedQueryDto query, List<string>? statuses = null)
        {
            var queryable = _userRepo.QueryWithRolesIncludeDeleted().AsNoTracking();
            return FilterAndPageUsers(queryable, query, statuses, includeDeleted: true);
        }

        private PagedResultDto<UserDto> FilterAndPageUsers(
            IQueryable<User> queryable,
            UserPagedQueryDto queryDto,
            List<string>? statuses,
            bool includeDeleted)
        {
            if (statuses != null && statuses.Count > 0)
            {
                var statusSet = new HashSet<string>(statuses
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .Select(s => s.Trim().ToLowerInvariant()));

                if (!includeDeleted) statusSet.Remove("deleted");

                if (statusSet.Count > 0)
                {
                    queryable = queryable.Where(u => statusSet.Contains(u.Status.ToLower()));
                }
            }

            if (!string.IsNullOrWhiteSpace(queryDto.SearchTerm))
            {
                var term = queryDto.SearchTerm.Trim();
                queryable = queryable.Where(u =>
                    (u.UserName ?? "").Contains(term) ||
                    (u.Email ?? "").Contains(term) ||
                    (u.FullName ?? "").Contains(term));
            }

            if (queryDto.Roles != null && queryDto.Roles.Count > 0)
            {
                var roleSet = queryDto.Roles
                    .Where(r => !string.IsNullOrWhiteSpace(r))
                    .Select(r => r.Trim().ToLowerInvariant())
                    .ToHashSet();

                queryable = queryable.Where(u => u.UserRoles.Any(ur =>
                    roleSet.Contains(ur.Role.RoleName.ToLower()) ||
                    roleSet.Contains(ur.Role.RoleId.ToString())
                ));
            }

            var pageNumber = queryDto.PageNumber > 0 ? queryDto.PageNumber : 1;
            var pageSize = queryDto.PageSize > 0 ? queryDto.PageSize : 10;
            var totalCount = queryable.Count();

            var data = queryable
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ProjectTo<UserDto>(_mapper.ConfigurationProvider)
                .ToList();

            return new PagedResultDto<UserDto>
            {
                Data = data,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };
        }
    }
}