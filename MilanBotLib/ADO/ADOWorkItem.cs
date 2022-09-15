using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Patch;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.XPath;

namespace MilanBotLib.ADO
{
    public class ADOWorkItem
    {
        /// <summary>
        /// The work item ID.
        /// </summary>
        public int ID { get; private set; }
        /// <summary>
        /// The title of the work item as it appears in ADO.
        /// </summary>
        public string Title { get; private set; }
        /// <summary>
        /// The name of the branch associated with this work item. This should be the local branch name.
        /// </summary>
        public string GitBranchName { get; private set; }
        /// <summary>
        /// The total hours spent on this work item. This should be cumulative.
        /// </summary>
        public double TotalHoursSpent { get; private set; }
        /// <summary>
        /// The type of work item in ADO. See <see cref="ADOWorkItemType"/> for the available types of work items.
        /// </summary>
        public ADOWorkItemType WorkItemType { get; private set; }

        private readonly VssConnection _connection;
        private JsonPatchDocument? _jsonPatchDocument;
        private event Action? _onCommitActions;

        public ADOWorkItem(VssConnection connection, int id, string title, string gitBranchName, double totalHoursSpent, ADOWorkItemType workItemType)
        {
            _connection = connection;
            ID = id;
            Title = title;
            GitBranchName = gitBranchName;
            TotalHoursSpent = totalHoursSpent;
            WorkItemType = workItemType;
        }

        /// <summary>
        /// Queues up a change to set the total hours spent on this work item. Note that this does NOT change the value stored in
        /// this work item until the changes are commited with <see cref="CommitChanges"/>
        /// </summary>
        /// <param name="newTotalHours">The new total amount of hours spent on this work item</param>
        public void QueueNewTotalHoursSpent(double newTotalHours)
        {
            newTotalHours = Math.Round(newTotalHours, 2);
            _jsonPatchDocument = _addToOrCreateNewPatchOperation("Microsoft.VSTS.Scheduling.Effort", $"{newTotalHours}", _jsonPatchDocument);
            _onCommitActions += () => TotalHoursSpent = newTotalHours;
        }

        /// <summary>
        /// Queues up a change to set the git branch name for this work item. Note that this does NOT change the value stored in
        /// this work item until the changes are commited with <see cref="CommitChanges"/>
        /// </summary>
        /// <param name="newGitBranchName">The new git branch name for this work item</param>
        public void QueueNewGitBranchName(string newGitBranchName)
        {
            _jsonPatchDocument = _addToOrCreateNewPatchOperation("One_custom.CustomField4", newGitBranchName, _jsonPatchDocument);
            _onCommitActions += () => GitBranchName = newGitBranchName;
        }

        /// <summary>
        /// Changes the total hours spent on this work item. This will immediately commit all changes that have been queued up as
        /// well as this change.
        /// If this throws an exception, no values will be updated, however this update will still be queued. Retrying with
        /// <see cref="CommitChanges"/> will, if successful, update the work item appropriately.
        /// </summary>
        /// <param name="newTotalHours">The new total amount of hours spent on this work item</param>
        /// <returns>A Task that will resolve if successful or throw if an error occurs while updating the work item in ADO</returns>
        public async Task UpdateTotalHoursSpent(double newHours)
        {
            QueueNewTotalHoursSpent(newHours);
            await CommitChanges();
        }

        /// <summary>
        /// Changes the git branch name on this work item. This will immediately commit all changes that have been queued up as
        /// well as this change.
        /// If this throws an exception, no values will be updated, however this update will still be queued. Retrying with
        /// <see cref="CommitChanges"/> will, if successful, update the work item appropriately.
        /// </summary>
        /// <param name="newBranchName">The new git branch name for this work item</param>
        /// <returns>A Task that will resolve if successful or throw if an error occurs while updating the work item in ADO</returns>
        public async Task UpdateGitBranchName(string newBranchName)
        {
            QueueNewGitBranchName(newBranchName);
            await CommitChanges();
        }

        /// <summary>
        /// Commits all changes that have been queued up for this work item. If no changes have been queued up, this will immediately
        /// resolve.
        /// </summary>
        /// <returns>A Task that will resolve if successful or throw if an error occurs while updating the work item in ADO</returns>
        public async Task CommitChanges()
        {
            if (_jsonPatchDocument != null)
            {
                var client = _connection.GetClient<WorkItemTrackingHttpClient>();
                await client.UpdateWorkItemAsync(_jsonPatchDocument, ID);
                if (_onCommitActions != null)
                {
                    _onCommitActions();
                    _onCommitActions = null;
                }
            }
        }

        private JsonPatchDocument _addToOrCreateNewPatchOperation(string fieldName, string newValue, JsonPatchDocument? patchDocument = null)
        {
            if (fieldName == null) // Theoretically should never fire but meh
            {
                throw new ArgumentException("fieldName cannot be null");
            }

            if (patchDocument == null)
            {
                patchDocument = new JsonPatchDocument();
            }

            var operationPath = $"/fields/{fieldName}";
            patchDocument.Remove(patchDocument.Find(val => val.Path == operationPath)); // Remove if it already exists (new value)
            patchDocument.Add(
                new JsonPatchOperation()
                {
                    Operation = Operation.Add,
                    Path = operationPath,
                    Value = newValue
                }
            );
            return patchDocument;
        }
    }

    /// <summary>
    /// The type of ADO work item.
    /// </summary>
    public enum ADOWorkItemType
    {
        BUG,
        PRODUCT_BACKLOG_ITEM,
        TASK
    }
}
