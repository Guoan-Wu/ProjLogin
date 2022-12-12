using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel;

namespace ProjLogin.Encrypt
{

   
    /// <summary>
    /// JWT操作帮助类
    /// </summary>
    public class JwtHelper
    {
        [Conditional("Debug")]
        public static void TestDisplay(string msg)
        {
            Console.WriteLine(msg);
        }
        /// <summary>
        /// 颁发JWT
        /// </summary>
        /// <param name="tokenModel">当前颁发对象的用户信息</param>
        /// <returns>JWT字符串</returns>
        public static string IssueJwt(string email, string password)
        {

            #region 【Step1-从配置文件中获取生成JWT所需要的数据】
            string? iss = Appsettings.GetVal(new string[] { "Audience", "Issuer" });//颁发者
            string? aud = Appsettings.GetVal(new string[] { "Audience", "Audience" });//使用者
            string? secret = Appsettings.GetVal(new string[] { "Audience", "Secret" }); //密钥
            #endregion

            #region 【Step2-通过Claim创建JWT中的Payload(载荷)信息】

            var claimsIdentity = new List<Claim>
                {
                 new Claim(JwtRegisteredClaimNames.Email,email), //JWT ID
                new Claim(JwtRegisteredClaimNames.Iat, $"{new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds()}"),//JWT的发布时间
                new Claim (JwtRegisteredClaimNames.Exp,$"{new DateTimeOffset(DateTime.Now.AddHours(3600)).ToUnixTimeSeconds()}"),//JWT到期时间
                new Claim(JwtRegisteredClaimNames.Iss,iss??string.Empty), //颁发者
                new Claim(JwtRegisteredClaimNames.Aud,aud??string.Empty),//使用者
                new Claim("pwd",password)
               };

            //添加用户的角色信息（非必须，可添加多个）
            string roles = "Admin,System,Guest";
            var claimRoleList = roles.Split(',').Select(role => new Claim(ClaimTypes.Role, role)).ToList();
            claimsIdentity.AddRange(claimRoleList );
            #endregion

            #region 【Step3-签名对象】

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret??string.Empty)); //创建密钥对象
            var sigCreds = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256); //创建密钥签名对象

            #endregion

            #region 【Step5-将JWT相关信息封装成对象】
            var jwt = new JwtSecurityToken(
              issuer: iss,
              claims: claimsIdentity,
              signingCredentials: sigCreds);
            #endregion

            #region 【Step6-将JWT信息对象生成字符串形式】
            var jwtHandler = new JwtSecurityTokenHandler();
            string token = jwtHandler.WriteToken(jwt);
            #endregion

            return token;
        } // END IssueJwt()

        /// <summary>
        /// 将JWT加密的字符串进行解析
        /// </summary>
        /// <param name="jwtStr">JWT加密的字符</param>
        /// <returns>JWT中的用户信息</returns>
        public static Tuple<string?,string?> SerializeJwtStr(string jwtStr)
        {
            var jwtHandler = new JwtSecurityTokenHandler();

            if (!string.IsNullOrEmpty(jwtStr) && jwtHandler.CanReadToken(jwtStr))
            {
                //将JWT字符读取到JWT对象
                JwtSecurityToken jwtToken = jwtHandler.ReadJwtToken(jwtStr);

                string? passowrd = jwtToken.Payload["pwd"].ToString();
                string? email = jwtToken.Payload[JwtRegisteredClaimNames.Email].ToString();                
                return Tuple.Create(email, passowrd);
            }
            return Tuple.Create<string?,string?>(null, null);
            
        } //END SerializeJwt()



    }
}