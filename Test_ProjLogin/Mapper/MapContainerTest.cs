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
        public static void Map_RegisterDTO_User()
        {
            RegisterDTO source1 = new RegisterDTO { Email = "abc@12.nz", Password = "qwert12345", Name = "Name1" };
            var dest1 = MapContainer.Map<RegisterDTO, User>(source1);
            object[] src = { source1.Name, source1.Email, source1.Password };
            object[] dst = { dest1.User_name, dest1.Email, dest1.Password };
            Assert.Equal(src, dst);
        }
        
        [Fact]
        public static void Map_LoginDTO_User()
        { 
            LoginDTO source = new LoginDTO { Email = "12a@gmail.nz", Password = "1234er" };
            var dest2 = MapContainer.Map<LoginDTO, User>(source);
            object[] src = {  source.Email, source.Password };
            object[] dst = {  dest2.Email, dest2.Password };
            Assert.Equal(src, dst);
        }
    }
}
