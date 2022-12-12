using Microsoft.EntityFrameworkCore;
using Moq;
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
            string email = "jack@jack.abc";
            User user = new User(1,"","",email,"",1,true,null);

            mockRespo.Setup(x => x.GetID("jack@jack.abc")).Returns(user);
            UserLogic.SetDBContext(mockRespo.Object);


            //act
            var result = UserLogic.Login(email, "");

            //verify
            Assert.Equal(user, result);
        }
        [Fact]
        public void Login_WithFail()
        {
            //Arrange
            var mockRespo = new Mock<IUserRepository>();
            User user = new User(1, "", "others@gmail.com", "", "", 1, true, null);
            string email = "jack@jack.abc";

            mockRespo.Setup(x => x.GetID(user.Email)).Returns(user);
            UserLogic.SetDBContext(mockRespo.Object);

            //act
            var result = UserLogic.Login(email, "");

            //verify
            Assert.True(user != result);
        }
    }

}
