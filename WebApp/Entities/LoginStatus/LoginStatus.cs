using Progress.Sitefinity.AspNetCore.ViewComponents.AttributeConfigurator.Attributes;
using Progress.Sitefinity.Renderer.Designers.Attributes;
using Progress.Sitefinity.Renderer.Entities.Content;

namespace Vfl.Renderer.Entities.LoginStatus
{
    public class LoginStatusEntity
    {
        [Content(Type = KnownContentTypes.Pages, AllowMultipleItemsSelection = false, DisableInteraction = true, ShowSiteSelector = true)]
        public MixedContentContext LoginPage { get; set; }

        [Content(Type = KnownContentTypes.Pages, AllowMultipleItemsSelection = false, DisableInteraction = true, ShowSiteSelector = true)]
        public MixedContentContext RegistrationPage { get; set; }

        [Content(Type = KnownContentTypes.Pages, AllowMultipleItemsSelection = false, DisableInteraction = true, ShowSiteSelector = true)]
        public MixedContentContext ProfilePage { get; set; }

        [Content(Type = KnownContentTypes.Pages, AllowMultipleItemsSelection = false, DisableInteraction = true, ShowSiteSelector = true)]
        public MixedContentContext Support { get; set; }
        [Content(Type = KnownContentTypes.Pages, AllowMultipleItemsSelection = false, DisableInteraction = true, ShowSiteSelector = true)]
        public MixedContentContext UserDashboard { get; set; }

        [ViewSelector]
        public string ViewName { get; set; }

    }
}
