using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using Witsml.Data.MudLog;

using WitsmlExplorer.Api.Jobs;
using WitsmlExplorer.Api.Models;
using WitsmlExplorer.Api.Query;
using WitsmlExplorer.Api.Services;

namespace WitsmlExplorer.Api.Workers.Delete
{
    public class DeleteMudLogsWorker : BaseWorker<DeleteMudLogsJob>, IWorker
    {
        private readonly IDeleteUtils _deleteUtils;
        public JobType JobType => JobType.DeleteMudLogs;

        public DeleteMudLogsWorker(ILogger<DeleteMudLogsJob> logger, IWitsmlClientProvider witsmlClientProvider, IDeleteUtils deleteUtils) : base(witsmlClientProvider, logger)
        {
            _deleteUtils = deleteUtils;
        }

        public override async Task<(WorkerResult, RefreshAction)> Execute(DeleteMudLogsJob job)
        {
            job.ToDelete.Verify();
            IEnumerable<WitsmlMudLog> queries = ObjectQueries.DeleteObjectsQuery<WitsmlMudLog>(job.ToDelete.WellUid, job.ToDelete.WellboreUid, job.ToDelete.ObjectUids);
            RefreshObjects refreshAction = new(GetTargetWitsmlClientOrThrow().GetServerHostname(), job.ToDelete.WellUid, job.ToDelete.WellboreUid, EntityType.MudLog);
            return await _deleteUtils.DeleteObjectsOnWellbore(GetTargetWitsmlClientOrThrow(), queries, refreshAction);
        }

    }
}