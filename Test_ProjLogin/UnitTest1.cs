using ProjLogin.Encrypt;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using Xunit.Abstractions;

namespace ProjLogin.Test_ProjLogin
{
   
    public class UnitTest_GeneralMethods
    {
        protected readonly ITestOutputHelper Output;

        public UnitTest_GeneralMethods(ITestOutputHelper tempOutput)
        {
            Output = tempOutput;
        }
        [Fact]
        public void Test_HashPassword()
        {
            
            //string hash = "";
            //string salt = default(string);
            //GeneralMethods.HashPassword("12345", out hash, out salt);
            //Output.WriteLine(hash);
            //Output.WriteLine(salt);
            //Assert.Equal<string>(salt, "123");
            //Assert.Equal<string>(hash, "12345");
            
            //Assert.True(hash == "12345");
        }
        [Fact]
        public void Test_VerifyOnlineUser()
        {

            Assert.True(GeneralMethods.VerifyOnlineUser("12345",@"LQTcPMyEYDPqRfyXUpYGq93OvZheVA2qeZYObeUG / ms =",
            @"HXRF0MwKejk ="));


        }
    }
}