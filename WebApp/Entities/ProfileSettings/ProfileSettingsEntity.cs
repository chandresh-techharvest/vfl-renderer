using Microsoft.AspNetCore.Http;
using Progress.Sitefinity.AspNetCore.ViewComponents.AttributeConfigurator.Attributes;

namespace VFL.Renderer.Entities.ProfileSettings
{
    public class ProfileSettingsEntity
    {
        public string Title { get; set; }
        [ViewSelector]
        public string ViewName { get; set; }
    }
}
