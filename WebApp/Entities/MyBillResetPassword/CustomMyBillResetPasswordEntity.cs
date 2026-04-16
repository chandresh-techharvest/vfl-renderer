using Progress.Sitefinity.AspNetCore;
using Progress.Sitefinity.AspNetCore.Models.Common;
using Progress.Sitefinity.AspNetCore.Models;
using Progress.Sitefinity.AspNetCore.ViewComponents.AttributeConfigurator.Attributes;
using Progress.Sitefinity.AspNetCore.Widgets.Models.Common;
using Progress.Sitefinity.Renderer.Designers;
using Progress.Sitefinity.Renderer.Designers.Attributes;
using Progress.Sitefinity.Renderer.Entities.Content;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace VFL.Renderer.Entities.MyBillResetPassword
{ 
    /// <summary>
    /// The entity for the MyBill Reset password widget. Contains all of the data persited in the database.
    /// </summary>
    [SectionsOrder(SelectPages, Constants.ContentSectionTitles.DisplaySettings)]
    public class CustomMyBillResetPasswordEntity : IHasMargins<MarginStyle>
    {
        /// <summary>
        /// Gets or sets the login page.
        /// </summary>
        [ContentSection(SelectPages, 1)]
        [DisplayName("MyBill Login page")]
        [Description("This is the page where you have dropped MyBill login form widget.")]
        [Content(Type = KnownContentTypes.Pages, AllowMultipleItemsSelection = false)]
        public MixedContentContext LoginPage { get; set; }

        /// <summary>
        /// Gets or sets the view name.
        /// </summary>
        [ViewSelector]
        [ContentSection(Constants.ContentSectionTitles.DisplaySettings, 1)]
        [DisplayName("Reset password template")]
        public string SfViewName { get; set; }

        /// <summary>
        /// Gets or sets the margins.
        /// </summary>
        [ContentSection(Constants.ContentSectionTitles.DisplaySettings, 1)]
        [DisplayName("Margins")]
        [TableView("Reset password")]
        public MarginStyle Margins { get; set; }

        /// <summary>
        /// Gets or sets the CSS class of the button.
        /// </summary>
        [Category(PropertyCategory.Advanced)]
        [DisplayName("CSS class")]
        public string CssClass { get; set; }

        /// <summary>
        /// Gets or sets the reset password header.
        /// </summary>
        [DisplayName("Reset password header")]
        [ContentSection(Constants.ContentSectionTitles.LabelsAndMessages)]
        [Category(PropertyCategory.Advanced)]
        [DefaultValue("Reset password")]
        public string ResetPasswordHeader { get; set; }

        /// <summary>
        /// Gets or sets the new password label.
        /// </summary>
        [DisplayName("Password field label")]
        [ContentSection(Constants.ContentSectionTitles.LabelsAndMessages)]
        [Category(PropertyCategory.Advanced)]
        [DefaultValue("New password")]
        public string NewPasswordLabel { get; set; }

        /// <summary>
        /// Gets or sets the repeat password field label.
        /// </summary>
        [DisplayName("Repeat password field label")]
        [ContentSection(Constants.ContentSectionTitles.LabelsAndMessages)]
        [Category(PropertyCategory.Advanced)]
        [DefaultValue("Repeat new password")]
        public string RepeatNewPasswordLabel { get; set; }

        /// <summary>
        /// Gets or sets the save button label.
        /// </summary>
        [DisplayName("Save button")]
        [ContentSection(Constants.ContentSectionTitles.LabelsAndMessages)]
        [Category(PropertyCategory.Advanced)]
        [DefaultValue("Save")]
        public string SaveButtonLabel { get; set; }

        /// <summary>
        /// Gets or sets the success message.
        /// </summary>
        [DisplayName("Success message")]
        [ContentSection(Constants.ContentSectionTitles.LabelsAndMessages)]
        [Category(PropertyCategory.Advanced)]
        [DefaultValue("Your password is successfully changed.")]
        public string SuccessMessage { get; set; }

        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        [DisplayName("Error message")]
        [ContentSection(Constants.ContentSectionTitles.LabelsAndMessages)]
        [Category(PropertyCategory.Advanced)]
        [DefaultValue("You are unable to reset password. Contact your administrator for assistance.")]
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets the back link label.
        /// </summary>
        [DisplayName("Back link")]
        [ContentSection(Constants.ContentSectionTitles.LabelsAndMessages)]
        [Category(PropertyCategory.Advanced)]
        [DefaultValue("Back to login")]
        public string BackLinkLabel { get; set; }

        /// <summary>
        /// Gets or sets the required fields error message.
        /// </summary>
        [DisplayName("Required fields error message.")]
        [ContentSection(Constants.ContentSectionTitles.LabelsAndMessages)]
        [Category(PropertyCategory.Advanced)]
        [DefaultValue("All fields are required.")]
        public string AllFieldsAreRequiredErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets the passwords mismatch error message.
        /// </summary>
        [DisplayName("Passwords mismatch error message.")]
        [ContentSection(Constants.ContentSectionTitles.LabelsAndMessages)]
        [Category(PropertyCategory.Advanced)]
        [DefaultValue("New password and repeat password don't match.")]
        public string PasswordsMismatchErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets the forgotten password header.
        /// </summary>
        [DisplayName("Forgotten password header")]
        [ContentSection(Constants.ContentSectionTitles.LabelsAndMessages)]
        [Category(PropertyCategory.Advanced)]
        [DefaultValue("Forgot your password?")]
        public string ForgottenPasswordHeader { get; set; }

        /// <summary>
        /// Gets or sets the Forgotten password label.
        /// </summary>
        [DisplayName("Forgotten password label")]
        [ContentSection(Constants.ContentSectionTitles.LabelsAndMessages)]
        [Category(PropertyCategory.Advanced)]
        [DefaultValue("Enter your BAN (Business Account Number) and you will receive an email with a link to reset your password.")]
        public string ForgottenPasswordLabel { get; set; }

        /// <summary>
        /// Gets or sets the BAN label.
        /// </summary>
        [DisplayName("BAN field label")]
        [ContentSection(Constants.ContentSectionTitles.LabelsAndMessages)]
        [Category(PropertyCategory.Advanced)]
        [DefaultValue("BAN (Business Account Number)")]
        public string BanLabel { get; set; }

        /// <summary>
        /// Gets or sets the send button label.
        /// </summary>
        [DisplayName("Send button")]
        [ContentSection(Constants.ContentSectionTitles.LabelsAndMessages)]
        [Category(PropertyCategory.Advanced)]
        [DefaultValue("Send")]
        public string SendButtonLabel { get; set; }

        /// <summary>
        /// Gets or sets the Forgotten password submit message.
        /// </summary>
        [DisplayName("Forgotten password submit message")]
        [ContentSection(Constants.ContentSectionTitles.LabelsAndMessages)]
        [Category(PropertyCategory.Advanced)]
        [DefaultValue("You sent a request to reset your password for BAN: {0}")]
        public string ForgottenPasswordSubmitMessage { get; set; }

        /// <summary>
        /// Gets or sets the Forgotten password link message.
        /// </summary>
        [DisplayName("Forgotten password link message")]
        [ContentSection(Constants.ContentSectionTitles.LabelsAndMessages)]
        [Category(PropertyCategory.Advanced)]
        [DefaultValue("Use the link provided in your email to reset the password for your account.")]
        public string ForgottenPasswordLinkMessage { get; set; }

        /// <summary>
        /// Gets or sets the invalid BAN format message.
        /// </summary>
        [DisplayName("Invalid BAN format message")]
        [ContentSection(Constants.ContentSectionTitles.LabelsAndMessages)]
        [Category(PropertyCategory.Advanced)]
        [DefaultValue("Invalid BAN format.")]
        public string InvalidBanFormatMessage { get; set; }

        /// <summary>
        /// Gets or sets the field is required message.
        /// </summary>
        [DisplayName("Field is required message")]
        [ContentSection(Constants.ContentSectionTitles.LabelsAndMessages)]
        [Category(PropertyCategory.Advanced)]
        [DefaultValue("Field is required.")]
        public string FieldIsRequiredMessage { get; set; }

        /// <summary>
        /// Gets or sets the attributes for the widget.
        /// </summary>
        [Category(PropertyCategory.Advanced)]
        [ContentSection(Constants.ContentSectionTitles.Attributes)]
        [DisplayName("Attributes for...")]
        [DataType(customDataType: KnownFieldTypes.Attributes)]
        [LengthDependsOn(null, "", " ", ExtraRecords = "[{\"Name\": \"MyBillResetPassword\", \"Title\": \"MyBill Reset password\"}]")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "Must be able to set in property editor.")]
        public IDictionary<string, IList<AttributeModel>> Attributes { get; set; }

        private const string SelectPages = "Select pages";
    }
}
