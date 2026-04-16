using Progress.Sitefinity.AspNetCore.ViewComponents.AttributeConfigurator.Attributes;
using Progress.Sitefinity.Renderer.Designers.Attributes;
using Progress.Sitefinity.Renderer.Entities.Content;

namespace VFL.Renderer.Entities.Dashboard
{
    public class DashboardEntity
    {
        [ViewSelector]
        public string ViewName { get; set; }

        [Content(Type = KnownContentTypes.Images)]
        public MixedContentContext SliderImages { get; set; }        
    }
}
