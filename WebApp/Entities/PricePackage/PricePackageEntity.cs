using Progress.Sitefinity.AspNetCore.ViewComponents.AttributeConfigurator.Attributes;
using Progress.Sitefinity.Renderer.Designers.Attributes;
using Progress.Sitefinity.Renderer.Entities.Content;

namespace VFL.Renderer.Entities.PricePackage
{
    public class PricePackageEntity
    { 
        [Content(Type = "Telerik.Sitefinity.DynamicTypes.Model.PricePackages.PricePackage")]
        public MixedContentContext PricePackages { get; set; }

        [ViewSelector]
        public string ViewName { get; set; }
    }
}
