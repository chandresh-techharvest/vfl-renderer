namespace VFL.Renderer.Services.MyBillProfile.Models
{
    /// <summary>
    /// GraphQL response model for allAccountsByPrimary query
    /// </summary>
    public class GraphQLAccountsResponse
    {
        public GraphQLAccountsData Data { get; set; }
    }

    public class GraphQLAccountsData
    {
        public PrimaryAccountInfo[] AllAccountsByPrimary { get; set; }
    }

    /// <summary>
    /// Primary account information from GraphQL API
    /// Contains primary account details and list of business account numbers
    /// </summary>
    public class PrimaryAccountInfo
    {
        /// <summary>
        /// Primary account name (company name)
        /// </summary>
        public string PrimaryAccountName { get; set; }

        /// <summary>
        /// Contact person full name
        /// </summary>
        public string ContactFullName { get; set; }

        /// <summary>
        /// Contact phone number
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Contact email address
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// List of business account numbers (BANs) under this primary account
        /// </summary>
        public BusinessAccountNumber[] BusinessAccountNumbers { get; set; }
    }

    /// <summary>
    /// Business account number (BAN) information
    /// </summary>
    public class BusinessAccountNumber
    {
        /// <summary>
        /// Account name/label for this BAN
        /// </summary>
        public string AccountName { get; set; }

        /// <summary>
        /// BAN account number (e.g., "911499841")
        /// </summary>
        public string Number { get; set; }

        /// <summary>
        /// List of paperless email addresses for this BAN
        /// </summary>
        public PaperlessEmail[] PaperlessEmails { get; set; }
    }

    /// <summary>
    /// Paperless email information
    /// </summary>
    public class PaperlessEmail
    {
        /// <summary>
        /// Email address
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Unique identifier for this paperless email
        /// </summary>
        public int Id { get; set; }
    }
}
