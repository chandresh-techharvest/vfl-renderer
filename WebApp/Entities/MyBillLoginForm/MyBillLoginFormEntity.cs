using Progress.Sitefinity.AspNetCore.Models.Common;
using Progress.Sitefinity.AspNetCore.Models;
using Progress.Sitefinity.AspNetCore.ViewComponents.AttributeConfigurator.Attributes;
using Progress.Sitefinity.AspNetCore.Widgets.Models.Common;
using Progress.Sitefinity.AspNetCore.Widgets.Models.LoginForm;
using Progress.Sitefinity.Renderer.Designers.Attributes;
using Progress.Sitefinity.Renderer.Entities.Content;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace VFL.Renderer.Entities.MyBillLoginForm
{
    /// <summary>
    /// Entity for MyBill Login form widget. Contains all data persisted in the database.
    /// Separate from MyVodafone login form.
    /// </summary>
    [SectionsOrder(new string[] { "Select pages", "Display settings" })]
    public class MyBillLoginFormEntity : IHasMargins<MarginStyle>
    {
        private const string SelectPages = "Select pages";

        /// <summary>
        /// Gets or sets post login action.
        /// </summary>
        [ContentSection(SelectPages, 1)]
        [DisplayName("After login users will...")]
        [DataType("radioChoices")]
        public PostLoginAction PostLoginAction { get; set; }

        /// <summary>
        /// Gets or sets the redirect page after successful login.
        /// </summary>
        [ContentSection(SelectPages, 1)]
        [DisplayName("")]
        [Content(Type = "Telerik.Sitefinity.Pages.Model.PageNode", AllowMultipleItemsSelection = false)]
        [ConditionalVisibility("{\"conditions\":[{\"fieldName\":\"PostLoginAction\",\"operator\":\"Equals\",\"value\":\"RedirectToPage\"}],\"inline\":\"true\"}")]
        public MixedContentContext PostLoginRedirectPage { get; set; }

        /// <summary>
        /// Gets or sets the MyBill reset password page.
        /// </summary>
        [ContentSection(SelectPages, 1)]
        [DisplayName("Reset password page")]
        [Description("This is the page where you have dropped the MyBill Reset password widget. If you leave this field empty, a link to the Reset password page will not be displayed.")]
        [Content(Type = "Telerik.Sitefinity.Pages.Model.PageNode", AllowMultipleItemsSelection = false)]
        public MixedContentContext ResetPasswordPage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the remember me checkbox is visible.
        /// </summary>
        [ContentSection(SelectPages, 1)]
        [DataType("choiceYesNo")]
        [Group("Login form options")]
        [DisplayName("Show \"Remember me\" checkbox")]
        public bool RememberMe { get; set; }

        /// <summary>
        /// Gets or sets the view name.
        /// </summary>
        [ViewSelector]
        [ContentSection("Display settings", 1)]
        [DisplayName("Login form template")]
        public string SfViewName { get; set; }

        /// <summary>
        /// Gets or sets the margins.
        /// </summary>
        [ContentSection("Display settings", 1)]
        [DisplayName("Margins")]
        [TableView("MyBill Login form")]
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
        [DisplayName("Login form header")]
        [Category("Advanced")]
        [ContentSection("Labels and messages")]
        [DefaultValue("MyBill Login")]
        public string Header { get; set; }

        /// <summary>
        /// Gets or sets the BAN field label.
        /// </summary>
        [DisplayName("BAN field label")]
        [Category("Advanced")]
        [ContentSection("Labels and messages")]
        [DefaultValue("BAN (Billing Account Number)")]
        public string BanLabel { get; set; }

        /// <summary>
        /// Gets or sets the password label.
        /// </summary>
        [DisplayName("Password field label")]
        [Category("Advanced")]
        [ContentSection("Labels and messages")]
        [DefaultValue("Password")]
        public string PasswordLabel { get; set; }

        /// <summary>
        /// Gets or sets the submit button label.
        /// </summary>
        [DisplayName("Login button label")]
        [Category("Advanced")]
        [ContentSection("Labels and messages")]
        [DefaultValue("Log in to MyBill")]
        public string SubmitButtonLabel { get; set; }

        /// <summary>
        /// Gets or sets the error message for incorrect credentials.
        /// </summary>
        [DisplayName("Incorrect credentials message")]
        [Category("Advanced")]
        [ContentSection("Labels and messages")]
        [DefaultValue("Incorrect BAN or password.")]
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets the remember me label.
        /// </summary>
        [DisplayName("Remember me checkbox label")]
        [Category("Advanced")]
        [ContentSection("Labels and messages")]
        [DefaultValue("Remember me")]
        public string RememberMeLabel { get; set; }

        /// <summary>
        /// Gets or sets the forgotten password link label.
        /// </summary>
        [DisplayName("Forgotten password link")]
        [Category("Advanced")]
        [ContentSection("Labels and messages")]
        [DefaultValue("Forgot your password?")]
        public string ForgottenPasswordLinkLabel { get; set; }

        /// <summary>
        /// Gets or sets the validation required message.
        /// </summary>
        [DisplayName("Required fields error message")]
        [Category("Advanced")]
        [ContentSection("Labels and messages")]
        [DefaultValue("BAN and password are required.")]
        public string ValidationRequiredMessage { get; set; }

        /// <summary>
        /// Gets or sets the invalid BAN error message.
        /// </summary>
        [DisplayName("Invalid BAN error message")]
        [Category("Advanced")]
        [ContentSection("Labels and messages")]
        [DefaultValue("Invalid BAN format.")]
        public string ValidationInvalidBanMessage { get; set; }

        /// <summary>
        /// Gets or sets the attributes for the widget.
        /// </summary>
        [Category("Advanced")]
        [ContentSection("Attributes")]
        [DisplayName("Attributes for...")]
        [DataType("attributes")]
        [LengthDependsOn(null, "", " ", ExtraRecords = "[{\"Name\": \"MyBillLoginForm\", \"Title\": \"MyBill Login form\"}]")]
        public IDictionary<string, IList<AttributeModel>> Attributes { get; set; }
    }
}
