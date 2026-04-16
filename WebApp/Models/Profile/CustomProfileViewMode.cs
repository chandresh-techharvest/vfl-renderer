using Progress.Sitefinity.Renderer.Designers.Attributes;
using System.ComponentModel;

namespace VFL.Renderer.Models.Profile
{
    public enum CustomProfileViewMode
    {
        /// <summary>
        /// Edit mode only.
        /// </summary>
        [EnumDisplayName("Edit mode only")]
        [Description("Edit mode only")]
        Edit = 0,

        /// <summary>
        /// Read mode only.
        /// </summary>
        [EnumDisplayName("Read mode only")]
        [Description("Read mode only")]
        Read = 1,

        /// <summary>
        /// Both - read and edit mode.
        /// </summary>
        [EnumDisplayName("Both - read and edit mode")]
        [Description("Both - read and edit mode")]
        ReadEdit = 2,
    }
}
