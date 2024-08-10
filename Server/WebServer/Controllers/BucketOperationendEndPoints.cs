using Azure;
using BuisnesLogic.ConstStorage;
using Contracts;
using DomainModel.Entity;
using Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Text.Json;

namespace WebServer.Controllers
{
    public class BucketOperationendEndPoints : ControllerBase
    {
        private readonly IBucketOperation _bucketOperation;
        private readonly ILogger<Program> _logger;
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;
        public BucketOperationendEndPoints(IBucketOperation bucketOperation, ILogger<Program> logger, UserManager<User> userManager, IConfiguration configuration)
        {
            _bucketOperation = bucketOperation ?? throw new ArgumentNullException(nameof(bucketOperation));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _configuration = configuration;
        }
        [HttpPost("/BucketOperationendEndPoints/AddInBucket")]
        public async Task<IResult>AddProductInBucket([FromForm] string productInBucket)
        {
            try
            {
                var product = await HttpContext.Request.ReadFromJsonAsync<ProductInBucketContract>() ?? throw new NullReferenceException();
                var user = await _userManager.FindByEmailAsync(product!.UserEmail) ?? throw new NullReferenceException();
                _bucketOperation.Add(user, product.Product);
                return Results.Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Concat(DateTime.UtcNow, " ", this.ToString(), " ", ex.Message));
                return Results.BadRequest();
            }
          
        }
        [HttpPost("/BucketOperationendEndPoints/RemoveInBucket")]
        public async Task<IResult>RemoveProductInBucket([FromQuery] int id, string email)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email) ?? throw new NullReferenceException();
                _bucketOperation.Remove(user, id);
                string url = _configuration["ClientHostForRedirect"]! + "/" + EndpointValueInStringStorage.MainController + "/" + Url.Action(EndpointValueInStringStorage.RedirectToBucket, new { email });
                return Results.Redirect(url);
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Concat(DateTime.UtcNow, " ", this.ToString(), " ", ex.Message));
                return Results.BadRequest();
            }
        }
        [HttpPost("/BucketOperationendEndPoints/Increase")]
        public async Task<IResult> Increase([FromQuery] int id, string email)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email) ?? throw new NullReferenceException();
                _bucketOperation.IncreaseCount(user, id);
                string url = _configuration["ClientHostForRedirect"]! + "/" + EndpointValueInStringStorage.MainController + "/" + Url.Action(EndpointValueInStringStorage.RedirectToBucket, new { email });
                return Results.Redirect(url);
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Concat(DateTime.UtcNow, " ", this.ToString(), " ", ex.Message));
                return Results.BadRequest();
            }
        }
        [HttpPost("/BucketOperationendEndPoints/Decrease")]
        public async Task<IResult> Decrease([FromQuery] int id, string email)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email) ?? throw new NullReferenceException();
                _bucketOperation.DecreaseCount(user, id);
                string url = _configuration["ClientHostForRedirect"]! + "/" + EndpointValueInStringStorage.MainController + "/" + Url.Action(EndpointValueInStringStorage.RedirectToBucket, new { email });
                return Results.Redirect(url);
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Concat(DateTime.UtcNow, " ", this.ToString(), " ", ex.Message));
                return Results.BadRequest();
            }
        }
    }
}
