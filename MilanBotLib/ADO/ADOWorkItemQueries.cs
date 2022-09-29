using Microsoft.TeamFoundation.Work.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MilanBotLib.ADO
{
    public class ADOWorkItemQueries
    {
        private readonly ADOConfiguration _config;
        private readonly VssConnection _connection;
        public ADOWorkItemQueries(ADOConfiguration configuration)
        {
            _config = configuration;
            _connection = new VssConnection(new Uri(configuration.CollectionUri), new VssBasicCredential(string.Empty, configuration.AccessToken));
        }

        public async Task<IEnumerable<ADOWorkItem>> GetWorkItemsInSprint()
        {
            var client = _connection.GetClient<WorkItemTrackingHttpClient>();
            var currentIterationPath = await _getCurrentIterationPath();
            Wiql queryWiql = new Wiql()
            {
                Query = string.Format(@"
                    SELECT
                        [System.WorkItemType],
                        [System.Title],
                        [Microsoft.VSTS.Scheduling.Effort],
                        [One_custom.CustomField4]
                    FROM workitems
                    WHERE
                        [System.TeamProject] = '{0}'
                        AND [System.AssignedTo] = @me
                        AND [System.IterationPath] = '{1}'
                    ", _config.ProjectName, currentIterationPath).CollapseWhitespace()
            };
            var workItemMetadata = await client.QueryByWiqlAsync(queryWiql);
            var resIds = workItemMetadata.WorkItems.ToList().Select(itm => itm.Id);
            var resCols = workItemMetadata.Columns.ToList().Select(col => col.ReferenceName);
            var workItems = await client.GetWorkItemsAsync(resIds, resCols);
            List<ADOWorkItem> workItemsInSprint = new List<ADOWorkItem>();
            foreach (var wi in workItems)
            {
                if (wi.Id == null)
                {
                    continue;
                }
                int id = wi.Id ?? -1; // Will never be -1
                string title = (string?)wi.Fields["System.Title"] ?? "";
                string gitBranchName = (string)wi.Fields.GetValueOrDefault("One_custom.CustomField4", "");
                double hoursSpent = Math.Round(Convert.ToDouble(wi.Fields.GetValueOrDefault("Microsoft.VSTS.Scheduling.Effort", 0)), 2);
                ADOWorkItemType wiType;
                switch ((string)wi.Fields["System.WorkItemType"])
                {
                    case "Bug":
                        wiType = ADOWorkItemType.BUG;
                        break;
                    case "Task":
                        wiType = ADOWorkItemType.TASK;
                        break;
                    case "Product Backlog Item":
                        wiType = ADOWorkItemType.PRODUCT_BACKLOG_ITEM;
                        break;
                    default:
                        continue;
                }
                var newWorkItem = new ADOWorkItem(_connection, id, title, gitBranchName, hoursSpent, wiType);
                workItemsInSprint.Add(newWorkItem);
            }
            return workItemsInSprint;
        }

        /// <summary>
        /// Gets the current iteration path.
        /// </summary>
        /// <returns>The current iteration path</returns>
        public async Task<string?> _getCurrentIterationPath()
        {
            var workClient = _connection.GetClient<WorkHttpClient>();
            var iterationList = await workClient.GetTeamIterationsAsync(new Microsoft.TeamFoundation.Core.WebApi.Types.TeamContext(_config.ProjectName));
            var firstItm = iterationList.Find(itm =>
                DateTime.Now >= itm.Attributes.StartDate && DateTime.Now <= itm.Attributes.FinishDate
                && itm.Path.Contains(@"\2Wk\"));
            return firstItm?.Path;
        }

        /// <summary>
        /// Gets the current iteration path.
        /// </summary>
        /// <returns>The current iteration path</returns>
        /// <exception cref="InvalidTeamSettingsIterationException">If a single iteration path is not returned</exception>
        //public async Task<string?> GetCurrentIterationPath()
        //{
        //    var workClient = _connection.GetClient<WorkHttpClient>();
        //    var iterationListWithOneItem = await workClient.GetTeamIterationsAsync(new Microsoft.TeamFoundation.Core.WebApi.Types.TeamContext(_config.ProjectName), "Current");
        //    if (iterationListWithOneItem?.Count == 1)
        //    {
        //        return iterationListWithOneItem[0].Path;
        //    }
        //    else
        //    {
        //        throw new InvalidTeamSettingsIterationException("Single team iteration not found.");
        //    }
        //}
    }
}
