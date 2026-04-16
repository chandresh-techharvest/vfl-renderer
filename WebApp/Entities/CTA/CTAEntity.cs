using Progress.Sitefinity.AspNetCore.ViewComponents.AttributeConfigurator.Attributes;
using Progress.Sitefinity.Renderer.Designers;
using Progress.Sitefinity.Renderer.Designers.Attributes;
using Progress.Sitefinity.Renderer.Entities.Content;
using Progress.Sitefinity.Renderer.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace VFL.Renderer.Entities.CTA
{
    public class CTAEntity
    {
        [ViewSelector]
        public string ViewName { get; set; }

        [ContentSection(SelectImages, 1)]
        [DisplayName("CTA Image 1")]
        [Description("Select CTA image.")]
        [Content(Type = KnownContentTypes.Images, AllowMultipleItemsSelection = false)]
        public MixedContentContext CTAImage1 { get; set; }

        [ContentSection(SelectImages, 1)]
        [DisplayName("CTA Image 2")]
        [Description("Select CTA image.")]
        [Content(Type = KnownContentTypes.Images, AllowMultipleItemsSelection = false)]
        public MixedContentContext CTAImage2 { get; set; }
        public string TagTitle { get; set; }
        public string TagTitleIconClass { get; set; }
        [DataType(customDataType: KnownFieldTypes.Html)]
        public string MainTitle { get; set; }

        [DataType(customDataType: KnownFieldTypes.Html)]
        public string Summary { get; set; }

        [ContentSection(SelectImages, 1)]
        [DisplayName("CTA price icon image")]
        [Description("Select CTA image.")]
        [Content(Type = KnownContentTypes.Images, AllowMultipleItemsSelection = false)]
        public MixedContentContext PriceIconImage { get; set; }

        [DataType(customDataType: KnownFieldTypes.Html)]
        public string PriceTitle { get; set; }
        public string CTAText { get; set; }

        [DataType(customDataType: "linkSelector")]
        public LinkModel CTALink { get;set; }

        private const string SelectPages = "Select pages";
        private const string SelectImages = "Select images";
    }
}
