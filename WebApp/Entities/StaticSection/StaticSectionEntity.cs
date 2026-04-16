using Progress.Sitefinity.AspNetCore.ViewComponents.AttributeConfigurator.Attributes;
using Progress.Sitefinity.Renderer.Designers.Attributes;
using Progress.Sitefinity.Renderer.Designers;
using System.Collections.Generic;
using System.ComponentModel;
using Progress.Sitefinity.AspNetCore.Widgets.Models.Section;
using System.ComponentModel.DataAnnotations;

namespace VFL.Renderer.Entities.StaticSection
{
    public class StaticSectionEntity
    {
        [ViewSelector("StaticSection")]
        public string ViewType { get; set; }

        /// <summary>
        /// Gets or sets the columns background.
        /// </summary>
        [ContentSection("Column style", 2)]
        [DisplayName("Background")]        
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "Must be able to set in property editor.")]
        public IDictionary<string, SimpleBackgroundStyle> ColumnsBackground { get; set; }

        /// <summary>
        /// Gets or sets the custom CSS for the columns and for the section.
        /// </summary>
        [Category(PropertyCategory.Advanced)]
        [ContentSection("Custom CSS classes")]
        [DisplayName("Custom CSS class for...")]        
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "Must be able to set in property editor.")]
        public string CustomCssClass { get; set; }
    }
}
