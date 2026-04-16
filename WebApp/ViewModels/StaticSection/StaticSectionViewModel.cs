using Progress.Sitefinity.AspNetCore.ViewComponents;

namespace Vfl.Renderer.ViewModels.StaticSection
{
    public class StaticSectionViewModel
    {
        public ICompositeViewComponentContext Context { get; set; }

        /// <summary>
        /// Gets or sets the classes applied for the section element(row).
        /// </summary>
        public string CustomClasses { get; set; }
    }
}
