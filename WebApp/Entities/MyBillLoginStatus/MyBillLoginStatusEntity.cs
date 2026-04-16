using Progress.Sitefinity.Renderer.Designers.Attributes;
using Progress.Sitefinity.Renderer.Entities.Content;

namespace VFL.Renderer.Entities.MyBillLoginStatus
{
    /// <summary>
    /// Entity for MyBill Login Status widget
    /// Configures navigation links for authenticated MyBill users
    /// </summary>
    public class MyBillLoginStatusEntity
    {
        [Content(Type = KnownContentTypes.Pages, AllowMultipleItemsSelection = false, DisableInteraction = true, ShowSiteSelector = true)]
        public MixedContentContext LoginPage { get; set; }

        [Content(Type = KnownContentTypes.Pages, AllowMultipleItemsSelection = false, DisableInteraction = true, ShowSiteSelector = true)]
        public MixedContentContext ProfilePage { get; set; }

        [Content(Type = KnownContentTypes.Pages, AllowMultipleItemsSelection = false, DisableInteraction = true, ShowSiteSelector = true)]
        public MixedContentContext MyBillDashboard { get; set; }

        [Content(Type = KnownContentTypes.Pages, AllowMultipleItemsSelection = false, DisableInteraction = true, ShowSiteSelector = true)]
        public MixedContentContext TransactionHistory { get; set; }

        [Content(Type = KnownContentTypes.Pages, AllowMultipleItemsSelection = false, DisableInteraction = true, ShowSiteSelector = true)]
        public MixedContentContext LogoutRedirectPage { get; set; }
    }
}
