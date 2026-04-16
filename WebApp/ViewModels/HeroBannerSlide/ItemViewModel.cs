using VFL.Renderer.Dto;

namespace VFL.Renderer.ViewModels.HeroBannerSlide
{
    public class ItemViewModel
    {
         public string Title { get; set; }
        public string Description { get; set; }
        public string SlideImage { get; set; }
        public string SlideMobileImage { get; set; }
        public string ButtonText1 { get; set; }
        public string ButtonLink1 { get; set; }
        public string ButtonText2 { get; set; }
        public string ButtonLink2 { get; set; }
        public string VideoLink { get; set; }
        public string SubTitle { get; set; }
        public bool IsTextLight { get; set; }
    }
}
