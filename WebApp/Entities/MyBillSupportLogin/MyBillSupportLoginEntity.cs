using Progress.Sitefinity.AspNetCore.Models.Common;
using Progress.Sitefinity.AspNetCore.Models;
using Progress.Sitefinity.AspNetCore.ViewComponents.AttributeConfigurator.Attributes;
using Progress.Sitefinity.AspNetCore.Widgets.Models.Common;
using Progress.Sitefinity.Renderer.Designers.Attributes;
using Progress.Sitefinity.Renderer.Entities.Content;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace VFL.Renderer.Entities.MyBillSupportLogin
{
    /// <summary>
    /// Entity for MyBill Support Login widget.
    /// Used for customer support to access customer profiles using a single-use access token.
    /// </summary>
    [SectionsOrder(new string[] { "Select pages", "Display settings" })]
    public class MyBillSupportLoginEntity : IHasMargins<MarginStyle>
    {
        private const string SelectPages = "Select pages";

        /// <summary>
        /// Gets or sets the redirect page after successful login.
        /// </summary>
        [ContentSection(SelectPages, 1)]
        [DisplayName("After login redirect to")]
        [Description("The page to redirect to after successful support login (e.g., MyBill Dashboard)")]
        [Content(Type = "Telerik.Sitefinity.Pages.Model.PageNode", AllowMultipleItemsSelection = false)]
        public MixedContentContext PostLoginRedirectPage { get; set; }

        /// <summary>
        /// Gets or sets the view name.
        /// </summary>
        [ViewSelector]
        [ContentSection("Display settings", 1)]
        [DisplayName("Support login template")]
        public string SfViewName { get; set; }

        /// <summary>
        /// Gets or sets the margins.
        /// </summary>
        [ContentSection("Display settings", 1)]
        [DisplayName("Margins")]
        [TableView("MyBill Support Login")]
        public MarginStyle Margins { get; set; }

        /// <summary>
        /// Gets or sets the CSS class.
        /// </summary>
        [Category("Advanced")]
        [DisplayName("CSS class")]
        public string CssClass { get; set; }

        /// <summary>
        /// Gets or sets the header.
        /// </summary>
        [DisplayName("Page header")]
        [Category("Advanced")]
        [ContentSection("Labels and messages")]
        [DefaultValue("Support Access")]
        public string Header { get; set; }

        /// <summary>
        /// Gets or sets the loading message.
        /// </summary>
        [DisplayName("Loading message")]
        [Category("Advanced")]
        [ContentSection("Labels and messages")]
        [DefaultValue("Authenticating, please wait...")]
        public string LoadingMessage { get; set; }

        /// <summary>
        /// Gets or sets the success message.
        /// </summary>
        [DisplayName("Success message")]
        [Category("Advanced")]
        [ContentSection("Labels and messages")]
        [DefaultValue("Login successful! Redirecting to dashboard...")]
        public string SuccessMessage { get; set; }

        /// <summary>
        /// Gets or sets the error message for invalid/expired token.
        /// </summary>
        [DisplayName("Invalid token error message")]
        [Category("Advanced")]
        [ContentSection("Labels and messages")]
        [DefaultValue("Invalid or expired access token. Please request a new token.")]
        public string InvalidTokenMessage { get; set; }

        /// <summary>
        /// Gets or sets the error message for missing token.
        /// </summary>
        [DisplayName("Missing token error message")]
        [Category("Advanced")]
        [ContentSection("Labels and messages")]
        [DefaultValue("No access token provided. Please use a valid support access link.")]
        public string MissingTokenMessage { get; set; }

        /// <summary>
        /// Gets or sets the attributes for the widget.
        /// </summary>
        [Category("Advanced")]
        [ContentSection("Attributes")]
        [DisplayName("Attributes for...")]
        [DataType("attributes")]
        [LengthDependsOn(null, "", " ", ExtraRecords = "[{\"Name\": \"MyBillSupportLogin\", \"Title\": \"MyBill Support Login\"}]")]
        public IDictionary<string, IList<AttributeModel>> Attributes { get; set; }
    }
}
