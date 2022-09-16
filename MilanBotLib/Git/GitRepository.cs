using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MilanBotLib.Git
{
    public class GitRepository
    {
        private Repository _repo;

        /// <summary>
        /// Creates a new Git repository representation.
        /// </summary>
        /// <param name="gitConfiguration">The configuration to use</param>
        public GitRepository(GitConfiguration gitConfiguration)
        {
            if (string.IsNullOrWhiteSpace(gitConfiguration?.RepositoryPath))
            {
                throw new ArgumentException("RepositoryPath must be specified");
            }

            _repo = new Repository(gitConfiguration.RepositoryPath);
        }

        /// <summary>
        /// Gets the name of the currently checked out branch.
        /// </summary>
        /// <returns>The name of the current branch, or null if in detached state</returns>
        public string? GetCurrentBranchName()
        {
            return _repo.Head?.FriendlyName;
        }

        public IEnumerable<string> GetAllBranchNames()
        {
            return _repo.Branches.Select(branch => branch.FriendlyName);
        }
    }
}
