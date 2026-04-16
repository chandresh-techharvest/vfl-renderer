using Progress.Sitefinity.Renderer.Designers.Attributes;
using System.ComponentModel;

namespace VFL.Renderer.Models.Profile
{
    /// <summary>
    /// The actions that can be performed in both read and edit mode post update/after saving.
    /// </summary>
    public enum ReadEditModeAction
    {
        /// <summary>
        /// Displays a message.
        /// </summary>
        [EnumDisplayName("View a message")]
        [Description("View a message")]
        ViewMessage = 0,

        /// <summary>
        /// Switch to Read Mode
        /// </summary>
        [EnumDisplayName("Switch to Read mode")]
        [Description("Switch to Read mode")]
        SwitchToReadMode = 1,

        /// <summary>
        /// Redirects to specific Sitefinity page.
        /// </summary>
        [EnumDisplayName("Redirect to page...")]
        [Description("Redirect to page...")]
        RedirectToPage = 2,
    }
}
