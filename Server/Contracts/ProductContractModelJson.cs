using AutoMapper;
using DomainModel.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public class ProductContractModelJson
    {
        public int Quantity { get; set; }
        public double Price { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public byte[]? FileInBytes { get; set; }
        public string? FileName { get; set; }
        public static implicit operator Product(ProductContractModelJson model)
        {
            var mapper = new Mapper(new MapperConfiguration(cfg => cfg.CreateMap<ProductContractModelJson, Product>()
                 .ForMember("Name", opt => opt.MapFrom(o => o.Name))
                 .ForMember("Price", opt => opt.MapFrom(src => src.Price))
                 .ForMember("Quantity", opt => opt.MapFrom(src => src.Quantity))
                 .ForMember("Description", opt => opt.MapFrom(src => src.Description))));
            var usermap = mapper.Map<Product>(model);
            return usermap;
        }
    }
}
