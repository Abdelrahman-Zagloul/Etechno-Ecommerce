namespace Ecommerce.Utility
{
    public class PaymentStatus
    {
        public const string Pending = "Pending";    // لم يتم الدفع بعد
        public const string Approved = "Approved";  // تم الدفع بنجاح
        public const string Rejected = "Rejected";  // فشل الدفع
        public const string Refunded = "Refunded";  // تم استرجاع المبلغ
    }
}
