using Progress.Sitefinity.Renderer.Designers.Attributes;
using System.ComponentModel;

namespace VFL.Renderer.Models.Registration
{
    /// <summary>
    /// Choices for post registration action.
    /// </summary>
    public enum PostRegistrationAction
    {
        /// <summary>
        /// Displays a message.
        /// </summary>
        [EnumDisplayName("View a message")]
        [Description("View a message")]
        ViewMessage = 0,

        /// <summary>
        /// Redirects to specific Sitefinity page.
        /// </summary>
        [EnumDisplayName("Redirect to page...")]
        [Description("Redirect to page...")]
        RedirectToPage = 1,
    }
}
