using Progress.Sitefinity.AspNetCore.ViewComponents.AttributeConfigurator.Attributes;
using Progress.Sitefinity.Renderer.Designers.Attributes;
using Progress.Sitefinity.Renderer.Entities.Content;

namespace VFL.Renderer.Entities.VitiProduct
{
    public class VitiProductEntity
    {
        [Content(Type = "Telerik.Sitefinity.DynamicTypes.Model.VitiProducts.VitiProduct")]
        public MixedContentContext VitiProducts { get; set; }
         
        [ViewSelector]
        public string ViewName { get; set; }
    }
}
