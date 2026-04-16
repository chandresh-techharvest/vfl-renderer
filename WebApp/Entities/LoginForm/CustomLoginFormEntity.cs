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

namespace VFL.Renderer.Entities.LoginForm
{
    //
    // Summary:
    //     The entity for the Login form widget. Contains all of the data persited in the
    //     database.
    [SectionsOrder(new string[] { "Select pages", "Login with external providers", "Display settings" })]
    public class CustomLoginFormEntity : IHasMargins<MarginStyle>
    {
        private const string SelectPages = "Select pages";

        private const string LoginWithExternalProviders = "Login with external providers";

        //
        // Summary:
        //     Gets or sets post login action.
        [ContentSection("Select pages", 1)]
        [DisplayName("After login users will...")]
        [DataType("radioChoices")]
        public PostLoginAction PostLoginAction { get; set; }

        //
        // Summary:
        //     Gets or sets the redirect url.
        [ContentSection("Select pages", 1)]
        [DisplayName("")]
        [Content(Type = "Telerik.Sitefinity.Pages.Model.PageNode", AllowMultipleItemsSelection = false)]
        [ConditionalVisibility("{\"conditions\":[{\"fieldName\":\"PostLoginAction\",\"operator\":\"Equals\",\"value\":\"RedirectToPage\"}],\"inline\":\"true\"}")]
        public MixedContentContext PostLoginRedirectPage { get; set; }

        //
        // Summary:
        //     Gets or sets the login page.
        [ContentSection("Select pages", 1)]
        [DisplayName("Registration page")]
        [Description("This is the page where you have dropped the Registration form widget. If you leave this field empty, a link to the Registration page will not be displayed in the Login form widget.")]
        [Content(Type = "Telerik.Sitefinity.Pages.Model.PageNode", AllowMultipleItemsSelection = false)]
        public MixedContentContext RegistrationPage { get; set; }

        //
        // Summary:
        //     Gets or sets the login page.
        [ContentSection("Select pages", 1)]
        [DisplayName("Reset password page")]
        [Description("This is the page where you have dropped the Reset password widget. If you leave this field empty, a link to the Reset password page will not be displayed in the Login form widget.")]
        [Content(Type = "Telerik.Sitefinity.Pages.Model.PageNode", AllowMultipleItemsSelection = false)]
        public MixedContentContext ResetPasswordPage { get; set; }

        //
        // Summary:
        //     Gets or sets the login page.
        [ContentSection("Select pages", 1)]
        [DisplayName("Verification page")]
        [Description("This is the page where you have dropped the register widget. If you leave this field empty, a resend confirm email button will not be displayed in the Login form widget.")]
        [Content(Type = "Telerik.Sitefinity.Pages.Model.PageNode", AllowMultipleItemsSelection = false)]
        public MixedContentContext VerificationPage { get; set; }



        //
        // Summary:
        //     Gets or sets a value indicating whether the remember me checkbox is visible.
        [ContentSection("Select pages", 1)]
        [DataType("choiceYesNo")]
        [Group("Login form options")]
        [DisplayName("Show \"Remember me\" checkbox")]
        public bool RememberMe { get; set; }

        //
        // Summary:
        //     Gets or sets the external providers.
        //
        // Value:
        //     The external providers.
        [ContentSection("Login with external providers", 1)]
        [DisplayName("Allow users to log in with...")]
        [DataType("multipleChoiceChip")]
        [Choice(ServiceUrl = "/Default.GetExternalProviders()", ButtonTitle = "Add", ActionTitle = "Select external providers")]
        public IEnumerable<string> ExternalProviders { get; set; }

        //
        // Summary:
        //     Gets or sets the view name.
        [ViewSelector]
        [ContentSection("Display settings", 1)]
        [DisplayName("Login form template")]
        public string SfViewName { get; set; }

        //
        // Summary:
        //     Gets or sets the margins.
        [ContentSection("Display settings", 1)]
        [DisplayName("Margins")]
        [TableView("Login form")]
        public MarginStyle Margins { get; set; }

        //
        // Summary:
        //     Gets or sets the CSS class of the button.
        [Category("Advanced")]
        [DisplayName("CSS class")]
        public string CssClass { get; set; }

        //
        // Summary:
        //     Gets or sets the membership provider name.
        [Category("Advanced")]
        [DisplayName("Membership Provider")]
        public string MembershipProviderName { get; set; }

        //
        // Summary:
        //     Gets or sets the header.
        //
        // Value:
        //     The header.
        [DisplayName("Login form header")]
        [Category("Advanced")]
        [ContentSection("Labels and messages")]
        [DefaultValue("Login")]
        public string Header { get; set; }

        //
        // Summary:
        //     Gets or sets the email label.
        //
        // Value:
        //     The email label.
        [DisplayName("Email field label")]
        [Category("Advanced")]
        [ContentSection("Labels and messages")]
        [DefaultValue("Email / Username")]
        public string EmailLabel { get; set; }

        //
        // Summary:
        //     Gets or sets the password label.
        //
        // Value:
        //     The password label.
        [DisplayName("Password field label")]
        [Category("Advanced")]
        [ContentSection("Labels and messages")]
        [DefaultValue("Password")]
        public string PasswordLabel { get; set; }

        //
        // Summary:
        //     Gets or sets the submit button label.
        //
        // Value:
        //     The submit button label.
        [DisplayName("Login form button")]
        [Category("Advanced")]
        [ContentSection("Labels and messages")]
        [DefaultValue("Log in")]
        public string SubmitButtonLabel { get; set; }

        //
        // Summary:
        //     Gets or sets the error message.
        //
        // Value:
        //     The error message.
        [DisplayName("Incorrect credentials message")]
        [Category("Advanced")]
        [ContentSection("Labels and messages")]
        [DefaultValue("Incorrect credentials.")]
        public string ErrorMessage { get; set; }

        //
        // Summary:
        //     Gets or sets the remember me label.
        //
        // Value:
        //     The remember me label.
        [DisplayName("Remember me checkbox")]
        [Category("Advanced")]
        [ContentSection("Labels and messages")]
        [DefaultValue("Remember me")]
        public string RememberMeLabel { get; set; }

        //
        // Summary:
        //     Gets or sets the forgotten password link label.
        //
        // Value:
        //     The forgotten password link label.
        [DisplayName("Forgotten password link")]
        [Category("Advanced")]
        [ContentSection("Labels and messages")]
        [DefaultValue("Forgotten password")]
        public string ForgottenPasswordLinkLabel { get; set; }

        //
        // Summary:
        //     Gets or sets the not registered label.
        //
        // Value:
        //     The not registered label.
        [DisplayName("Not registered label")]
        [Category("Advanced")]
        [ContentSection("Labels and messages")]
        [DefaultValue("Not registered yet?")]
        public string NotRegisteredLabel { get; set; }

        //
        // Summary:
        //     Gets or sets the register link text.
        //
        // Value:
        //     The register link text.
        [DisplayName("Register link")]
        [Category("Advanced")]
        [ContentSection("Labels and messages")]
        [DefaultValue("Register now")]
        public string RegisterLinkText { get; set; }

        //
        // Summary:
        //     Gets or sets the external providers header.
        //
        // Value:
        //     The external providers header.
        [DisplayName("External providers header")]
        [Category("Advanced")]
        [ContentSection("Labels and messages")]
        [DefaultValue("or use account in...")]
        public string ExternalProvidersHeader { get; set; }

        //
        // Summary:
        //     Gets or sets the validation required message.
        //
        // Value:
        //     The validation required message.
        [DisplayName("Required fields error message")]
        [Category("Advanced")]
        [ContentSection("Labels and messages")]
        [DefaultValue("All fields are required.")]
        public string ValidationRequiredMessage { get; set; }

        //
        // Summary:
        //     Gets or sets the invalid email error message.
        //
        // Value:
        //     The invalid email error message.
        [DisplayName("Invalid email error message")]
        [Category("Advanced")]
        [ContentSection("Labels and messages")]
        [DefaultValue("Invalid email format.")]
        public string ValidationInvalidEmailMessage { get; set; }

        //
        // Summary:
        //     Gets or sets the attributes for the widget.
        [Category("Advanced")]
        [ContentSection("Attributes")]
        [DisplayName("Attributes for...")]
        [DataType("attributes")]
        [LengthDependsOn(null, "", " ", ExtraRecords = "[{\"Name\": \"LoginForm\", \"Title\": \"Login form\"}]")]
        public IDictionary<string, IList<AttributeModel>> Attributes { get; set; }
    }
}
