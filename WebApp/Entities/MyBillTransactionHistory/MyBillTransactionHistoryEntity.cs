using Progress.Sitefinity.AspNetCore.ViewComponents.AttributeConfigurator.Attributes;
using Progress.Sitefinity.Renderer.Designers.Attributes;
using System.ComponentModel;

namespace VFL.Renderer.Entities.MyBillTransactionHistory
{
    /// <summary>
    /// Entity for MyBill Transaction History widget configuration in Sitefinity CMS
    /// </summary>
    public class MyBillTransactionHistoryEntity
    {
        /// <summary>
        /// Allows selection of different transaction history views in Sitefinity Designer
        /// </summary>
        [ViewSelector]
        public string ViewName { get; set; }

        /// <summary>
        /// Number of records to display per page
        /// </summary>
        [DisplayName("Page Size")]
        [Description("Number of transactions to display per page (default: 20)")]
        [DefaultValue(20)]
        public int PageSize { get; set; } = 20;

        /// <summary>
        /// Widget title displayed in the card header
        /// </summary>
        [DisplayName("Widget Title")]
        [Description("Title displayed in the transaction history card header")]
        [DefaultValue("Payment Transaction History")]
        public string WidgetTitle { get; set; } = "Payment Transaction History";

        /// <summary>
        /// Show/hide the search filter
        /// </summary>
        [DisplayName("Show Search")]
        [Description("Show the search filter input")]
        [DefaultValue(true)]
        public bool ShowSearch { get; set; } = true;

        /// <summary>
        /// Show/hide the status filter dropdown
        /// </summary>
        [DisplayName("Show Status Filter")]
        [Description("Show the status filter dropdown")]
        [DefaultValue(true)]
        public bool ShowStatusFilter { get; set; } = true;
    }
}
