namespace VFL.Renderer.Models.MyBillCheckout
{
    /// <summary>
    /// Interface for storing and retrieving MyBill checkout sessions
    /// </summary>
    public interface ICheckoutSessionStore
    {
        /// <summary>
        /// Save a checkout session
        /// </summary>
        void Save(CheckoutSession session);

        /// <summary>
        /// Retrieve a checkout session by ID
        /// </summary>
        CheckoutSession Get(string id);

        /// <summary>
        /// Remove a checkout session by ID
        /// </summary>
        void Remove(string id);
    }
}
