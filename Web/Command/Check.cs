using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Web
{
    public class Check
    {
        /// <summary>
        /// 检测是否字符串
        /// </summary>
        /// <param name="Account"></param>
        /// <returns></returns>
        public static bool IsStr(string Account, int MinLength = 4, int MaxLength = 15)
        {
            var Reg = @"^[A-Za-z0-9|\u4e00-\u9fa5]{MinLength,MaxLength}$";
            Reg = Reg.Replace("MinLength", MinLength.ToString()).Replace("MaxLength", MaxLength.ToString());
            return string.IsNullOrEmpty(Account) ? false : Regex.IsMatch(Account.Trim(), Reg);
        }
        /// <summary>
        /// 检测是否密码
        /// </summary>
        /// <param name="PassWord"></param>
        /// <returns></returns>
        public static bool IsPassWord(string PassWord)
        {
            return string.IsNullOrEmpty(PassWord) ? false : Regex.IsMatch(PassWord.Trim(), @"^[A-Za-z0-9]{4,32}$");
        }
    }
}
