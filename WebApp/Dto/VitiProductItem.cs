using Progress.Sitefinity.RestSdk.Dto;

namespace VFL.Renderer.Dto
{
    public class VitiProductItem : SdkItem
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public Image[] ProductImage { get; set; }
        public string Brand { get; set; }
        public string Price { get; set; }
        public string BuyNowLink { get; set; }
    }
}
