
namespace Ecommerce.Models.ViewModels
{
    public class AdminDashboardViewModel
    {
        public int TotalUser { get; set; }  
        public int TotalOrder { get; set; }  
        public int FacebookUsers {  get; set; }  
        public int GoogleUsers{  get; set; }  
        public int GithubUsers {  get; set; }  
        public int LinkedInUsers {  get; set; }
        public int MicrosoftUsers { get; set; }
        public decimal TotalSalals { get; set; }
        public int NumberOfCategory { get; set; }
        public int NumberOfProduct { get; set; }
        public int PendingPercentOrderStatus { get; set; }
        public int ApprovedPercentOrderStatus { get; set; }
        public int InProcessPercentOrderStatus { get; set; }
        public int ShippedPercentOrderStatus { get; set; }
        public int DeliveredPercentOrderStatus { get; set; }
        public int CancelledPercentOrderStatus { get; set; }
        public int PendingPercentPaymentStatus { get; set; }
        public int ApprovedPercentPaymentStatus { get; set; }
        public int RefundedPercentPaymentStatus { get; set; }
        public int RejectedPercentPaymentStatus { get; set; }

    }
}
