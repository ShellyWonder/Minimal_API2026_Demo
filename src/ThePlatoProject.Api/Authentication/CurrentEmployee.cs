namespace MinimalAPI2026Demo.Authentication;

public sealed record CurrentEmployee(
    ApplicationUser User,
    string Role)
{
    public string UserId => User.Id;
    public int? AssignedSiteId => User.AssignedSiteId;
}
