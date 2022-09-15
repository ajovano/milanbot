using System;
using System.Collections.Generic;
using System.Text;

namespace MilanBotLib.Git
{
    /// <summary>
    /// The configuration for a Git repository representation.
    /// </summary>
    public class GitConfiguration
    {
        /// <summary>
        /// The path to the repository. Generally, this should be the working directory of the repository.
        /// </summary>
        public string RepositoryPath { get; set; } = string.Empty;
    }
}
