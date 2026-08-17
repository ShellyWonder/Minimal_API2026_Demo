using System;
using System.Collections.Generic;
using System.Text;

namespace ThePlatoProject.Contracts.Authorization
{
    public static class AppRoles
    {
        public const string Admin = "Admin";
        public const string SiteManager = "Site Manager";
        public const string Archivist = "Archivist";
        public const string FieldResearcher = "Field Researcher";
        public const string FieldStaff = "Field Staff";
        public const string OffSitePersonnel = "Off-site Personnel";

        public static bool IsOnSiteRole(string? role) =>
       role is SiteManager or FieldResearcher or FieldStaff;

        public static bool HasOrganizationWideReadScope(string? role) =>
            role is Admin or Archivist or OffSitePersonnel;
        public static readonly List<string> AllRoles = new List<string>
        {
            Admin,
            SiteManager,
            Archivist,
            FieldResearcher,
            FieldStaff,
            OffSitePersonnel
        };
        public static readonly List<string> OnSiteRoles = new List<string>
        {
            SiteManager,
            FieldResearcher,
            FieldStaff,
        };

    }
}
