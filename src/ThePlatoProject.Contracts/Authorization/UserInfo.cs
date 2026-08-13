namespace ThePlatoProject.Contracts.Authentication;

public sealed class UserInfo
{
    public required string UserId { get; init; }
    public required string Email { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string Role { get; init; }

    public int? AssignedSiteId { get; init; }
    public string? AssignedSiteName { get; init; }

    public string FullName => $"{FirstName} {LastName}".Trim();

    public string Initials
    {
        get
        {
            string first = string.IsNullOrWhiteSpace(FirstName)
                ? string.Empty
                : FirstName[..1];

            string last = string.IsNullOrWhiteSpace(LastName)
                ? string.Empty
                : LastName[..1];

            string initials = $"{first}{last}".ToUpperInvariant();
            return string.IsNullOrWhiteSpace(initials) ? "?" : initials;
        }
    }
}
