using ProjLogin.Encrypt;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using Xunit.Abstractions;

namespace Test_ProjLogin.Encrypt
{
    public class UnitTest_HashMethods
    {
        protected readonly ITestOutputHelper Output;

        public UnitTest_HashMethods(ITestOutputHelper tempOutput)
        {
            Output = tempOutput;
        }
        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void Test1(int value)
        {
            Assert.True(value > 0);
        }

        [Theory]
        [InlineData(@"123456")]
        [InlineData(@"feqwrd#221@A")]
        [InlineData(@"=-z/x.,$32\dee")]
        public void Test_HashPassword_Verify(string password)
        {

            string hash = "";
            string salt = "4PM3d2Lw9qo=";//default(string);
            HashMethods.HashPassword(password, out hash, out salt);
            Output.WriteLine("Salt: " + salt);
            Output.WriteLine("hash: " + hash);
            //Assert.Equal<string>(salt, "4PM3d2Lw9qo=");
            //Assert.Equal<string>(hash, "ooKr9RPynKZFBrPXKp302Yy+BLdJe4lIe7d8WFJwzzo=");

            //Assert.True(hash == "ooKr9RPynKZFBrPXKp302Yy+BLdJe4lIe7d8WFJwzzo=");

            //verify
            Assert.True(HashMethods.VerifyOnlineUser(password, hash, salt));
        }
        [Fact]
        public void Test2()
        {
            Assert.True(HashMethods.VerifyOnlineUser("12345", @"LQTcPMyEYDPqRfyXUpYGq93OvZheVA2qeZYObeUG/ms=",
            @"HXRF0MwKejk ="));
        }

    }
}