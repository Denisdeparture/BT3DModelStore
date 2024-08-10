using AutoMapper;
using BuisnesLogic.ConstStorage;
using BuisnesLogic.Model.Roles;
using BuisnesLogic.ServicesInterface;
using BuisnesLogic.ServicesInterface.ClientsInterfaces.MessageBrokers;
using Contracts;
using DataBase.AppDbContexts;
using DomainModel.Entity;
using Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Utilities;
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
        private readonly IProduceClient<ProductContractModelJson> _produceClient;
        public AdminOperationController(MainDbContext database,UserManager<User> userManager, IMessageBrokerClient<ProductContractModelJson> messageBroker ,IRoleOperation roleOperations, IUsersRolesOperation usersRolesOperation ,IProductOperation productOperation, RoleManager<IdentityRole> roleManager,  IConfiguration configuration, IStrategyValidation cond, ILogger<Program> logger, IHostEnvironment application, IServiceProvider provider)
        {
            _strategy = cond ?? throw new ArgumentNullException(nameof(cond));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _application = application ?? throw new ArgumentNullException(nameof(application));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _productOperations = productOperation ?? throw new ArgumentNullException(nameof(productOperation));
            _userOperations = usersRolesOperation ?? throw new ArgumentNullException(nameof(usersRolesOperation));
            _produceClient = messageBroker ?? throw new ArgumentNullException(nameof(messageBroker));
        }
        [HttpPost("/AdminOperation/AddProduct")]
        public async Task<IActionResult> AddProduct([FromForm] ProductContractModel productModel, IFormFile Model)
        {
            if (Model is null) return BadRequest();
#pragma warning disable
            productModel.File = Model as FormFile;
#pragma warning restore
            var productViewIsValid = _strategy.StrategyCondition(new List<Predicate<ProductContractModel>>()
            {
                (pw) => pw != null,
                (pw) => pw.GetType().GetProperties().Where(pr => pr.GetValue(pw) == null).FirstOrDefault() == null,
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
                _logger.LogError("Productview is not valid: " + productViewIsValid.NumberConditionCounter);
                return BadRequest();
            }
            var mapper = new Mapper(new MapperConfiguration(cfg => cfg.CreateMap<ProductContractModel, ProductContractModelJson>()
               .ForMember("Name", opt => opt.MapFrom(o => o.Name))
               .ForMember("Price", opt => opt.MapFrom(o => o.Price))
               .ForMember("Quantity", opt => opt.MapFrom(o => o.Quantity))
               .ForMember("Description", opt => opt.MapFrom(o => o.Description))
               .ForMember("FileName", opt => opt.MapFrom(o => o.File.FileName))
               ));
            var kafkaAdapter = mapper.Map<ProductContractModelJson>(productModel);
            using (BinaryReader br = new BinaryReader(productModel.File!.OpenReadStream()))
            {
                kafkaAdapter.FileInBytes = br.ReadBytes((int)productModel.File.Length);
            }
            try
            {
                var cts = new CancellationTokenSource();
                await _produceClient.Produce(kafkaAdapter, cts, KafkaTopics.AddProductTopic);
            }
            catch (Exception ex)
            {
                _logger.LogError("3Dmodel was null validation isn`t worked " + ex.Message + " " + this.GetType().ToString());
                return Conflict(ex.Message);
            }
            return Redirect(_configuration["ClientHostForRedirect"]!);
        }
        [HttpPost("/AdminOperation/UpdateProduct")]
        public async Task<IActionResult> UpdateProduct([FromForm] ProductContractModel productViewModel, IFormFile Model)
        {
            if (Model is null) return BadRequest();
#pragma warning disable
            productViewModel.File = Model as FormFile;
#pragma warning restore
            var productViewIsValid = _strategy.StrategyCondition(new List<Predicate<ProductContractModel>>()
            {
                (pw) => pw != null,
                (pw) => !string.IsNullOrWhiteSpace(pw.Name),
                (pw) => pw.Quantity >= 1,
                (pw) => pw.Price >= 0.1,
                (pw) => pw.File is not null
            }, productViewModel);
            if (!productViewIsValid)
            {
                _logger.LogError(string.Concat(DateTime.UtcNow, " ",  this.ToString(),  " product is not valid ", productViewIsValid.NumberConditionCounter ));
                return BadRequest();
            }
            var mapper = new Mapper(new MapperConfiguration(cfg => cfg.CreateMap<ProductContractModel, Product>()
                .ForMember("Name", opt => opt.MapFrom(o => o.Name))
                .ForMember("Price", opt => opt.MapFrom(o => o.Price))
                .ForMember("Quantity", opt => opt.MapFrom(o => o.Quantity))
                .ForMember("Description", opt => opt.MapFrom(o => o.Description))
                .ForMember("Id", opt => opt.MapFrom(o => o.id))
                ));
            var product = mapper.Map<Product>(productViewModel);
            var productInBase = _productOperations.GetById(product.Id);
            if (productInBase is null) return NoContent();
            product.Url = productInBase.Url;
            try
            {
                var listPropertyInBase = productInBase.GetType().GetProperties().ToList();
                var listPropertyMap = product.GetType().GetProperties().ToList();
                var oldprice = productInBase.Price;
                for (int countProperty = 0;countProperty < listPropertyInBase.Count; countProperty++)
                {
                    listPropertyInBase[countProperty].SetValue(productInBase,listPropertyMap[countProperty].GetValue(product));
                }
                productInBase.OldPrice = oldprice;
                await _productOperations.UpdateProductAsync(productInBase, productViewModel.File!.OpenReadStream(), productViewModel.File.FileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Concat(DateTime.UtcNow, " ", ex.Message, " ", this.ToString()));
                return Conflict();
            }
            return Ok();
        }
        [HttpPost("/AdminOperation/DeleteProduct")]
        public async Task<IActionResult> DeleteProduct(DeleteProductContracts deleteProductViewModel)
        {
            var deleteViewIsValid = _strategy.StrategyCondition(new List<Predicate<DeleteProductContracts>>()
            {
                (pw) => pw != null,
            }, deleteProductViewModel);
            if (!deleteViewIsValid) return BadRequest();
            try
            {
                await _productOperations.DeleteProductAsync(deleteViewIsValid.Object!.Id, deleteViewIsValid.Object!.NameOfFile);
                return Ok();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest();
            }
        }
        [HttpPost("/AdminOperation/ChangeUserRole")]
        public async Task<IActionResult> ChangeUserRole([FromForm]ChangeRoleContract roleContract)
        {

            var IsValid = _strategy.StrategyCondition(new List<Predicate<ChangeRoleContract>>()
            {
                (pw) => !string.IsNullOrEmpty(pw.Email),
                (pw) => !string.IsNullOrEmpty(pw.Role),
                (pw) => roleContract.Role!.ToLower().Equals(RolesType.User.ToString().ToLower()) | roleContract.Role.ToLower().Equals(RolesType.Admin.ToString().ToLower())
            }, roleContract);
            if (!IsValid) return BadRequest();
            try
            {
                var res = await _userOperations.AddRoleFromUserAsync(roleContract.Email!, roleContract.Role!);
                if (!res.IsSucceced)
                {
                    foreach(var error in res.errors)
                    {
                        _logger.LogError(error.Message);
                    }
                    return BadRequest();
                }
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest();
            }

        }
       
    }
    
}

