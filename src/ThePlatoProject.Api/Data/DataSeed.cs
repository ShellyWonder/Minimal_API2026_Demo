using System.Text.Json;
using ThePlatoProject.Contracts.Authorization;

namespace MinimalAPI2026Demo.Data;

public class DataSeed
{
    // Development/test credentials only. Do not use this password outside seeded environments.
    private const string SeedCorrelationId = "mvp-seed-2026-08";

    private const string PrimarySiteName = "Mid-Atlantic Ridge AZ-01";
    private const string BoundarySiteName = "Lake Vostok V-22";
    private const string PublicationRequestSiteName = "Richat Structure RQ-07";
    private const string ClosureRequestedSiteName = "Göbekli Tepe GT-03";
    private const string ClosureAuthorizedSiteName = "Yonaguni Formation YN-05";
    private const string ClosedSiteName = "Mount Kailash MK-11";

    private static readonly DateTimeOffset SeedDate =
        new(2026, 8, 1, 12, 0, 0, TimeSpan.Zero);

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };

    // A valid one-pixel PNG used only when an artifact does not have an image in SeedData/Images.
    private static readonly byte[] PlaceholderPng = Convert.FromBase64String(
        "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mP8/x8AAusB9Wl6X9sAAAAASUVORK5CYII=");

    public static async Task ManageDataAsync(IServiceProvider svcProvider)
    {
        await using var context = svcProvider.GetRequiredService<ApplicationDbContext>();
        var userManager = svcProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var configuration = svcProvider.GetRequiredService<IConfiguration>();
        var seedPassword = configuration["SeedPassword"]
       ?? throw new InvalidOperationException(
           "The SeedPassword configuration value is missing.");

        await context.Database.MigrateAsync();

        await SeedRolesAsync(svcProvider);
        var users = await SeedUsersAsync(userManager, seedPassword);

        var sites = await SeedSitesAsync(context, users);
        await AssignSeedUsersToSitesAsync(userManager, users, sites);

        await ImportArtifactsAsync(context);
        var artifacts = await EnsureWorkflowArtifactsAsync(context, sites, users);
        await ApplySiteLifecycleStatesAsync(context, sites, users);

        await SeedArtifactMediaFilesAsync(context, users);
        await EnsureWorkflowMediaAsync(context, artifacts, users);

        await ImportCatalogRecordsAsync(context, userManager);
        await EnsureWorkflowCatalogRecordsAsync(context, artifacts, users);

        await SeedDirectMessagesAsync(context, users);
        await SeedAuditLogsAsync(context, sites, artifacts, users);
        await ResetPostgresSequencesAsync(context);
    }

    #region Sites and artifacts

    public static string GetSeedPath(params string[] paths)
    {
        var basePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "SeedData");
        return Path.Combine(basePath, Path.Combine(paths));
    }

    private static async Task<WorkflowSiteSet> SeedSitesAsync(
        ApplicationDbContext context,
        SeedUserSet users)
    {
        if (!await context.Sites.AnyAsync())
        {
            var filePath = GetSeedPath("sites.json");
            if (File.Exists(filePath))
            {
                var json = await File.ReadAllTextAsync(filePath);
                var importedSites = JsonSerializer.Deserialize<List<Site>>(json, JsonOptions);

                if (importedSites is { Count: > 0 })
                {
                    context.Sites.AddRange(importedSites);
                    await context.SaveChangesAsync();
                    Console.WriteLine($"Seeded {importedSites.Count} Sites from sites.json.");
                }
            }
            else
            {
                Console.WriteLine($"Sites seed file not found: {filePath}. Creating workflow Sites.");
            }
        }

        var primary = await EnsureSiteAsync(
            context,
            PrimarySiteName,
            "North Atlantic Ocean",
            38.0000,
            -30.0000,
            "A submerged ridge site containing worked stone and metallic deposits.",
            "A multidisciplinary expedition is documenting material recovered from the Mid-Atlantic Ridge.",
            "The distribution of worked surfaces suggests repeated occupation or controlled access.");

        var boundary = await EnsureSiteAsync(
            context,
            BoundarySiteName,
            "Lake Vostok Region, East Antarctica",
            -78.4667,
            106.8000,
            "A subglacial research zone above Lake Vostok with sealed ice strata.",
            "Researchers are studying preserved formations beneath the Antarctic ice sheet.",
            "Thermal anomalies remain under internal review and are not approved for public interpretation.");

        var publicationRequest = await EnsureSiteAsync(
            context,
            PublicationRequestSiteName,
            "Adrar Province, Mauritania",
            21.1240,
            -11.4000,
            "A concentric geological structure with exposed sedimentary rings.",
            "Field teams are mapping erosion patterns and documenting surface finds.",
            "The assigned Site Manager has requested public access; Admin review remains pending.");

        var closureRequested = await EnsureSiteAsync(
            context,
            ClosureRequestedSiteName,
            "Şanlıurfa Province, Türkiye",
            37.2231,
            38.9225,
            "A monumental archaeological complex with carved limestone pillars.",
            "Documentation continues while the current field phase approaches completion.",
            "Site closure has been requested but has not yet been authorized.");

        var closureAuthorized = await EnsureSiteAsync(
            context,
            ClosureAuthorizedSiteName,
            "Okinawa Prefecture, Japan",
            24.4350,
            123.0120,
            "A submerged sandstone formation containing terraces and angular faces.",
            "Recorded features are preserved for ongoing comparative study.",
            "Closure is authorized after transfer of the documentary package to corporate custody.");

        var closed = await EnsureSiteAsync(
            context,
            ClosedSiteName,
            "Tibet Autonomous Region",
            31.0675,
            81.3119,
            "A high-altitude survey area containing unusual stone alignments.",
            "The completed survey remains available as a historical public record.",
            "Field operations are closed; all recovered material is warehoused.");

        ConfigurePublication(primary, true, VerificationStatus.Verified, users.Admin, SeedDate.AddDays(-60));
        ConfigurePublication(boundary, false, VerificationStatus.Unverified, null, null);
        ConfigurePublication(publicationRequest, false, VerificationStatus.Pending, null, null);
        ConfigurePublication(closureRequested, true, VerificationStatus.Verified, users.Admin, SeedDate.AddDays(-90));
        ConfigurePublication(closureAuthorized, true, VerificationStatus.Verified, users.Admin, SeedDate.AddDays(-120));
        ConfigurePublication(closed, true, VerificationStatus.Verified, users.Admin, SeedDate.AddDays(-180));

        await context.SaveChangesAsync();

        return new WorkflowSiteSet(
            primary,
            boundary,
            publicationRequest,
            closureRequested,
            closureAuthorized,
            closed);
    }

    private static async Task<Site> EnsureSiteAsync(
        ApplicationDbContext context,
        string name,
        string location,
        double latitude,
        double longitude,
        string description,
        string publicNarrative,
        string alrecNarrative)
    {
        var site = await context.Sites.FirstOrDefaultAsync(s => s.Name == name);
        if (site is null)
        {
            site = new Site { Name = name };
            context.Sites.Add(site);
        }

        site.Location = location;
        site.Latitude = latitude;
        site.Longitude = longitude;
        site.Description ??= description;
        site.PublicNarrative ??= publicNarrative;
        site.ALRECNarrative ??= alrecNarrative;

        return site;
    }

    private static void ConfigurePublication(
        Site site,
        bool isPublic,
        VerificationStatus status,
        ApplicationUser? verifier,
        DateTimeOffset? verifiedAtUtc)
    {
        site.IsPublic = isPublic;
        site.VerificationStatus = status;
        site.VerifiedById = status == VerificationStatus.Verified ? verifier?.Id : null;
        site.VerifiedAtUtc = status == VerificationStatus.Verified ? verifiedAtUtc : null;
    }

    private static async Task ImportArtifactsAsync(ApplicationDbContext context)
    {
        if (await context.Artifacts.AnyAsync())
        {
            return;
        }

        var filePath = GetSeedPath("artifacts.json");
        if (!File.Exists(filePath))
        {
            Console.WriteLine($"Artifact seed file not found: {filePath}. Creating workflow Artifacts.");
            return;
        }

        var json = await File.ReadAllTextAsync(filePath);
        var artifacts = JsonSerializer.Deserialize<List<Artifact>>(json, JsonOptions);

        if (artifacts is not { Count: > 0 })
        {
            return;
        }

        foreach (var artifact in artifacts)
        {
            if (artifact.DateDiscoveredUtc == default)
            {
                artifact.DateDiscoveredUtc = SeedDate.AddYears(-1);
            }

            if (artifact.DateSubmittedUtc == default)
            {
                artifact.DateSubmittedUtc = SeedDate.AddMonths(-6);
            }
        }

        context.Artifacts.AddRange(artifacts);
        await context.SaveChangesAsync();
        Console.WriteLine($"Seeded {artifacts.Count} Artifacts from artifacts.json.");
    }

    private static async Task<WorkflowArtifactSet> EnsureWorkflowArtifactsAsync(
        ApplicationDbContext context,
        WorkflowSiteSet sites,
        SeedUserSet users)
    {
        var publicArtifact = await EnsureArtifactAsync(
            context, sites.Primary, "ALREC-PUB-001", "Resonance Sphere",
            ArtifactType.CommunicationDevice, true, VerificationStatus.Verified,
            ArtifactCustodyStatus.OnSite, users,
            "A polished metallic sphere with a seamless surface and unusually high density.",
            "Recovered under controlled conditions and approved for public exhibition.");

        var privateArtifact = await EnsureArtifactAsync(
            context, sites.Primary, "ALREC-PRV-001", "Basalt Memory Tablet",
            ArtifactType.Device, false, VerificationStatus.Unverified,
            ArtifactCustodyStatus.OnSite, users,
            "A rectangular basalt tablet bearing shallow geometric incisions.",
            "Analysis is ongoing; no public interpretation has been approved.");

        var publicationPending = await EnsureArtifactAsync(
            context, sites.Primary, "ALREC-PND-001", "Luminous Alloy Fragment",
            ArtifactType.EnergySource, false, VerificationStatus.Pending,
            ArtifactCustodyStatus.OnSite, users,
            "A translucent alloy fragment that emits a faint blue-green glow under low light.",
            "The Site Manager requested public access; Admin review is pending.");

        var transferRequested = await EnsureArtifactAsync(
            context, sites.Primary, "ALREC-XFR-001", "Ceramic Harmonic Vessel",
            ArtifactType.Tool, false, VerificationStatus.Unverified,
            ArtifactCustodyStatus.TransferRequested, users,
            "A ceramic vessel with evenly spaced resonant chambers around its rim.",
            "Transfer to corporate storage has been requested.");

        var transferAuthorized = await EnsureArtifactAsync(
            context, sites.Primary, "ALREC-AUT-001", "Polar Harmonic Array",
            ArtifactType.Machine, false, VerificationStatus.Verified,
            ArtifactCustodyStatus.TransferAuthorized, users,
            "Six alloy columns arranged around a central stone aperture.",
            "Admin authorized transfer; shipment has not yet been marked sent.");

        var inTransit = await EnsureArtifactAsync(
            context, sites.Primary, "ALREC-TRN-001", "Cryogenic Interface Plate",
            ArtifactType.Device, false, VerificationStatus.Verified,
            ArtifactCustodyStatus.InTransit, users,
            "A layered plate with conductive channels that remain active below freezing.",
            "Shipment is in transit and awaits warehouse receipt.");

        var receivedAwaitingLocation = await EnsureArtifactAsync(
            context, sites.Primary, "ALREC-RCV-001", "Calcite Navigation Disc",
            ArtifactType.Device, false, VerificationStatus.Verified,
            ArtifactCustodyStatus.InTransit, users,
            "A calcite disc engraved with radial reference marks.",
            "Warehouse receipt is verified; storage location remains to be recorded.");
        receivedAwaitingLocation.ShipmentReceivedAtUtc = SeedDate.AddDays(-3);
        receivedAwaitingLocation.ShipmentReceivedById = users.Archivist.Id;
        receivedAwaitingLocation.WarehouseLocation = null;

        var readyForCompletion = await EnsureArtifactAsync(
            context, sites.Primary, "ALREC-RDY-001", "Orbital Alignment Core",
            ArtifactType.Machine, false, VerificationStatus.Verified,
            ArtifactCustodyStatus.InTransit, users,
            "A nested brass-and-stone mechanism with freely rotating concentric rings.",
            "Receipt and location are recorded; explicit transfer completion remains pending.");
        readyForCompletion.ShipmentReceivedAtUtc = SeedDate.AddDays(-3);
        readyForCompletion.ShipmentReceivedById = users.Archivist.Id;
        readyForCompletion.WarehouseLocation = "Vault B / Bay 04 / Shelf 02";

        var warehousedPublic = await EnsureArtifactAsync(
            context, sites.Primary, "ALREC-WHS-001", "Trilithon Survey Lens",
            ArtifactType.Tool, true, VerificationStatus.Verified,
            ArtifactCustodyStatus.Warehoused, users,
            "A clear mineral lens mounted in a non-ferrous triangular frame.",
            "The lens remains publicly documented after transfer to corporate storage.");

        var boundaryArtifact = await EnsureArtifactAsync(
            context, sites.Boundary, "ALREC-BND-001", "Subglacial Signal Prism",
            ArtifactType.CommunicationDevice, true, VerificationStatus.Verified,
            ArtifactCustodyStatus.OnSite, users,
            "A translucent prism recovered from a sealed ice-core chamber.",
            "The Artifact is approved at its own level, but its private parent Site makes it publicly ineligible.");

        var closureRequestedArtifact = await EnsureArtifactAsync(
            context, sites.ClosureRequested, "ALREC-CLS-001", "Limestone Survey Marker",
            ArtifactType.Monolith, false, VerificationStatus.Verified,
            ArtifactCustodyStatus.OnSite, users,
            "A carved limestone marker documented during the final active survey phase.",
            "The Site cannot complete closure while this Artifact remains on site.");

        var closureAuthorizedArtifact = await EnsureArtifactAsync(
            context, sites.ClosureAuthorized, "ALREC-YON-001", "Submerged Terrace Gauge",
            ArtifactType.Tool, true, VerificationStatus.Verified,
            ArtifactCustodyStatus.Warehoused, users,
            "A mineral gauge used to compare submerged terrace elevations.",
            "The recovered gauge is warehoused and remains part of the public historical record.");

        var closedSiteArtifact = await EnsureArtifactAsync(
            context, sites.Closed, "ALREC-KAI-001", "High-Altitude Resonance Rod",
            ArtifactType.Device, true, VerificationStatus.Verified,
            ArtifactCustodyStatus.Warehoused, users,
            "A dense metallic rod exhibiting stable resonance at low atmospheric pressure.",
            "The closed Site and warehoused Artifact remain publicly eligible.");

        await context.SaveChangesAsync();

        return new WorkflowArtifactSet(
            publicArtifact,
            privateArtifact,
            publicationPending,
            transferRequested,
            transferAuthorized,
            inTransit,
            receivedAwaitingLocation,
            readyForCompletion,
            warehousedPublic,
            boundaryArtifact,
            closureRequestedArtifact,
            closureAuthorizedArtifact,
            closedSiteArtifact);
    }

    private static async Task<Artifact> EnsureArtifactAsync(
        ApplicationDbContext context,
        Site site,
        string catalogNumber,
        string name,
        ArtifactType type,
        bool isPublic,
        VerificationStatus verificationStatus,
        ArtifactCustodyStatus custodyStatus,
        SeedUserSet users,
        string description,
        string publicNarrative)
    {
        var artifact = await context.Artifacts
            .FirstOrDefaultAsync(a => a.CatalogNumber == catalogNumber);

        if (artifact is null)
        {
            artifact = new Artifact { CatalogNumber = catalogNumber };
            context.Artifacts.Add(artifact);
        }

        artifact.SiteId = site.Id;
        artifact.Name = name;
        artifact.Type = type;
        artifact.Description = description;
        artifact.PublicNarrative = publicNarrative;
        artifact.DateDiscoveredUtc = SeedDate.AddMonths(-8);
        artifact.DateSubmittedUtc = SeedDate.AddMonths(-7);
        artifact.IsPublic = isPublic;
        artifact.VerificationStatus = verificationStatus;
        artifact.VerifiedById = verificationStatus == VerificationStatus.Verified
            ? users.Admin.Id
            : null;
        artifact.VerifiedAtUtc = verificationStatus == VerificationStatus.Verified
            ? SeedDate.AddMonths(-6)
            : null;

        ConfigureCustody(artifact, custodyStatus, users);
        return artifact;
    }

    private static void ConfigureCustody(
        Artifact artifact,
        ArtifactCustodyStatus custodyStatus,
        SeedUserSet users)
    {
        artifact.CustodyStatus = custodyStatus;
        artifact.TransferReason = null;
        artifact.TransferRequestedAtUtc = null;
        artifact.TransferRequestedById = null;
        artifact.TransferAuthorizedAtUtc = null;
        artifact.TransferAuthorizedById = null;
        artifact.ShipmentSentAtUtc = null;
        artifact.ShipmentSentById = null;
        artifact.ShipmentReceivedAtUtc = null;
        artifact.ShipmentReceivedById = null;
        artifact.WarehouseLocation = null;
        artifact.TransferCompletedAtUtc = null;
        artifact.TransferCompletedById = null;

        if (custodyStatus == ArtifactCustodyStatus.OnSite)
        {
            return;
        }

        artifact.TransferReason =
            "Field analysis complete; preserve the Artifact in controlled corporate storage.";
        artifact.TransferRequestedAtUtc = SeedDate.AddDays(-12);
        artifact.TransferRequestedById = users.SiteManager.Id;

        if (custodyStatus == ArtifactCustodyStatus.TransferRequested)
        {
            return;
        }

        artifact.TransferAuthorizedAtUtc = SeedDate.AddDays(-10);
        artifact.TransferAuthorizedById = users.Admin.Id;

        if (custodyStatus == ArtifactCustodyStatus.TransferAuthorized)
        {
            return;
        }

        artifact.ShipmentSentAtUtc = SeedDate.AddDays(-6);
        artifact.ShipmentSentById = users.SiteManager.Id;

        if (custodyStatus == ArtifactCustodyStatus.InTransit)
        {
            return;
        }

        artifact.ShipmentReceivedAtUtc = SeedDate.AddDays(-4);
        artifact.ShipmentReceivedById = users.Archivist.Id;
        artifact.WarehouseLocation = "Vault A / Bay 02 / Shelf 03";
        artifact.TransferCompletedAtUtc = SeedDate.AddDays(-1);
        artifact.TransferCompletedById = users.Archivist.Id;
    }
    private static async Task ApplySiteLifecycleStatesAsync(
        ApplicationDbContext context,
        WorkflowSiteSet sites,
        SeedUserSet users)
    {
        ConfigureActiveSite(sites.Primary);
        ConfigureActiveSite(sites.Boundary);
        ConfigureActiveSite(sites.PublicationRequest);

        sites.ClosureRequested.LifecycleStatus = SiteLifecycleStatus.ClosureRequested;
        sites.ClosureRequested.ClosureReason = "Current excavation phase complete; prepare Site records for closure.";
        sites.ClosureRequested.ClosureRequestedAtUtc = SeedDate.AddDays(-8);
        sites.ClosureRequested.ClosureRequestedById = users.SiteManager.Id;
        sites.ClosureRequested.ClosureAuthorizedAtUtc = null;
        sites.ClosureRequested.ClosureAuthorizedById = null;
        sites.ClosureRequested.ClosedAtUtc = null;
        sites.ClosureRequested.ClosedById = null;

        await ForceSiteArtifactsWarehousedAsync(context, sites.ClosureAuthorized, users);
        sites.ClosureAuthorized.LifecycleStatus = SiteLifecycleStatus.ClosureAuthorized;
        sites.ClosureAuthorized.ClosureReason = "Field work and custody transfers are complete.";
        sites.ClosureAuthorized.ClosureRequestedAtUtc = SeedDate.AddDays(-20);
        sites.ClosureAuthorized.ClosureRequestedById = users.SiteManager.Id;
        sites.ClosureAuthorized.ClosureAuthorizedAtUtc = SeedDate.AddDays(-15);
        sites.ClosureAuthorized.ClosureAuthorizedById = users.Admin.Id;
        sites.ClosureAuthorized.ClosedAtUtc = null;
        sites.ClosureAuthorized.ClosedById = null;

        await ForceSiteArtifactsWarehousedAsync(context, sites.Closed, users);
        sites.Closed.LifecycleStatus = SiteLifecycleStatus.Closed;
        sites.Closed.ClosureReason = "All field operations and custody transfers completed.";
        sites.Closed.ClosureRequestedAtUtc = SeedDate.AddDays(-45);
        sites.Closed.ClosureRequestedById = users.SiteManager.Id;
        sites.Closed.ClosureAuthorizedAtUtc = SeedDate.AddDays(-40);
        sites.Closed.ClosureAuthorizedById = users.Admin.Id;
        sites.Closed.ClosedAtUtc = SeedDate.AddDays(-30);
        sites.Closed.ClosedById = users.Archivist.Id;

        await context.SaveChangesAsync();
    }

    private static void ConfigureActiveSite(Site site)
    {
        site.LifecycleStatus = SiteLifecycleStatus.Active;
        site.ClosureReason = null;
        site.ClosureRequestedAtUtc = null;
        site.ClosureRequestedById = null;
        site.ClosureAuthorizedAtUtc = null;
        site.ClosureAuthorizedById = null;
        site.ClosedAtUtc = null;
        site.ClosedById = null;
    }

    private static async Task ForceSiteArtifactsWarehousedAsync(
        ApplicationDbContext context,
        Site site,
        SeedUserSet users)
    {
        var artifacts = await context.Artifacts
            .Where(a => a.SiteId == site.Id)
            .ToListAsync();

        foreach (var artifact in artifacts)
        {
            ConfigureCustody(artifact, ArtifactCustodyStatus.Warehoused, users);
        }
    }

    #endregion

    #region Media, Catalog Records, Notes, messages, and audit

    private static async Task SeedArtifactMediaFilesAsync(
        ApplicationDbContext context,
        SeedUserSet users)
    {
        var imagesPath = GetSeedPath("Images");
        if (!Directory.Exists(imagesPath))
        {
            Console.WriteLine($"No image folder found: {imagesPath}. Workflow placeholders will be used.");
            return;
        }

        var allowedExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            ".jpg", ".jpeg", ".png"
        };

        var primaryArtifactIds = await context.MediaFiles
            .Where(m => m.IsPrimary)
            .Select(m => m.ArtifactId)
            .ToHashSetAsync();

        foreach (var file in Directory.GetFiles(imagesPath))
        {
            var extension = Path.GetExtension(file);
            if (!allowedExtensions.Contains(extension))
            {
                continue;
            }

            var fileName = Path.GetFileName(file);
            if (await context.MediaFiles.AnyAsync(m => m.FileName == fileName))
            {
                continue;
            }

            var baseName = Path.GetFileNameWithoutExtension(file);
            var parts = baseName.Split('-');
            var catalogNumber = parts.Length >= 2 ? $"{parts[0]}-{parts[1]}" : baseName;

            var artifact = await context.Artifacts
                .FirstOrDefaultAsync(a => a.CatalogNumber == catalogNumber);
            if (artifact is null)
            {
                Console.WriteLine($"No Artifact found for image {fileName}.");
                continue;
            }

            var isVerified = artifact.VerificationStatus == VerificationStatus.Verified;
            context.MediaFiles.Add(new ArtifactMediaFile
            {
                ArtifactId = artifact.Id,
                FileName = fileName,
                ContentType = extension.Equals(".png", StringComparison.OrdinalIgnoreCase)
                    ? "image/png"
                    : "image/jpeg",
                Data = await File.ReadAllBytesAsync(file),
                IsPrimary = primaryArtifactIds.Add(artifact.Id),
                UploadedById = users.FieldResearcher.Id,
                UploadedAtUtc = SeedDate.AddMonths(-5),
                VerificationStatus = isVerified
                    ? VerificationStatus.Verified
                    : VerificationStatus.Unverified,
                VerifiedById = isVerified ? users.SiteManager.Id : null,
                VerifiedAtUtc = isVerified ? SeedDate.AddMonths(-4) : null
            });
        }

        await context.SaveChangesAsync();
    }

    private static async Task EnsureWorkflowMediaAsync(
        ApplicationDbContext context,
        WorkflowArtifactSet artifacts,
        SeedUserSet users)
    {
        foreach (var artifact in artifacts.All)
        {
            if (await context.MediaFiles.AnyAsync(m => m.ArtifactId == artifact.Id))
            {
                continue;
            }

            var isVerified = artifact.VerificationStatus == VerificationStatus.Verified;
            context.MediaFiles.Add(new ArtifactMediaFile
            {
                ArtifactId = artifact.Id,
                FileName = $"{artifact.CatalogNumber}-seed.png",
                ContentType = "image/png",
                Data = PlaceholderPng,
                IsPrimary = true,
                UploadedById = users.FieldResearcher.Id,
                UploadedAtUtc = SeedDate.AddMonths(-5),
                VerificationStatus = isVerified
                    ? VerificationStatus.Verified
                    : VerificationStatus.Unverified,
                VerifiedById = isVerified ? users.SiteManager.Id : null,
                VerifiedAtUtc = isVerified ? SeedDate.AddMonths(-4) : null
            });
        }

        await context.SaveChangesAsync();
    }

    private static async Task ImportCatalogRecordsAsync(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager)
    {
        if (await context.CatalogRecords.AnyAsync())
        {
            return;
        }

        string[] files =
        [
            "catalogRecords.atlantis.json",
            "catalogRecords.sahara.json",
            "catalogRecords.andes.json",
            "catalogRecords.antarctica.json",
            "catalogRecords.gobekli.json",
            "catalogRecords.yonaguni.json",
            "catalogRecords.kailash.json"
        ];

        foreach (var fileName in files)
        {
            var filePath = GetSeedPath(fileName);
            if (!File.Exists(filePath))
            {
                continue;
            }

            var json = await File.ReadAllTextAsync(filePath);
            var imports = JsonSerializer.Deserialize<List<CatalogRecordImport>>(json, JsonOptions);
            if (imports is null)
            {
                continue;
            }

            foreach (var import in imports)
            {
                var artifact = await context.Artifacts
                    .FirstOrDefaultAsync(a => a.CatalogNumber == import.ArtifactCatalogNumber);
                var submittedBy = await userManager.FindByEmailAsync(import.SubmittedBy);

                if (artifact is null || submittedBy is null)
                {
                    continue;
                }

                var record = new CatalogRecord
                {
                    ArtifactId = artifact.Id,
                    SubmittedById = submittedBy.Id,
                    DateSubmittedUtc = NormalizeUtc(import.DateSubmitted)
                };

                foreach (var noteImport in import.Notes)
                {
                    var author = await userManager.FindByEmailAsync(noteImport.Author);
                    if (author is null)
                    {
                        continue;
                    }

                    record.Notes.Add(new CatalogRecordNote
                    {
                        AuthorId = author.Id,
                        Content = noteImport.Content,
                        CreatedAtUtc = NormalizeUtc(noteImport.Created)
                    });
                }

                context.CatalogRecords.Add(record);
            }
        }

        await context.SaveChangesAsync();
    }

    private static async Task EnsureWorkflowCatalogRecordsAsync(
        ApplicationDbContext context,
        WorkflowArtifactSet artifacts,
        SeedUserSet users)
    {
        await EnsureCatalogRecordAsync(
            context,
            artifacts.PublicArtifact,
            users.FieldResearcher,
            SeedDate.AddMonths(-4),
            (users.FieldResearcher, "Initial measurements and image references recorded for internal comparison."),
            (users.FieldStaff, "Packaging condition and field-label numbers confirmed before storage."),
            (users.FormerEmployee, "Surface reflectance readings entered before employee deactivation."));

        await EnsureCatalogRecordAsync(
            context,
            artifacts.PrivateArtifact,
            users.SiteManager,
            SeedDate.AddMonths(-3),
            (users.FieldResearcher, "Material sample remains under laboratory review; keep employee-only."));

        await EnsureCatalogRecordAsync(
            context,
            artifacts.InTransit,
            users.FieldResearcher,
            SeedDate.AddDays(-20),
            (users.FieldStaff, "Final field inventory completed before shipment was marked sent."));

        await EnsureCatalogRecordAsync(
            context,
            artifacts.WarehousedPublic,
            users.SiteManager,
            SeedDate.AddMonths(-2),
            (users.Archivist, "Warehouse package inspected; catalog container retained as employee-only."));

        await context.SaveChangesAsync();
    }

    private static async Task EnsureCatalogRecordAsync(
        ApplicationDbContext context,
        Artifact artifact,
        ApplicationUser submittedBy,
        DateTimeOffset submittedAtUtc,
        params (ApplicationUser Author, string Content)[] notes)
    {
        var record = await context.CatalogRecords
            .Include(r => r.Notes)
            .FirstOrDefaultAsync(r => r.ArtifactId == artifact.Id);

        if (record is null)
        {
            record = new CatalogRecord
            {
                ArtifactId = artifact.Id,
                SubmittedById = submittedBy.Id,
                DateSubmittedUtc = submittedAtUtc
            };
            context.CatalogRecords.Add(record);
        }

        foreach (var (author, content) in notes)
        {
            if (record.Notes.Any(n => n.Content == content))
            {
                continue;
            }

            record.Notes.Add(new CatalogRecordNote
            {
                AuthorId = author.Id,
                Content = content,
                CreatedAtUtc = submittedAtUtc.AddHours(2 + record.Notes.Count)
            });
        }
    }

    private static async Task SeedDirectMessagesAsync(
        ApplicationDbContext context,
        SeedUserSet users)
    {
        var messages = context.Set<DirectMessage>();
        const string publicationRequestMessage =
            "Please review the pending public-access request for the Richat Structure Site.";

        if (await messages.AnyAsync(m => m.Body == publicationRequestMessage))
        {
            return;
        }

        messages.AddRange(
            new DirectMessage
            {
                SenderId = users.SiteManager.Id,
                RecipientId = users.Admin.Id,
                Body = publicationRequestMessage,
                SentAtUtc = SeedDate.AddDays(-7),
                IsRead = true,
                ReadAtUtc = SeedDate.AddDays(-7).AddHours(3)
            },
            new DirectMessage
            {
                SenderId = users.Admin.Id,
                RecipientId = users.SiteManager.Id,
                Body = "The Site request remains pending while the public narrative is reviewed.",
                SentAtUtc = SeedDate.AddDays(-6),
                IsRead = false
            },
            new DirectMessage
            {
                SenderId = users.FieldResearcher.Id,
                RecipientId = users.Archivist.Id,
                Body = "The Cryogenic Interface Plate shipment is in transit; receipt verification will be required.",
                SentAtUtc = SeedDate.AddDays(-5),
                IsRead = false
            },
            new DirectMessage
            {
                SenderId = users.FormerEmployee.Id,
                RecipientId = users.Admin.Id,
                Body = "Historical contribution record submitted before account deactivation.",
                SentAtUtc = SeedDate.AddDays(-40),
                IsRead = true,
                ReadAtUtc = SeedDate.AddDays(-39),
                IsDeletedBySender = true,
                DeletedBySenderAtUtc = SeedDate.AddDays(-38)
            });

        await context.SaveChangesAsync();
    }

    private static async Task SeedAuditLogsAsync(
        ApplicationDbContext context,
        WorkflowSiteSet sites,
        WorkflowArtifactSet artifacts,
        SeedUserSet users)
    {
        await EnsureAuditAsync(context, "SitePublished", "Site", sites.Primary.Id.ToString(),
            users.Admin, SeedDate.AddDays(-60),
            "Admin verified and published the Site.",
            changes: new { VerificationStatus = "Verified", IsPublic = true });

        await EnsureAuditAsync(context, "SitePublicationRequested", "Site", sites.PublicationRequest.Id.ToString(),
            users.SiteManager, SeedDate.AddDays(-7),
            "Assigned Site Manager requested public access; Site remains private.",
            changes: new { VerificationStatus = "Pending", IsPublic = false });

        await EnsureAuditAsync(context, "ArtifactPublicationRequested", "Artifact", artifacts.PublicationPending.Id.ToString(),
            users.SiteManager, SeedDate.AddDays(-6),
            "Assigned Site Manager requested Artifact public access.",
            changes: new { VerificationStatus = "Pending", IsPublic = false });

        await EnsureAuditAsync(context, "ArtifactPublished", "Artifact", artifacts.PublicArtifact.Id.ToString(),
            users.Admin, SeedDate.AddMonths(-6),
            "Admin independently verified and published the Artifact.",
            changes: new { VerificationStatus = "Verified", IsPublic = true });

        var verifiedMedia = await context.MediaFiles
            .FirstOrDefaultAsync(m => m.ArtifactId == artifacts.PublicArtifact.Id && m.IsPrimary);
        if (verifiedMedia is not null)
        {
            await EnsureAuditAsync(context, "ArtifactImageVerified", "ArtifactMediaFile", verifiedMedia.Id.ToString(),
                users.SiteManager, SeedDate.AddMonths(-4),
                "Assigned Site Manager verified the primary Artifact Image.",
                changes: new { VerificationStatus = "Verified" });
        }

        var catalogRecord = await context.CatalogRecords
            .Include(record => record.Notes)
            .FirstOrDefaultAsync(record => record.ArtifactId == artifacts.PublicArtifact.Id);
        if (catalogRecord is not null)
        {
            await EnsureAuditAsync(context, "CatalogRecordCreated", "CatalogRecord", catalogRecord.Id.ToString(),
                users.FieldResearcher, catalogRecord.DateSubmittedUtc,
                "Field Researcher created an employee-only Catalog Record container.");

            var firstNote = catalogRecord.Notes.OrderBy(note => note.CreatedAtUtc).FirstOrDefault();
            if (firstNote is not null)
            {
                await EnsureAuditAsync(context, "CatalogNoteCreated", "CatalogNote", firstNote.Id.ToString(),
                    users.FieldResearcher, firstNote.CreatedAtUtc,
                    "Field Researcher added a private research Note to the Catalog Record.");
            }
        }

        await EnsureAuditAsync(context, "ArtifactTransferRequested", "Artifact", artifacts.TransferRequested.Id.ToString(),
            users.SiteManager, SeedDate.AddDays(-12),
            "Site Manager requested transfer to controlled corporate storage.",
            reason: artifacts.TransferRequested.TransferReason);

        await EnsureAuditAsync(context, "ArtifactTransferAuthorized", "Artifact", artifacts.TransferAuthorized.Id.ToString(),
            users.Admin, SeedDate.AddDays(-10),
            "Admin authorized the requested custody transfer.");

        await EnsureAuditAsync(context, "ArtifactShipmentSent", "Artifact", artifacts.InTransit.Id.ToString(),
            users.SiteManager, SeedDate.AddDays(-6),
            "Site Manager marked the shipment sent; Site-based modification is now locked.");

        await EnsureAuditAsync(context, "ArtifactShipmentReceived", "Artifact", artifacts.ReceivedAwaitingLocation.Id.ToString(),
            users.Archivist, SeedDate.AddDays(-3),
            "Archivist verified warehouse receipt; location remains pending.");

        await EnsureAuditAsync(context, "ArtifactWarehouseLocationRecorded", "Artifact", artifacts.ReadyForCompletion.Id.ToString(),
            users.Archivist, SeedDate.AddDays(-2),
            "Archivist recorded the warehouse location; explicit completion remains pending.",
            changes: new { artifacts.ReadyForCompletion.WarehouseLocation });

        await EnsureAuditAsync(context, "ArtifactTransferCompleted", "Artifact", artifacts.WarehousedPublic.Id.ToString(),
            users.Archivist, SeedDate.AddDays(-1),
            "Archivist explicitly completed the transfer and set custody to Warehoused.",
            changes: new { CustodyStatus = "Warehoused" });

        await EnsureAuditAsync(context, "SiteClosureRequested", "Site", sites.ClosureRequested.Id.ToString(),
            users.SiteManager, SeedDate.AddDays(-8),
            "Site Manager requested Site closure.",
            reason: sites.ClosureRequested.ClosureReason);

        await EnsureAuditAsync(context, "SiteClosureAuthorized", "Site", sites.ClosureAuthorized.Id.ToString(),
            users.Admin, SeedDate.AddDays(-15),
            "Admin authorized Site closure after custody prerequisites were satisfied.");

        await EnsureAuditAsync(context, "SiteClosed", "Site", sites.Closed.Id.ToString(),
            users.Archivist, SeedDate.AddDays(-30),
            "Archivist completed Site closure; records remain preserved and read-only.");

        await EnsureAuditAsync(context, "EmployeeDeactivated", "ApplicationUser", users.FormerEmployee.Id,
            users.Admin, SeedDate.AddDays(-35),
            "Admin deactivated a former employee while preserving historical authorship.",
            reason: users.FormerEmployee.DeactivationReason);

        await context.SaveChangesAsync();
    }

    private static async Task EnsureAuditAsync(
        ApplicationDbContext context,
        string action,
        string entityType,
        string entityId,
        ApplicationUser actor,
        DateTimeOffset occurredAtUtc,
        string summary,
        string? reason = null,
        object? changes = null)
    {
        var auditLogs = context.Set<AuditLog>();
        var exists = await auditLogs.AnyAsync(a =>
            a.CorrelationId == SeedCorrelationId &&
            a.Action == action &&
            a.EntityType == entityType &&
            a.EntityId == entityId);

        if (exists)
        {
            return;
        }

        auditLogs.Add(new AuditLog
        {
            OccurredAtUtc = occurredAtUtc,
            ActorUserId = actor.Id,
            ActorDisplayName = actor.FullName,
            Action = action,
            EntityType = entityType,
            EntityId = entityId,
            Summary = summary,
            Reason = reason,
            ChangesJson = changes is null ? null : JsonSerializer.Serialize(changes),
            CorrelationId = SeedCorrelationId
        });
    }

    private static DateTimeOffset NormalizeUtc(DateTimeOffset value) =>
        value == default ? SeedDate : value.ToUniversalTime();

    private sealed class CatalogRecordImport
    {
        public string ArtifactCatalogNumber { get; set; } = string.Empty;
        public string SubmittedBy { get; set; } = string.Empty;
        public DateTimeOffset DateSubmitted { get; set; }
        public List<CatalogNoteImport> Notes { get; set; } = [];
    }

    private sealed class CatalogNoteImport
    {
        public string Author { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTimeOffset Created { get; set; }
    }

    #endregion

    #region Roles and users

    private static async Task SeedRolesAsync(IServiceProvider svcProvider)
    {
        var roleManager = svcProvider.GetRequiredService<RoleManager<IdentityRole>>();

        foreach (var roleName in AppRoles.AllRoles)
        {
            if (await roleManager.RoleExistsAsync(roleName))
            {
                continue;
            }

            var result = await roleManager.CreateAsync(new IdentityRole(roleName));
            EnsureIdentitySucceeded(result, $"create role '{roleName}'");
        }
    }

    private static async Task<SeedUserSet> SeedUsersAsync(
        UserManager<ApplicationUser> userManager, string seedPassword)
    {
        var admin = await GetOrCreateActiveUserAsync(
            userManager, seedPassword, "Ada", "Mercer", "admin@alrecgroup.org", AppRoles.Admin);
        var siteManager = await GetOrCreateActiveUserAsync(
            userManager, seedPassword, "Marcus", "Vale", "sitemanager@alrecgroup.org", AppRoles.SiteManager);
        var archivist = await GetOrCreateActiveUserAsync(
            userManager, seedPassword, "Iris", "Chen", "archivist@alrecgroup.org", AppRoles.Archivist);
        var fieldResearcher = await GetOrCreateActiveUserAsync(
            userManager, seedPassword, "Elena", "Ruiz", "researcher@alrecgroup.org", AppRoles.FieldResearcher);
        var fieldStaff = await GetOrCreateActiveUserAsync(
            userManager, seedPassword, "Jonah", "Okafor", "fieldstaff@alrecgroup.org", AppRoles.FieldStaff);
        var offSitePersonnel = await GetOrCreateActiveUserAsync(
            userManager, seedPassword, "Priya", "Nair", "offsite@alrecgroup.org", AppRoles.OffSitePersonnel);

        var formerEmployee = await GetOrCreateUserCoreAsync(
            userManager, seedPassword, "Mara", "Ellis", "former.fieldstaff@alrecgroup.org");
        formerEmployee.IsActive = false;
        formerEmployee.AssignedSiteId = null;
        formerEmployee.DeactivatedAtUtc = SeedDate.AddDays(-35);
        formerEmployee.DeactivatedById = admin.Id;
        formerEmployee.DeactivationReason = "Employment ended; retain historical research authorship.";
        formerEmployee.LockoutEnabled = true;
        formerEmployee.LockoutEnd = DateTimeOffset.MaxValue;
        EnsureIdentitySucceeded(
            await userManager.UpdateAsync(formerEmployee),
            $"deactivate seed user '{formerEmployee.Email}'");

        var formerRoles = await userManager.GetRolesAsync(formerEmployee);
        if (formerRoles.Count > 0)
        {
            EnsureIdentitySucceeded(
                await userManager.RemoveFromRolesAsync(formerEmployee, formerRoles),
                $"remove active roles from '{formerEmployee.Email}'");
        }

        var users = new SeedUserSet(
            admin,
            siteManager,
            archivist,
            fieldResearcher,
            fieldStaff,
            offSitePersonnel,
            formerEmployee);

        foreach (var user in users.All)
        {
            await RemoveLegacySeedClaimsAsync(userManager, user);
        }

        return users;
    }

    private static async Task<ApplicationUser> GetOrCreateActiveUserAsync(
        UserManager<ApplicationUser> userManager,
        string seedPassword,
        string firstName,
        string lastName,
        string email,
        string role)
    {
        var user = await GetOrCreateUserCoreAsync(userManager, seedPassword, firstName, lastName, email);
        user.IsActive = true;
        user.DeactivatedAtUtc = null;
        user.DeactivatedById = null;
        user.DeactivationReason = null;
        user.RestoredAtUtc = null;
        user.RestoredById = null;
        user.LockoutEnabled = true;
        user.LockoutEnd = null;

        EnsureIdentitySucceeded(
            await userManager.UpdateAsync(user),
            $"update seed user '{email}'");
        await EnsureSingleRoleAsync(userManager, user, role);

        return user;
    }

    private static async Task<ApplicationUser> GetOrCreateUserCoreAsync(
        UserManager<ApplicationUser> userManager,
        string seedPassword,
        string firstName,
        string lastName,
        string email)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is not null)
        {
            user.FirstName = firstName;
            user.LastName = lastName;
            user.UserName = email;
            user.EmailConfirmed = true;
            return user;
        }

        user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true,
            FirstName = firstName,
            LastName = lastName,
            IsActive = true,
            LockoutEnabled = true
        };

        var result = await userManager.CreateAsync(user, seedPassword);
        EnsureIdentitySucceeded(result, $"create seed user '{email}'");
        return user;
    }

    private static async Task EnsureSingleRoleAsync(
        UserManager<ApplicationUser> userManager,
        ApplicationUser user,
        string role)
    {
        var currentRoles = await userManager.GetRolesAsync(user);
        var rolesToRemove = currentRoles
            .Where(currentRole => currentRole != role)
            .ToArray();

        if (rolesToRemove.Length > 0)
        {
            EnsureIdentitySucceeded(
                await userManager.RemoveFromRolesAsync(user, rolesToRemove),
                $"replace roles for '{user.Email}'");
        }

        if (!await userManager.IsInRoleAsync(user, role))
        {
            EnsureIdentitySucceeded(
                await userManager.AddToRoleAsync(user, role),
                $"assign role '{role}' to '{user.Email}'");
        }
    }

    private static async Task RemoveLegacySeedClaimsAsync(
        UserManager<ApplicationUser> userManager,
        ApplicationUser user)
    {
        string[] legacyClaimTypes =
        [
            "CanVerifyCatalogRecords",
            "CanUploadMedia",
            "CanManageUsers"
        ];

        var claims = await userManager.GetClaimsAsync(user);
        var claimsToRemove = claims
            .Where(claim => legacyClaimTypes.Contains(claim.Type))
            .ToArray();

        if (claimsToRemove.Length == 0)
        {
            return;
        }

        EnsureIdentitySucceeded(
            await userManager.RemoveClaimsAsync(user, claimsToRemove),
            $"remove obsolete claims from '{user.Email}'");
    }

    private static async Task AssignSeedUsersToSitesAsync(
        UserManager<ApplicationUser> userManager,
        SeedUserSet users,
        WorkflowSiteSet sites)
    {
        users.SiteManager.AssignedSiteId = sites.Primary.Id;
        users.FieldResearcher.AssignedSiteId = sites.Primary.Id;
        users.FieldStaff.AssignedSiteId = sites.Primary.Id;

        users.Admin.AssignedSiteId = null;
        users.Archivist.AssignedSiteId = null;
        users.OffSitePersonnel.AssignedSiteId = null;

        foreach (var user in users.Active)
        {
            EnsureIdentitySucceeded(
                await userManager.UpdateAsync(user),
                $"assign Site scope for '{user.Email}'");
        }
    }

    private static void EnsureIdentitySucceeded(IdentityResult result, string operation)
    {
        if (result.Succeeded)
        {
            return;
        }

        var errors = string.Join("; ", result.Errors.Select(error => error.Description));
        throw new InvalidOperationException($"Unable to {operation}: {errors}");
    }

    #endregion

    private static async Task ResetPostgresSequencesAsync(ApplicationDbContext context)
    {
        await context.Database.ExecuteSqlRawAsync("""
            SELECT setval(
                pg_get_serial_sequence('"Sites"', 'Id'),
                COALESCE(MAX("Id"), 1),
                MAX("Id") IS NOT NULL
            ) FROM "Sites";
            """);

        await context.Database.ExecuteSqlRawAsync("""
            SELECT setval(
                pg_get_serial_sequence('"Artifacts"', 'Id'),
                COALESCE(MAX("Id"), 1),
                MAX("Id") IS NOT NULL
            ) FROM "Artifacts";
            """);

        await context.Database.ExecuteSqlRawAsync("""
            SELECT setval(
                pg_get_serial_sequence('"CatalogRecords"', 'Id'),
                COALESCE(MAX("Id"), 1),
                MAX("Id") IS NOT NULL
            ) FROM "CatalogRecords";
            """);

        Console.WriteLine("PostgreSQL identity sequences reset successfully.");
    }

    private sealed record SeedUserSet(
        ApplicationUser Admin,
        ApplicationUser SiteManager,
        ApplicationUser Archivist,
        ApplicationUser FieldResearcher,
        ApplicationUser FieldStaff,
        ApplicationUser OffSitePersonnel,
        ApplicationUser FormerEmployee)
    {
        public IEnumerable<ApplicationUser> Active =>
        [
            Admin,
            SiteManager,
            Archivist,
            FieldResearcher,
            FieldStaff,
            OffSitePersonnel
        ];

        public IEnumerable<ApplicationUser> All => [.. Active, FormerEmployee];
    }

    private sealed record WorkflowSiteSet(
        Site Primary,
        Site Boundary,
        Site PublicationRequest,
        Site ClosureRequested,
        Site ClosureAuthorized,
        Site Closed);

    private sealed record WorkflowArtifactSet(
        Artifact PublicArtifact,
        Artifact PrivateArtifact,
        Artifact PublicationPending,
        Artifact TransferRequested,
        Artifact TransferAuthorized,
        Artifact InTransit,
        Artifact ReceivedAwaitingLocation,
        Artifact ReadyForCompletion,
        Artifact WarehousedPublic,
        Artifact BoundaryArtifact,
        Artifact ClosureRequestedArtifact,
        Artifact ClosureAuthorizedArtifact,
        Artifact ClosedSiteArtifact)
    {
        public IEnumerable<Artifact> All =>
        [
            PublicArtifact,
            PrivateArtifact,
            PublicationPending,
            TransferRequested,
            TransferAuthorized,
            InTransit,
            ReceivedAwaitingLocation,
            ReadyForCompletion,
            WarehousedPublic,
            BoundaryArtifact,
            ClosureRequestedArtifact,
            ClosureAuthorizedArtifact,
            ClosedSiteArtifact
        ];
    }
}
