using AutoMapper;
using BuisnesLogic.Model.Roles;
using BuisnesLogic.ServicesInterface;
using Contracts;
using DataBase.AppDbContexts;
using DomainModel;
using Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
namespace WebServer.Controllers
{
    public class AdminOperationController : ControllerBase
    {
        private readonly IStrategyValidation _strategy;
        private readonly IHostEnvironment _application;
        private readonly IConfiguration _configuration;
        private readonly ILogger<Program> _logger;
        private readonly IProductOperation _productOperations;
        private readonly IUsersRolesOperation _userOperations;
        public AdminOperationController(MainDbContext database,UserManager<User> userManager, IRoleOperation roleOperations, IUsersRolesOperation usersRolesOperation ,IProductOperation productOperation, RoleManager<IdentityRole> roleManager,  IConfiguration configuration, IStrategyValidation cond, ILogger<Program> logger, IHostEnvironment application, IServiceProvider provider)
        {
            _strategy = cond ?? throw new ArgumentNullException(nameof(cond));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _application = application ?? throw new ArgumentNullException(nameof(application));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _productOperations = productOperation ?? throw new ArgumentNullException(nameof(productOperation));
            _userOperations = usersRolesOperation ?? throw new ArgumentNullException(nameof(usersRolesOperation));
        }
        [HttpPost("/AdminOperation/AddProduct")]
        public IActionResult AddProduct([FromForm] ProductContractModel productModel)
        {
            _logger.LogInformation("BeginAdd");
            var productViewIsValid = _strategy.StrategyCondition(new List<Predicate<ProductContractModel>>()
            {
                (pw) => pw != null,
                (pw) => pw.GetType().GetProperties().Where(pr => pr.GetValue(pw) != null).ToArray().Length == pw.GetType().GetProperties().Length,
                (pw) =>
                {
                    bool redflag = true;
                    foreach(var prop in pw.GetType().GetProperties())
                    {
                        if(!prop.PropertyType.Equals(typeof(String))) continue;
                        if(string.IsNullOrEmpty(prop.GetValue(pw) as string)) redflag = false;
                    }
                    return redflag;
                }
            }, productModel);
            if (!productViewIsValid)
            {
                if (_application.IsDevelopment()) _logger.LogError("Productview is not valid: " + productViewIsValid.NumberConditionCounter);
                return BadRequest();
            }
            var product = Mapping(productModel);
            var ProductsInDBisValid = _strategy.StrategyCondition(new List<Predicate<IEnumerable<Product>>>()
            {
                (products) => !products.Equals(null),
            }, _productOperations.GetAllProducts());
            if (!ProductsInDBisValid)
            {
                if (_application.IsDevelopment()) _logger.LogError("Product is not valid");
                return NoContent();
            }
            var isExist = (ProductsInDBisValid.Object!.Where(prod =>
            prod.Name == product.Name &
            prod.Description == product.Description
            ).SingleOrDefault()) is not null;
            if (isExist) return BadRequest("this object exist");
            try
            {
                var task = _productOperations.CreateProductAsync(product, productModel.Model.OpenReadStream(), productModel.Model.FileName);
                task.Wait();
            }
            catch (NullReferenceException ex)
            {
                _logger.LogError("3Dmodel was null validation isn`t worked " + this.GetType().ToString());
                return Conflict("3Dmodel was null");
            }
            return Redirect(_configuration["ClientHostForRedirect"]!);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateProduct([FromForm] ProductContractModel productViewModel)
        {
            var productViewIsValid = _strategy.StrategyCondition(new List<Predicate<ProductContractModel>>()
            {
                (pw) => pw != null,
                (pw) => pw.GetType().GetProperties().Where(pr => pr.GetValue(pw) != null).ToArray().Length == pw.GetType().GetProperties().Length,
                (pw) => pw.GetType().GetProperties().Where(pr => pr.PropertyType.Equals(typeof(String))).Where(pr => string.IsNullOrEmpty((pr.GetValue(pw) as string))).ToArray().Length == pw.GetType().GetProperties().Where(pr => pr.PropertyType.Equals(typeof(String))).ToArray().Length
            }, productViewModel);
            if (!productViewIsValid) return BadRequest();
            var product = Mapping(productViewModel);
            var productInBase = (_productOperations.GetAllProducts().Where(prod =>
        prod.Name == product.Name &
        prod.Description == product.Description
        ).SingleOrDefault());
            if (productInBase == null) return NoContent();
            try
            {
                await _productOperations.UpdateProductAsync(productInBase, productViewModel.Model.OpenReadStream(), productViewModel.Model.FileName);
            }
            catch (NullReferenceException ex)
            {
                return Conflict("3Dmodel was null");
            }
            return Ok();
        }
        [HttpPost]
        public async Task<IActionResult> DeleteProduct(DeleteProductContracts deleteProductViewModel)
        {
            var deleteViewIsValid = _strategy.StrategyCondition(new List<Predicate<DeleteProductContracts>>()
            {
                (pw) => string.IsNullOrEmpty(pw.Url),
                (pw) => string.IsNullOrEmpty(pw.NameOfFile)
            }, deleteProductViewModel);
            if (deleteViewIsValid) return BadRequest();
            var res = await _productOperations.DeleteProductAsync(deleteViewIsValid.Object!.Url, deleteViewIsValid.Object!.NameOfFile);
            if (!res) return BadRequest();
            return Ok();
        }
        [HttpPost("/AdminOperation/ChangeUserRole")]
        public async Task<IResult> ChangeUserRole([FromForm]ChangeRoleContract roleContract)
        {

            var IsValid = _strategy.StrategyCondition(new List<Predicate<ChangeRoleContract>>()
            {
                (pw) => !string.IsNullOrEmpty(pw.Email),
                (pw) => !string.IsNullOrEmpty(pw.Role),
                (pw) => roleContract.Role!.ToLower().Equals(RolesType.User.ToString().ToLower()) | roleContract.Role.ToLower().Equals(RolesType.Admin.ToString().ToLower())
            }, roleContract);
            if (!IsValid) return Results.BadRequest();
            var res = await _userOperations.AddRoleFromUserAsync(roleContract.Email!, roleContract.Role!);
            return res ? Results.Ok() : Results.NotFound();
        }
        private Product Mapping(ProductContractModel productViewModel)
        {
            var mapper = new Mapper(new MapperConfiguration(cfg => cfg.CreateMap<ProductContractModel, Product>()
                .ForMember("Name", opt => opt.MapFrom(o => o.Name))
                .ForMember("Price", opt => opt.MapFrom(o => o.Price))
                .ForMember("Quantity", opt => opt.MapFrom(o => o.Quantity))
                .ForMember("Description", opt => opt.MapFrom(o => o.Description))));
            var product = mapper.Map<Product>(productViewModel);
            return product;
        }
    }
    
}

