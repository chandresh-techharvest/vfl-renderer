using Progress.Sitefinity.AspNetCore.ViewComponents.AttributeConfigurator.Attributes;

namespace VFL.Renderer.Entities.TransactionHistory
{
    public class TransactionHistoryEntity
    {
        [ViewSelector]
        public string ViewName { get; set; }
    }
}
