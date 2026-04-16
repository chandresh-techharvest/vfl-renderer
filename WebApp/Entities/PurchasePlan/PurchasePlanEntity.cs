using Progress.Sitefinity.AspNetCore.ViewComponents.AttributeConfigurator.Attributes;
using Progress.Sitefinity.Renderer.Designers.Attributes;
using Progress.Sitefinity.Renderer.Entities.Content;
using System.ComponentModel;

namespace VFL.Renderer.Entities.PurchasePlan
{
    public class PurchasePlanEntity
    {
        [Content(Type = "Telerik.Sitefinity.DynamicTypes.Model.VitiProducts.VitiProduct")]
        public MixedContentContext VitiProducts { get; set; }

        [Content(Type = KnownContentTypes.Pages, AllowMultipleItemsSelection = false, DisableInteraction = true, ShowSiteSelector = true)]
        public MixedContentContext CheckoutPage { get; set; }



        [ViewSelector]
        public string ViewName { get; set; }

        [ContentSection(SelectPages, 1)]
        [DisplayName("Customer Support page")]
        [Description("This is the page for Customer Support Page.")]
        [Content(Type = KnownContentTypes.Pages, AllowMultipleItemsSelection = false)]
        public MixedContentContext CustomerSuportPage { get; set; }

        private const string SelectPages = "Select pages";
    }
}
