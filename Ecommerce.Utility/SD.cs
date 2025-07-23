namespace Ecommerce.Utility
{
    public static class SD
    {
        public const string CardNumber = "NumberOfCard";
        public const string ProductFolderPath = @"/images/product/";
        public const string CustomerRole = "Customer";
        public const string AdminRole = "Admin";
        public const string AllowExtension = "jpg,png,jpeg";
        public const int MaxSizeByByte = 2 * 1024 * 1024;
        public const string AdminEmail = "abdelrahman.zagloul.dev@gmail.com";
       
        //// if local
        //public const string Domain = @"https://localhost:7256/";
        
         //if Global
        public const string Domain = @"https://etechno.runasp.net/";
        
        public const string SuccessUrl = $"{Domain}Customer/Payment/Success?";
        public const string CancelUrl = $"{Domain}Customer/Payment/Cancel";
        public const string returnUrl = $"/Admin/Order/Details/";


    }
}
