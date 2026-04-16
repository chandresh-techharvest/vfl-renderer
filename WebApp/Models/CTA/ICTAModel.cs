using System.Threading.Tasks;
using VFL.Renderer.Entities.CTA;
using VFL.Renderer.ViewModels.CTA;


namespace VFL.Renderer.Models.CTA
{
    public interface ICTAModel
    {
        Task<CTAViewModel> GetViewModels(CTAEntity entity);
    }
}
