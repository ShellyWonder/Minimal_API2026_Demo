using System;
using System.Collections.Generic;
using System.Text;

namespace ThePlatoProject.Contracts.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ArtifactCustodyStatus
    {
        OnSite = 1,
        TransferRequested = 2,
        TransferAuthorized = 3,
        InTransit = 4,
        Warehoused = 5

    }
}
