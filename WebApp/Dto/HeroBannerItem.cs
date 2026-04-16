using Progress.Sitefinity.RestSdk.Dto;

namespace VFL.Renderer.Dto
{
    public class HeroBannerSlideItem : SdkItem
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public Image[] SlideImage { get; set; }
        public string ButtonText1 { get; set; }
        public string ButtonLink1 { get; set; }
        public string ButtonText2 { get; set; }
        public string ButtonLink2 { get; set; }
        public string VideoLink { get; set; }
        public string SubTitle { get; set; }
        public Image[] SlideMobileImage { get; set; }
        public string IsTextLight { get; set; }
    }
}
