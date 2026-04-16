using Progress.Sitefinity.AspNetCore.ViewComponents.AttributeConfigurator.Attributes;

namespace VFL.Renderer.Entities.PageTitle
{
    public class PageTitleEntity
    {
        public string Title { get; set; }
        [ViewSelector]
        public string ViewName { get; set; }
    }
}
