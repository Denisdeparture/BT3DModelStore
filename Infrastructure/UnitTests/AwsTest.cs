using AppServiceInterfice.Service;
using AppServiceInterfice.ServicesInterface;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests
{
    public class AwsTest
    {
      
        public async void AddTest()
        {
           string AccessKey = "";
           string SecretKey = "";
           YandexCloudClient test = new YandexCloudClient(AccessKey, SecretKey, "testbucketmyapp");
            // Здесь должен быть IFormFile
            using (var stream = new FileStream(@"D:\загрузки\Снимок экрана 2024-06-09 190949.png", FileMode.Open))
            {
               var res = await test.AddModel(stream, @"D:\загрузки", "Снимок экрана 2024-06-09 190949.png");
               Assert.True(res.isCorrect);
            }
        }
       
        public async void UpdateTest()
        {
            string AccessKey = "";
            string SecretKey = "";
            YandexCloudClient test = new YandexCloudClient(AccessKey, SecretKey, "testbucketmyapp");
            // Здесь должен быть IFormFile
            using (var stream = new FileStream(@"D:\загрузки\Снимок экрана 2024-06-09 155341.png", FileMode.Open))
            {
                var res = await test.UpdateModel(stream, @"D:\загрузки", "Снимок экрана 2024-06-09 190949.png");
                Assert.True(res.isCorrect);
            }
        }
        [Fact]
        public async void DeleteTest()
        {
           string AccessKey = "";
           string SecretKey = "";
           YandexCloudClient test = new YandexCloudClient(AccessKey, SecretKey, "testbucketmyapp");
           var res = await test.DeleteModel(@"D:\загрузки", "Снимок экрана 2024-06-09 190949.png");
           Assert.True(res);

        }
    }
}
