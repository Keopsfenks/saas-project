using Application.Services;
using Domain.Entities;
using System.Security.Claims;

namespace WebUI.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IRepositoryService<User> _userRepository;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor, IRepositoryService<User> userRepository)
        {
            _httpContextAccessor = httpContextAccessor;
            _userRepository = userRepository;
        }

        public string UserId { get { return GetUserId(); } }
        public string? IpAddress => _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString().Split(',').FirstOrDefault();
        public string? Language => _httpContextAccessor.HttpContext?.Request.Headers.AcceptLanguage;
        public string? Email => _httpContextAccessor.HttpContext?.User?.Claims?.First(x => x.Type == "fullName")?.Value;
        public string? Token => string.Empty;
        public string? MobileAppType => string.Empty;

        private string GetUserId()
        {
            try
            {
                var nameIdentifier = _httpContextAccessor.HttpContext?.User?.Claims?.First(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
                if (nameIdentifier == null) return string.Empty;

                var user = _userRepository.FindOneAsync(f => f.IsDeleted == false && f.Id == nameIdentifier).Result;
                if (user == null) return string.Empty;
                return user.Id;
            }
            catch (Exception) { return string.Empty; }
        }
    }
}
