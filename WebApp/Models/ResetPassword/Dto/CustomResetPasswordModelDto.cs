namespace VFL.Renderer.Models.ResetPassword.Dto
{
    internal class CustomResetPasswordModelDto
    {
        /// <summary>
        /// Gets or sets the new password.
        /// </summary>
        public string NewPassword { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the membership provider settings require question and answer for reset/retrieval password functionality.
        /// </summary>
        /// <value>
        /// <c>true</c> if the membership provider requires question and answer; otherwise, <c>false</c>.
        /// </value>
        public bool RequiresQuestionAndAnswer { get; set; }

        /// <summary>
        /// Gets or sets the answer.
        /// </summary>
        public string Answer { get; set; }

        /// <summary>
        /// Gets or sets the security question.
        /// </summary>
        public string SecurityQuestion { get; set; }

        /// <summary>
        /// Gets or sets the security token.
        /// </summary>
        public string SecuirtyToken { get; set; }
    }
}
