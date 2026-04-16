using Progress.Sitefinity.AspNetCore.Configuration;
using Progress.Sitefinity.AspNetCore.Models;
using Progress.Sitefinity.AspNetCore.Widgets.Models.ResetPassword;
using System.Collections.Generic;

namespace VFL.Renderer.ViewModels.MyBillResetPassword
{
    public class CustomMyBillResetPasswordViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomMyBillResetPasswordViewModel"/> class.
        /// </summary>
        public CustomMyBillResetPasswordViewModel()
        {
            this.Labels = new ResetPasswordLabelsViewModel();
        }

        /// <summary>
        /// Gets or sets the reset user password handler path.
        /// </summary>
        public string ResetUserPasswordHandlerPath { get; set; }

        /// <summary>
        /// Gets or sets the change password handler path.
        /// </summary>
        public string SendResetPasswordEmailHandlerPath { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether there is an error for the view model.
        /// </summary>
        public bool Error { get; set; }

        /// <summary>
        /// Gets or sets the login page link.
        /// </summary>
        public string LoginPageLink { get; set; }

        /// <summary>
        /// Gets or sets the labels.
        /// </summary>
        public ResetPasswordLabelsViewModel Labels { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the widget is in reset password mode.
        /// </summary>
        public bool IsResetPasswordRequest { get; set; }

        /// <summary>
        /// Gets or sets the reset password url.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1056:URI-like properties should not be strings", Justification = "Reviewed.")]
        public string ResetPasswordUrl { get; set; }

        /// <summary>
        /// Gets or sets the html form class attribute values.
        /// </summary>
        public string CssClass { get; set; }

        /// <summary>
        /// Gets or sets the invalid class.
        /// </summary>
        public string InvalidClass { get; set; }

        /// <summary>
        /// Gets or sets the warning.
        /// </summary>
        public string Warning { get; set; }

        /// <summary>
        /// Gets or sets the attributes for the widget.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "Must be able to set in property editor.")]
        public IDictionary<string, IList<AttributeModel>> Attributes { get; set; }

        /// <summary>
        /// Gets or sets the attributes for the columns and for the section.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "Must be able to set in the model.")]
        public IDictionary<VisibilityStyle, string> VisibilityClasses { get; set; }

        /// <summary>
        /// Gets or sets the BAN (Business Account Number).
        /// </summary>
        public string BAN { get; set; }

        /// <summary>
        /// Gets or sets the token.
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Gets or sets the verification page URL.
        /// </summary>
        public string VerificationPageUrl { get; set; }
    }
}
