using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities.EventHandlers;
using ProjLogin.DTO;
using ProjLogin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test_ProjLogin.Mapper
{
    public class MapContainerTest
    {
        [Fact]
        public static void Map()
        {
            RegisterDTO source1 = new RegisterDTO { Email = "abc@12.nz", Password = "qwert12345", Name = "Name1" };            
            var dest1 = MapContainer.Map<RegisterDTO, User>(source1);
            Assert.Equal(source1.Email, dest1.Email);
            Assert.Equal(source1.Password,dest1.Password);



            LoginDTO source = new LoginDTO { Email = "12a@gmail.nz", Password = "1234er" };
            var dest2 = MapContainer.Map<LoginDTO, User>(source);
            Console.WriteLine(dest2);

            Assert.True(true);
        }
    }
}
