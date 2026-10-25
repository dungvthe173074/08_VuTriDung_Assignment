using System.Net.Http.Headers;

namespace _08_VuTriDung_Assignment02_FrontEnd.Services
{
    public class JwtAuthorizationHeaderHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public JwtAuthorizationHeaderHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext != null)
            {
                var token = httpContext.Session.GetString("JWToken");

                if (string.IsNullOrEmpty(token) && httpContext.User.Identity != null && httpContext.User.Identity.IsAuthenticated)
                {
                    token = httpContext.User.FindFirst("Token")?.Value;
                }

                if (!string.IsNullOrEmpty(token) && request.Headers.Authorization == null)
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
