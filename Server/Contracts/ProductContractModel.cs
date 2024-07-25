using Microsoft.AspNetCore.Http;
namespace Contracts
{
    /// <summary>
    /// This class reduced to the desired model
    /// </summary>
    /// <typeparam name="T"> what model should I lead to</typeparam>
    public class ProductContractModel
    {
        public int Quantity { get; set; }
        public double Price { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public IFormFile Model { get; set; } = null!;
    }
}
