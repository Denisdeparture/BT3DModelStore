namespace Application.Models
{
    public class PayModel
    {
        public string notification_type { get; set; } = null!;
        public int withdraw_amount { get; set; }
        public DateTime datetime { get; set; }

    }
}
