using System.Collections.Generic;
using Progress.Sitefinity.AspNetCore.Models;
using Progress.Sitefinity.AspNetCore.Configuration;

namespace VFL.Renderer.ViewModels.MyBillSupportLogin
{
    /// <summary>
    /// View model for MyBill Support Login widget
    /// </summary>
    public class MyBillSupportLoginViewModel
    {
        /// <summary>
        /// Gets or sets the login handler path
        /// </summary>
        public string LoginHandlerPath { get; set; } = "/api/MyBillLoginForm/LoginUsingSingleAccessToken";

        /// <summary>
        /// Gets or sets the redirect URL after successful login
        /// </summary>
        public string RedirectUrl { get; set; }

        /// <summary>
        /// Gets or sets the CSS class
        /// </summary>
        public string CssClass { get; set; }

        /// <summary>
        /// Gets or sets the visibility classes dictionary
        /// </summary>
        public IDictionary<VisibilityStyle, string> VisibilityClasses { get; set; }

        /// <summary>
        /// Gets or sets the labels
        /// </summary>
        public MyBillSupportLoginLabels Labels { get; set; } = new MyBillSupportLoginLabels();

        /// <summary>
        /// Gets or sets the attributes
        /// </summary>
        public IDictionary<string, IList<AttributeModel>> Attributes { get; set; }

        /// <summary>
        /// Gets or sets any error message
        /// </summary>
        public string ErrorMessage { get; set; }
    }

    /// <summary>
    /// Labels for the MyBill Support Login widget
    /// </summary>
    public class MyBillSupportLoginLabels
    {
        /// <summary>
        /// Gets or sets the header text
        /// </summary>
        public string Header { get; set; } = "Support Access";

        /// <summary>
        /// Gets or sets the loading message
        /// </summary>
        public string LoadingMessage { get; set; } = "Authenticating, please wait...";

        /// <summary>
        /// Gets or sets the success message
        /// </summary>
        public string SuccessMessage { get; set; } = "Login successful! Redirecting to dashboard...";

        /// <summary>
        /// Gets or sets the invalid token message
        /// </summary>
        public string InvalidTokenMessage { get; set; } = "Invalid or expired access token. Please request a new token.";

        /// <summary>
        /// Gets or sets the missing token message
        /// </summary>
        public string MissingTokenMessage { get; set; } = "No access token provided. Please use a valid support access link.";
    }
}
