using System.Collections.Generic;
using System.Threading.Tasks;
using VFL.Renderer.Entities.VitiProduct;
using VFL.Renderer.ViewModels.VitiProduct;


namespace VFL.Renderer.Models.VitiProduct
{
    public interface IVitiProductModel
    {
        /// <summary>
        /// Gets the view models.
        /// </summary>
        /// <returns>The generated view models.</returns>
        Task<IList<ProductViewModel>> GetViewModels(VitiProductEntity entity);
    }
}
