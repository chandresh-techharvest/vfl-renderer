using Progress.Sitefinity.AspNetCore.ViewComponents.AttributeConfigurator.Attributes;
using Progress.Sitefinity.Renderer.Designers.Attributes;

namespace VFL.Renderer.Entities.MyBillProfileSettings
{
    /// <summary>
    /// Entity for MyBill Profile Settings widget configuration in Sitefinity CMS
    /// </summary>
    public class MyBillProfileSettingsEntity
    {
        /// <summary>
        /// Allows selection of different profile settings views in Sitefinity Designer
        /// </summary>
        [ViewSelector]
        public string ViewName { get; set; }
    }
}
