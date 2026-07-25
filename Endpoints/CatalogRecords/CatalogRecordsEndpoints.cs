namespace MinimalAPI2026Demo.Endpoints.CatalogRecords
{
    public  static class CatalogRecordsEndpoints
    {
        public static IEndpointRouteBuilder MapCatalogRecordsEndpoints(this IEndpointRouteBuilder route)
        {
            #region Groups
            var publicGroup = route.MapGroup("api/public/catalogRecords")
                .AllowAnonymous()
                .WithTags("Catalog Records - Public")
                .WithSummary("Public Catalog Records Endpoints")
                .WithDescription("Returns publically available catalog records.")
                .AddEndpointFilter<ExceptionHandlingFilter>();

            var privateGroup = route.MapGroup("api/private/catalogRecords")
                .RequireAuthorization()
                .WithTags("Catalog Records - Private")
                .WithSummary("Private Catalog Records Endpoints")
                .WithDescription("Returns private catalog records - authorization required.")
                .AddEndpointFilter<ExceptionHandlingFilter>();
            #endregion

            #region Get Endpoints

            #endregion

            #region Create | Update | Delete

            #endregion

            return route;
        }

        #region Handlers


        #region Get Record by Id

        #endregion

        #region Get Records <List>

        #endregion

        #region Create | Update | Delete

        #endregion

        #endregion

    }
}
