using LagoVista.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.Interfaces
{
    public interface IStorageIdProvider
    {
        NormalizedId32 Id { get; }
    }

    public interface IOwnedStorageRecord
    {
        EntityHeader OwnerOrganization { get; }
    }

    public interface ISummaryTableBuilder<TSummary>
    {
        TSummary CreateSummary();
    }

}
