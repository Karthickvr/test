using System.ComponentModel.DataAnnotations;

namespace XOProject.Api.Model
{
    public class TradeModel
    {
        [Required]
        public string Symbol { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "NoOfShares Should be greater then '0'")]
        public int NoOfShares { get; set; }

        [Required]
        public int PortfolioId { get; set; }

        //BUG: The team found trade records in the database with an unexpected value in the action column
        [Required]
        [EnumDataType(typeof(Action))]
        public string Action { get; set; }
    }

    public enum Action
    {
        BUY,
        SELL
    }
}