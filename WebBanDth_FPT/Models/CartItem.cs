using System.Linq;

namespace WebBanDth_FPT.Models
{
    public class CartItem
    {
        WebBanDthEntities database = new WebBanDthEntities();

        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ImageUrl { get; set; }
        public double UnitPrice { get; set; }
        public int Quantity { get; set; }

        // Thành tiền = Số lượng * Đơn giá
        public double GetTotalPrice() 
        { 
            return Quantity * UnitPrice; 
        }

        public CartItem(int productId)
        {
            ProductId = productId;
            Product product = database.Products.Single(n => n.ProductId == productId);
            
            ProductName = product.ProductName;
            ImageUrl = product.ImageUrl;
            UnitPrice = double.Parse(product.Price.ToString());
            Quantity = 1; 
        }
    }
}