using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace TestData
{
    class Program
    {
        static void Main(string[] args)
        {
            var re = DES.Decrypt(Encoding.UTF8.GetString(Convert.FromBase64String("TjBPY3IxQWp2N1JGTUliZE1ENy9CUT09")));
            
            Console.ReadKey();
        }
    }

    public class DES
    {
        //private static readonly string DesKey = "!*&@#^%$";
        private static readonly string DesIv = "@)!^@)!&";
        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="sourceString"></param>
        /// <param name="DesKey"></param>
        /// <returns></returns>
        public static string Encrypt(string sourceString, string DesKey = "!*&@#^%$")
        {
            var btKey = Encoding.Default.GetBytes(DesKey);
            var btIv = Encoding.Default.GetBytes(DesIv);
            using (var des = new DESCryptoServiceProvider())
            {
                using (var ms = new MemoryStream())
                {
                    var inData = Encoding.Default.GetBytes(sourceString);
                    try
                    {
                        using (var cs = new CryptoStream(ms, des.CreateEncryptor(btKey, btIv), CryptoStreamMode.Write))
                        {
                            cs.Write(inData, 0, inData.Length);
                            cs.FlushFinalBlock();
                        }

                        return Convert.ToBase64String(ms.ToArray());
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
        }
        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="encryptedString"></param>
        /// <param name="DesKey">todo: describe DesKey parameter on Decrypt</param>
        /// <returns></returns>
        public static string Decrypt(string encryptedString, string DesKey = "!*&@#^%$")
        {
            var btKey = Encoding.Default.GetBytes(DesKey);
            var btIv = Encoding.Default.GetBytes(DesIv);
            using (var des = new DESCryptoServiceProvider())
            {
                using (var ms = new MemoryStream())
                {
                    try
                    {
                        var inData = Convert.FromBase64String(encryptedString);
                        using (var cs = new CryptoStream(ms, des.CreateDecryptor(btKey, btIv), CryptoStreamMode.Write))
                        {
                            cs.Write(inData, 0, inData.Length);
                            cs.FlushFinalBlock();
                        }

                        return Encoding.Default.GetString(ms.ToArray());
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
        }

    }
    public class TData
    {
        private readonly string ConnectionString;
        public TData(string _ConnectionString)
        {
            ConnectionString = _ConnectionString;
        }
        public void Clear()
        {
            Values.Clear();
        }
        public void AddString(string FieldName, int Length)
        {
            char[] Pattern = new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
            string result = "";
            int n = Pattern.Length;
            Random random = new Random(~unchecked((int)DateTime.Now.Ticks));
            for (int i = 0; i < Length; i++)
            {
                int rnd = random.Next(0, n);
                result += Pattern[rnd];
            }
            Values.Add((FieldName, result, true));
        }
        public void AddBool(string FieldName, bool? FixedValue = null)
        {
            if (FixedValue.HasValue)
            {
                Values.Add((FieldName, FixedValue.Value ? 1 : 0, false));
            }
            else
            {
                Random random = new Random(~unchecked((int)DateTime.Now.Ticks));
                Values.Add((FieldName, random.Next(0, 2), false));
            }
        }
        public void AddNumber(string FieldName, int Length)
        {
            char[] Pattern = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            string result = "";
            int n = Pattern.Length;
            Random random = new Random(~unchecked((int)DateTime.Now.Ticks));
            for (int i = 0; i < Length; i++)
            {
                int rnd = random.Next(0, n);
                result += Pattern[rnd];
            }
            Values.Add((FieldName, result, false));
        }
        public void AddDate(string FieldName)
        {
            DateTime Start = DateTime.Now.AddYears(-10);
            var ts = new TimeSpan(DateTime.Now.Ticks - DateTime.Now.AddYears(-10).Ticks).TotalSeconds;
            Random random = new Random(~unchecked((int)DateTime.Now.Ticks));
            Values.Add((FieldName, Start.AddSeconds(random.NextDouble() * ts).ToString("yyyy-MM-dd HH:mm:ss"), true));
        }
        private readonly List<(string FieldName, object Value, bool IsQuotation)> Values = new List<(string FieldName, object Value, bool IsQuotation)>();

        public void BuildSQL(string TableName, int Count)
        {
            string SQL = $"INSERT INTO {TableName} () VALUES ()";
            string Fields = string.Empty;
            foreach (var item in Values)
            {

            }
        }
    }
}
