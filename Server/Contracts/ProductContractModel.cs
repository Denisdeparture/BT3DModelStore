using AutoMapper;
using DomainModel;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Contracts
{
    /// <summary>
    /// This class reduced to the desired model
    /// </summary>
    /// <typeparam name="T"> what model should I lead to</typeparam>
    public class ProductContractModel
    {
        public int id { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public FormFile File { get; set; } = null!;
       
    }
}
