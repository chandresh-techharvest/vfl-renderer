using Progress.Sitefinity.AspNetCore.ViewComponents.AttributeConfigurator.Attributes;
using Progress.Sitefinity.Renderer.Designers.Attributes;
using Progress.Sitefinity.Renderer.Entities.Content;
using System;
using System.ComponentModel;

namespace VFL.Renderer.Entities.Checkout
{
    public class CheckoutEntity
    {
        [ViewSelector]
        public string ViewName { get; set; }
        public string Id { get; set; } = Guid.NewGuid().ToString("N");
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string PlanCode { get; set; }  // final selected plancode
        public string PlanCodesJson { get; set; }  //store full array

        public decimal Amount { get; set; }
        public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;

        public string PageUrl { get; set; }

        /// <summary>
        /// eg: WebTopUp, PurchasePlan
        /// </summary>
        public string PageType { get; set; }

        [ContentSection(SelectPages, 1)]
        [DisplayName("Customer Support page")]
        [Description("This is the page for Customer Support Page.")]
        [Content(Type = KnownContentTypes.Pages, AllowMultipleItemsSelection = false)]
        public MixedContentContext CustomerSuportPage { get; set; }

        [ContentSection(SelectPages, 1)]
        [DisplayName("Home page")]
        [Description("This is the page for Home Page.")]
        [Content(Type = KnownContentTypes.Pages, AllowMultipleItemsSelection = false)]
        public MixedContentContext HomePage { get; set; }

        [ContentSection(SelectPages, 1)]
        [DisplayName("Terms & Conditions page")]
        [Description("This is the page for terms and conditions.")]
        [Content(Type = KnownContentTypes.Pages, AllowMultipleItemsSelection = false)]
        public MixedContentContext TermsConditionsPage { get; set; }

        private const string SelectPages = "Select pages";
    }
}
