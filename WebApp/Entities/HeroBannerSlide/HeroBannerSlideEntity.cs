using Progress.Sitefinity.AspNetCore.ViewComponents.AttributeConfigurator.Attributes;
using Progress.Sitefinity.Renderer.Designers.Attributes;
using Progress.Sitefinity.Renderer.Entities.Content;

namespace VFL.Renderer.Entities.HeroBannerSlide
{
    public class HeroBannerSlideEntity
    {
        [Content(Type = "Telerik.Sitefinity.DynamicTypes.Model.HeroBannerSlides.HeroBannerSlide")]
        public MixedContentContext HeroBannerSlides { get; set; }

        [ViewSelector]
        public string ViewName { get; set; }
    }
}
