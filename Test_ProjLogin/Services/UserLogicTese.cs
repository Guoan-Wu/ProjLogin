using Microsoft.EntityFrameworkCore;
using Moq;
using ProjLogin.DTO;
using ProjLogin.Models;
using ProjLogin.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;  
using System.Threading.Tasks;

namespace Test_ProjLogin.Services
{
   public class UserLogicTest
    {
        [Fact(DisplayName ="Test UserLogic.login()")]
        public void Login_WithSucceed()
        {
            //Arrange
            var mockRespo = new Mock<IUserRepository>();
            string email = "jack14@jack.nz";
            User user = new User(1, "jack14",email, @"8WGzAbClvDaBTmRBEHHDfdTPFu3Pb/r/K81QcRhMhlo=", @"Ze/OD+QOr/8=", 1,true,null);

            mockRespo.Setup(x => x.GetID(email)).Returns(user);
            UserLogic.SetDBContext(mockRespo.Object);


            //act
            var result = UserLogic.Login(new LoginDTO { Email = email, Password = "123456" });

            //verify
            Assert.Equal(user, result);
        }
        [Fact]
        public void Login_WithFail()
        {           
            try
            {
                User user = new User(1, "jack14", @"jack14@jack.nz", @"8WGzAbClvDaBTmRBEHHDfdTPFu3Pb/r/K81QcRhMhlo=", @"Ze/OD+QOr/8=", 1, true, null);
                //Arrange
                var mockRespo = new Mock<IUserRepository>();

                string email = "noname@jack.abc";

#pragma warning disable CS8604 // Possible null reference argument.
                mockRespo.Setup(x => x.GetID(user.Email)).Returns(user);
#pragma warning restore CS8604 // Possible null reference argument.
                UserLogic.SetDBContext(mockRespo.Object);

                //act
                var result = UserLogic.Login(new LoginDTO { Email = email, Password = "wrongpass" });   
                Assert.False(user == result);

            }
            catch
            {
                Assert.False(false);
            }           
        }
    }

}
