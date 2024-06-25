using SV20T1020460.DomainModels;

namespace SV20T1020460.Web.Models
{
    public class OrderSearchResult : BasePaginationResult
    {
        public int Status { get; set; } = 0;
        public string TimeRange { get; set; } = "";
        public List<Order> Data { get; set; } = new List<Order>();
    }

}
