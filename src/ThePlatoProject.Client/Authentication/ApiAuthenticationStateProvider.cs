using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using ThePlatoProject.Client.Authorization.Records;
using ThePlatoProject.Client.Services.Interfaces;
using ThePlatoProject.Contracts.Authentication;

namespace ThePlatoProject.Client.Authentication;

public sealed class ApiAuthenticationStateProvider(HttpClient httpClient)
    : AuthenticationStateProvider, IAuthenticationService
{
    private static readonly ClaimsPrincipal AnonymousPrincipal =
        new(new ClaimsIdentity());

    private static readonly AuthenticationState AnonymousState =
        new(AnonymousPrincipal);

    private UserInfo? _currentUser;

    public override async Task<AuthenticationState>
        GetAuthenticationStateAsync()
    {
        if (_currentUser is not null) return CreateAuthenticationState(_currentUser);
        

        return await RefreshAuthenticationStateAsync(notify: false);
    }

    public async Task<AccountResult> LoginAsync(
        PlatoLoginRequest request,
        CancellationToken ct = default)
    {
        string cookieMode = request.RememberMe
            ? "useCookies=true"
            : "useSessionCookies=true";

        var identityRequest = new
        {
            request.Email,
            request.Password
        };

        using HttpResponseMessage response = await httpClient.PostAsJsonAsync(
            $"api/auth/login?{cookieMode}",
            identityRequest,
            ct);

        if (!response.IsSuccessStatusCode)
        {
            return new AccountResult(
                false,
                response.StatusCode == HttpStatusCode.Unauthorized
                    ? "Invalid email or password."
                    : "Unable to sign in. Please try again.");
        }

        AuthenticationState state =
            await RefreshAuthenticationStateAsync(notify: true, ct);

        return state.User.Identity?.IsAuthenticated == true
            ? new AccountResult(true)
            : new AccountResult(false, "The account could not be loaded.");
    }

    public async Task LogoutAsync(CancellationToken ct = default)
    {
        using HttpResponseMessage _ = await httpClient.PostAsync(
            "api/auth/logout",
            content: null,
            ct);

        _currentUser = null;

        NotifyAuthenticationStateChanged(
            Task.FromResult(AnonymousState));
    }

    private async Task<AuthenticationState> RefreshAuthenticationStateAsync(
        bool notify,
        CancellationToken ct = default)
    {
        try
        {
            using HttpResponseMessage response = await httpClient.GetAsync(
                "api/auth/user-info",
                ct);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                _currentUser = null;
                return AnonymousState;
            }

            if (!response.IsSuccessStatusCode) return AnonymousState;
            
            _currentUser = await response.Content
                .ReadFromJsonAsync<UserInfo>(cancellationToken: ct);

            AuthenticationState state = _currentUser is null
                ? AnonymousState
                : CreateAuthenticationState(_currentUser);

            if (notify) NotifyAuthenticationStateChanged(Task.FromResult(state));

           
            return state;
        }
        catch (HttpRequestException)
        {
            _currentUser = null;
            return AnonymousState;
        }
    }

    private static AuthenticationState CreateAuthenticationState(
        UserInfo userInfo)
    {
        Claim[] claims =
        [
            new(ClaimTypes.NameIdentifier, userInfo.UserId),
            new(ClaimTypes.Name, userInfo.FullName),
            new(ClaimTypes.Email, userInfo.Email),
            new(ClaimTypes.Role, userInfo.Role),
            new(nameof(UserInfo.FirstName), userInfo.FirstName),
            new(nameof(UserInfo.LastName), userInfo.LastName),
            .. CreateOptionalSiteClaims(userInfo)
        ];

        ClaimsIdentity identity = new(
            claims,
            authenticationType: nameof(ApiAuthenticationStateProvider));

        return new AuthenticationState(new ClaimsPrincipal(identity));
    }

    private static IEnumerable<Claim> CreateOptionalSiteClaims(
        UserInfo userInfo)
    {
        if (userInfo.AssignedSiteId is int siteId)
        {
            yield return new Claim(
                nameof(UserInfo.AssignedSiteId),
                siteId.ToString());
        }

        if (!string.IsNullOrWhiteSpace(userInfo.AssignedSiteName))
        {
            yield return new Claim(
                nameof(UserInfo.AssignedSiteName),
                userInfo.AssignedSiteName);
        }
    }
}
