using System.Collections.Generic;
using System.Threading.Tasks;
using VFL.Renderer.Entities.PricePackage;
using VFL.Renderer.ViewModels.PricePackage;


namespace VFL.Renderer.Models.PricePackage
{
    public interface IPricePackageModel
    {
        /// <summary>
        /// Gets the view models.
        /// </summary>
        /// <returns>The generated view models.</returns>
        Task<IList<PricePackageViewModel>> GetViewModels(PricePackageEntity entity);
    }
}
