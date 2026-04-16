using System.Threading.Tasks;
using VFL.Renderer.Entities.MyBillLoginStatus;
using VFL.Renderer.ViewModels.MyBillLoginStatus;

namespace VFL.Renderer.Models.MyBillLoginStatus
{
    /// <summary>
    /// Interface for MyBill Login Status model
    /// </summary>
    public interface IMyBillLoginStatusModel
    {
        Task<MyBillLoginStatusViewModel> GetViewModels(MyBillLoginStatusEntity entity);
    }
}
