using EcommerceWebApi.Authentication;
using EcommerceWebApi.Entities;
using EcommerceWebApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;

namespace EcommerceWebApi.Filters
{
    public class AuthorizeFilter : IAsyncAuthorizationFilter
    {
        private readonly AuthService _authService;
        private readonly UserService _userService;
        private readonly JwtService _jwtService;
        private readonly ILogger<AuthorizeFilter> _logger;

        public AuthorizeFilter(
            AuthService authService,
            UserService userService,
            JwtService jwtService,
            ILogger<AuthorizeFilter> logger
        )
        {
            _authService = authService;
            _userService = userService;
            _jwtService = jwtService;
            _logger = logger;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            try
            {
                var allowAnonymous = context.ActionDescriptor.EndpointMetadata
                    .OfType<AllowAnonymousAttribute>()
                    .Any();
                if (allowAnonymous)
                {
                    return;
                }

                var allowFirstFactor = context.ActionDescriptor.EndpointMetadata
                    .OfType<AllowFirstFactorAttribute>()
                    .Any();

                var requiredRoles = context.ActionDescriptor.EndpointMetadata
                    .OfType<AuthorizeRoleAttribute>()
                    .Select(a => a.Role);

                var accessTokenString = context.HttpContext.Request.Cookies["access_token"];
                var refreshTokenString = context.HttpContext.Request.Cookies["refresh_token"];

                if (!allowFirstFactor)
                {
                    var (refreshTokenValid, user) = await CheckRefreshTokenAsync(refreshTokenString, context).ConfigureAwait(false);
                    if (!refreshTokenValid)
                    {
                        return;
                    }
                    ;

                    if (string.IsNullOrEmpty(accessTokenString) && user != null)
                    {
                        _ = await _jwtService.GenerateJWT(
                            user,
                            isSecondFactorChecked: true,
                            context.HttpContext
                        ).ConfigureAwait(false);

                        return;
                    }
                    else
                    {
                        var (accessTokenValid, accessToken, currentUser) = await CheckAccessTokenAsync(
                            accessTokenString,
                            user,
                            context
                        ).ConfigureAwait(false);

                        if (!accessTokenValid)
                        {
                            return;
                        }

                        if (requiredRoles.Any() && !CheckRole(accessToken!, requiredRoles, context))
                        {
                            return;
                        }
                        ;

                        _authService.CurrentUser = currentUser!;
                    }
                }
                else
                {
                    var (accessTokenValid, accessToken, currentUser) = await CheckAccessTokenAsync(
                        accessTokenString,
                        null,
                        context
                    ).ConfigureAwait(false);

                    if (!accessTokenValid)
                    {
                        return;
                    }
                    ;
                    if (requiredRoles.Any() && !CheckRole(accessToken!, requiredRoles, context))
                    {
                        return;
                    }
                    ;

                    _authService.CurrentUser = currentUser!;

                    context.Result = new ContentResult()
                    {
                        Content = "Need second factor authentication",
                        StatusCode = StatusCodes.Status200OK
                    };
                    return;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message} - {ex.InnerException?.Message}");
                context.Result = new ContentResult()
                {
                    Content = "Unauthorized",
                    StatusCode = StatusCodes.Status401Unauthorized
                };
                return;
            }
        }

        private async Task<(bool, User?)> CheckRefreshTokenAsync(
            string? refreshTokenString,
            AuthorizationFilterContext context
        )
        {
            try
            {
                if (string.IsNullOrEmpty(refreshTokenString))
                {
                    context.Result = new ContentResult()
                    {
                        Content = "Unauthorized",
                        StatusCode = StatusCodes.Status401Unauthorized
                    };
                    return (false, null);
                }

                var user = await _userService.GetUserByTokenAsync(token: refreshTokenString).ConfigureAwait(false);

                if (user == null || user.RefreshToken.Expires < DateTime.UtcNow)
                {
                    context.Result = new ContentResult()
                    {
                        Content = "Unauthorized",
                        StatusCode = StatusCodes.Status401Unauthorized
                    };
                    return (false, null);
                }

                return (true, user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message} - {ex.InnerException?.Message}");
                context.Result = new ContentResult()
                {
                    Content = "Unauthorized",
                    StatusCode = StatusCodes.Status401Unauthorized
                };
                return (false, null);
            }
        }

        private async Task<(bool, JwtSecurityToken?, User?)> CheckAccessTokenAsync(
            string? accessTokenString,
            User? user,
            AuthorizationFilterContext context
        )
        {
            try
            {
                var accessToken = new JwtSecurityTokenHandler().ReadJwtToken(accessTokenString);
                var accessTokenId = accessToken.Claims.FirstOrDefault(x => x.Type == "id");
                var isSecondFactorChecked = accessToken.Claims.FirstOrDefault(
                    x => x.Type == "status"
                );

                if (accessTokenId == null || isSecondFactorChecked == null)
                {
                    return (false, null, null);
                }

                var currentUser = await _userService.GetUserByIdAsync(accessTokenId.Value).ConfigureAwait(false);

                if (
                    (
                        user != null
                        && accessTokenId.Value == user.Id
                        && isSecondFactorChecked.Value == "True"
                    ) || (user == null && currentUser != null)
                )
                {
                    return (true, accessToken, currentUser);
                }
                context.Result = new ContentResult()
                {
                    Content = "Unauthorized",
                    StatusCode = StatusCodes.Status401Unauthorized
                };
                return (false, null, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message} - {ex.InnerException?.Message}");
                context.Result = new ContentResult()
                {
                    Content = "Unauthorized",
                    StatusCode = StatusCodes.Status401Unauthorized
                };
                return (false, null, null);
            }
        }

        private bool CheckRole(
            JwtSecurityToken accessToken,
            IEnumerable<string>? requiredRoles,
            AuthorizationFilterContext context
        )
        {
            try
            {
                if (requiredRoles == null)
                {
                    return false;
                }

                if (requiredRoles.Any())
                {
                    var userRoleClaim = accessToken.Claims.FirstOrDefault(c => c.Type == "role");
                    if (userRoleClaim == null || !requiredRoles.Contains(userRoleClaim.Value))
                    {
                        context.Result = new ContentResult()
                        {
                            Content = "Forbidden",
                            StatusCode = StatusCodes.Status403Forbidden
                        };
                        return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message} - {ex.InnerException?.Message}");
                context.Result = new ContentResult()
                {
                    Content = "Forbidden",
                    StatusCode = StatusCodes.Status403Forbidden
                };
                return false;
            }
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class AllowAnonymousAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Method)]
    public class AllowFirstFactorAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class AuthorizeRoleAttribute : Attribute
    {
        public string Role { get; }

        public AuthorizeRoleAttribute(string role)
        {
            Role = role;
        }
    }
}
