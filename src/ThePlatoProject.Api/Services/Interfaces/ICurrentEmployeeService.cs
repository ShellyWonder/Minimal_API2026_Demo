namespace MinimalAPI2026Demo.Services.Interfaces;

public interface ICurrentEmployeeService
{
    Task<CurrentEmployee?> GetAsync(
        ClaimsPrincipal principal,
        CancellationToken ct);
}
