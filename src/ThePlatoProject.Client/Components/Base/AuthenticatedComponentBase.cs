using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using ThePlatoProject.Client.Authentication;
using ThePlatoProject.Contracts.Authentication;
using ThePlatoProject.Contracts.Authorization;

namespace ThePlatoProject.Client.Components.Base;

public abstract class AuthenticatedComponentBase : ComponentBase
{
    [CascadingParameter]
    protected Task<AuthenticationState> AuthStateTask { get; set; }
        = default!;

    protected ClaimsPrincipal AuthUser { get; private set; }
        = new(new ClaimsIdentity());

    protected UserInfo? UserInfo { get; private set; }

    protected bool AuthReady { get; private set; }

    protected bool IsAuthenticated =>
        AuthUser.Identity?.IsAuthenticated == true;

    protected bool IsAdmin => UserIsInRole(AppRoles.Admin);
    protected bool IsSiteManager => UserIsInRole(AppRoles.SiteManager);
    protected bool IsArchivist => UserIsInRole(AppRoles.Archivist);

    protected int? AssignedSiteId => UserInfo?.AssignedSiteId;

    protected sealed override async Task OnInitializedAsync()
    {
        AuthenticationState authenticationState = await AuthStateTask;
        AuthUser = authenticationState.User;

        if (IsAuthenticated)
        {
            UserInfo = UserInfoFactory.FromPrincipal(AuthUser);
        }

        AuthReady = true;
        await OnInitializedWithAuthAsync();
    }

    protected virtual Task OnInitializedWithAuthAsync() =>
        Task.CompletedTask;

    protected bool UserIsInRole(string role) =>
        AuthUser.IsInRole(role);

    protected bool UserIsAnyRole(params string[] roles) =>
        roles.Any(AuthUser.IsInRole);

    protected bool UserIsAssignedToSite(int siteId) =>
        AssignedSiteId == siteId;

    protected bool HasOrganizationWideReadScope =>
        UserInfo is not null &&
        AppRoles.HasOrganizationWideReadScope(UserInfo.Role);
}
