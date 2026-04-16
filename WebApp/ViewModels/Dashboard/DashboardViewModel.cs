using Progress.Sitefinity.Renderer.Designers.Attributes;
using Progress.Sitefinity.Renderer.Entities.Content;
using System.Collections.Generic;
using VFL.Renderer.Services.Profile.Models;
using VFL.Renderer.ViewModels.Profile;

namespace VFL.Renderer.ViewModels.Dashboard
{
    public class DashboardViewModel
    {
        public List<string> SliderImages { get; set; }
        public Device[] devices { get; set; }
    }

   

}
