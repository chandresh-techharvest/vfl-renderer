using Progress.Sitefinity.AspNetCore.ViewComponents.AttributeConfigurator.Attributes;
using Progress.Sitefinity.Renderer.Designers.Attributes;
using Progress.Sitefinity.Renderer.Entities.Content;
using System.ComponentModel;

namespace VFL.Renderer.Entities.MyBillDashboard
{
    /// <summary>
    /// Entity for MyBill Dashboard widget configuration in Sitefinity CMS
    /// </summary>
    public class MyBillDashboardEntity
    {
        /// <summary>
        /// Allows selection of different dashboard views in Sitefinity Designer
        /// </summary>
        [ViewSelector]
        public string ViewName { get; set; }

        /// <summary>
        /// Slider images from Sitefinity media library for dashboard carousel
        /// </summary>
        [Content(Type = KnownContentTypes.Images)]
        [DisplayName("Slider Images")]
        [Description("Images for dashboard carousel")]
        public MixedContentContext SliderImages { get; set; }

        /// <summary>
        /// Checkout page for bill payment
        /// </summary>
        [ContentSection("Navigation", 2)]
        [Content(Type = KnownContentTypes.Pages, AllowMultipleItemsSelection = false)]
        [DisplayName("Checkout Page")]
        [Description("Page where users complete bill payment")]
        public MixedContentContext CheckoutPage { get; set; }
    }
}
