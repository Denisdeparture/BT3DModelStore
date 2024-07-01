namespace Application.Models
{
    public class ProductViewModel
    {
        public int Quantity { get; set; }
        public double Price { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public IFormFile Model { get; set; } = null!;
    }
}
