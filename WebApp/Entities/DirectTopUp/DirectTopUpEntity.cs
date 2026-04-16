using Progress.Sitefinity.AspNetCore.ViewComponents.AttributeConfigurator.Attributes;
using Progress.Sitefinity.Renderer.Designers.Attributes;
using Progress.Sitefinity.Renderer.Entities.Content;
using System.ComponentModel;

namespace VFL.Renderer.Entities.DirectTopUp
{
    public class DirectTopUpEntity
    {
        [ViewSelector]
        public string ViewName { get; set; }

        [ContentSection(SelectPages, 1)]
        [DisplayName("Customer Support page")]
        [Description("This is the page for Customer Support.")]
        [Content(Type = KnownContentTypes.Pages, AllowMultipleItemsSelection = false, DisableInteraction = true, ShowSiteSelector = true)]
        public MixedContentContext CustomerSupport { get; set; }

        [ContentSection(SelectPages, 1)]
        [DisplayName("Terms & Conditions page")]
        [Description("This is the page for terms and conditions.")]
        [Content(Type = KnownContentTypes.Pages, AllowMultipleItemsSelection = false)]
        public MixedContentContext TermsConditionsPage { get; set; }        

        private const string SelectPages = "Select pages";
    }
}
