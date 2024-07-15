using BuisnesLogic.ServicesInterface;
using AutoMapper;
using Infrastructure;
using DomainModel;
using Microsoft.AspNetCore.Mvc;
using DataBase.AppDbContexts;
using ApplicationInfrastructure;
using Microsoft.IdentityModel.Tokens;
namespace Application.Controllers
{
    public partial class AdminController : Controller
    {
        public IActionResult AdminPanel() => View();
    }
  
}
