using System;
using System.Collections.Generic;
using System.Text;

namespace MilanBotLib.ADO
{
    public class ADOConfiguration
    {
        /// <summary>
        /// The Collection URI. Generally speaking, this is "XXX.visualstudio.com"
        /// </summary>
        public string CollectionUri { get; set; } = string.Empty;
        /// <summary>
        /// The Project Name. Generally speaking, this is the second item in the path displayed at
        /// the top of the ADO page.
        /// </summary>
        public string ProjectName { get; set; } = string.Empty;
        /// <summary>
        /// The VSO Access Token to use. This needs read+write permissions on Work Items.
        /// </summary>
        public string AccessToken { get; set; } = string.Empty;
    }
}
