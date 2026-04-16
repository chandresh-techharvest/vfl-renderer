using Progress.Sitefinity.Renderer.Designers.Attributes;
using Progress.Sitefinity.Renderer.Entities.Content;
using Progress.Sitefinity.Renderer.Models;
using Progress.Sitefinity.RestSdk.Clients.Pages.Dto;
using Progress.Sitefinity.RestSdk.Dto;
using System.Collections.Generic;
using System.ComponentModel;

namespace VFL.Renderer.ViewModels.CTA
{
    public class CTAViewModel
    {
        public List<ImageDto> CTAImage1 { get; set; }
        public List<ImageDto> CTAImage2 { get; set; }
        public string TagTitle { get; set; }
        public string TagTitleIconClass { get; set; }
        public string MainTitle { get; set; }
        public string Summary { get; set; }      
        public List<ImageDto> PriceIconImage { get; set; }

        public string PriceTitle { get; set; }
        public string CTAText { get; set; }
        public LinkModel CTALink { get; set; }
    }
}
