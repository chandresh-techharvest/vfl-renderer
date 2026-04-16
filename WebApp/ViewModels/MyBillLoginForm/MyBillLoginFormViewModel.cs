using Progress.Sitefinity.AspNetCore.Models;
using Progress.Sitefinity.AspNetCore.Configuration;
using System.Collections.Generic;

namespace VFL.Renderer.ViewModels.MyBillLoginForm
{
    /// <summary>
    /// View model for MyBill Login form widget
    /// Separate from MyVodafone login form
    /// </summary>
    public class MyBillLoginFormViewModel
    {
        /// <summary>
        /// Gets or sets the login handler path.
        /// </summary>
        public string LoginHandlerPath { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the remember me checkbox is visible.
        /// </summary>
        public bool RememberMe { get; set; }

        /// <summary>
        /// Gets or sets the redirect URL after successful login.
        /// </summary>
        public string RedirectUrl { get; set; }

        /// <summary>
        /// Gets or sets the forgotten password link URL.
        /// </summary>
        public string ForgottenPasswordLink { get; set; }

        /// <summary>
        /// Gets or sets the CSS class.
        /// </summary>
        public string CssClass { get; set; }

        /// <summary>
        /// Gets or sets the visibility classes dictionary.
        /// </summary>
        public IDictionary<VisibilityStyle, string> VisibilityClasses { get; set; }

        /// <summary>
        /// Gets or sets the invalid input CSS class.
        /// </summary>
        public string InvalidClass { get; set; }

        /// <summary>
        /// Gets or sets the attributes for the widget.
        /// </summary>
        public IDictionary<string, IList<AttributeModel>> Attributes { get; set; }

        /// <summary>
        /// Gets or sets the labels for the login form.
        /// </summary>
        public MyBillLoginFormLabels Labels { get; set; } = new MyBillLoginFormLabels();

        /// <summary>
        /// Gets or sets the error message to display.
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets the redirect URL when user is already authenticated.
        /// If set, the login page should redirect to this URL instead of showing the form.
        /// </summary>
        public string AlreadyAuthenticatedRedirectUrl { get; set; }
    }

    /// <summary>
    /// Labels for MyBill Login form
    /// </summary>
    public class MyBillLoginFormLabels
    {
        /// <summary>
        /// Gets or sets the header text.
        /// </summary>
        public string Header { get; set; }

        /// <summary>
        /// Gets or sets the BAN field label.
        /// </summary>
        public string BanLabel { get; set; }

        /// <summary>
        /// Gets or sets the password field label.
        /// </summary>
        public string PasswordLabel { get; set; }

        /// <summary>
        /// Gets or sets the submit button label.
        /// </summary>
        public string SubmitButtonLabel { get; set; }

        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets the remember me label.
        /// </summary>
        public string RememberMeLabel { get; set; }

        /// <summary>
        /// Gets or sets the forgotten password link label.
        /// </summary>
        public string ForgottenPasswordLinkLabel { get; set; }

        /// <summary>
        /// Gets or sets the validation required message.
        /// </summary>
        public string ValidationRequiredMessage { get; set; }

        /// <summary>
        /// Gets or sets the invalid BAN message.
        /// </summary>
        public string ValidationInvalidBanMessage { get; set; }
    }
}
