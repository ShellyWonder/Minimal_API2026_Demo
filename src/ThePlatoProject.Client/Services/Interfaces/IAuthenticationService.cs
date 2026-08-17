using ThePlatoProject.Contracts.Authentication;

namespace ThePlatoProject.Client.Services.Interfaces;

public interface IAuthenticationService
{
    Task<AccountResult> LoginAsync(
        PlatoLoginRequest request,
        CancellationToken ct = default);

    Task LogoutAsync(CancellationToken ct = default);
}
