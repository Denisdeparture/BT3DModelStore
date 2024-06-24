using BuisnesLogic.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisnesLogic.ServicesInterface
{
    public interface IYandexClient
    {
        public Task<bool> DeleteModel(string postfix, string nameofmodel);
        public Task<AwsActionResultModel> AddModel(Stream file, string postfix, string nameofmodel);
        public Task<AwsActionResultModel> UpdateModel(Stream newfile, string postfix, string nameofmodel);
    }
}
