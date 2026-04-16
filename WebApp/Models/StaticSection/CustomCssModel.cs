using Progress.Sitefinity.Renderer.Designers.Attributes;
using System.ComponentModel;

namespace Vfl.Renderer.Models.StaticSection
{
    /// <summary>
    /// Defines custom CSS model.
    /// </summary>
    public class CustomCssModel
    {
        /// <summary>
        /// Gets or sets the CSS class.
        /// </summary>
        [DisplayName("CLASS")]
        [Placeholder("type CSS class...")]
        public string Class { get; set; }
    }
}
