using SV20T1020460.DomainModels;

namespace SV20T1020460.Web.Models
{
    public class OrderDetailModel
    {
        private const int Quantity = 0;
        private const int SalePrice = 0;
        
        public Order Order { get; set; }
        public List<OrderDetail> Details { get; set; }
        public decimal TotalPrice
        { 
            get
            {
                return Quantity * SalePrice;
            }
        }
       
    }

}
