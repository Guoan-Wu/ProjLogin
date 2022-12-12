using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ProjLogin.Controllers;
using Microsoft.AspNetCore.TestHost;

namespace Test_ProjLogin.Controllers
{
    public class UserControllerTest
    {
        public UserControllerTest()
        {
            //var server = new TestServer(WebHost.CreateDefaultBuilder()
            //    .UseStartup<Startup>());
          //  Client = server.CreateClient();
        }

        public HttpClient Client { get; }

        [Fact]
        public async Task GetById_ShouldBe_Ok()
        {
            // Arrange
            string email = "jack@jack.nz";
            string password = "123456";


            // Act
            var response = await Client.GetAsync($"api/user/Login/{email},{password}");  

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }

}
