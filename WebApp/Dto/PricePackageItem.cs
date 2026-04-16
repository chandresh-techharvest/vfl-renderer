using Progress.Sitefinity.RestSdk.Dto;

namespace VFL.Renderer.Dto
{
    public class PricePackageItem : SdkItem
    {
        public string Title { get; set; }
        public string Category { get; set; }
        public string IconClass { get; set; }
        public string Features { get; set; }
        public string Price { get; set; }
        public string PackageLink { get; set; }
    }
}
