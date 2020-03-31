using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Web.Command
{
    public class Helper
    {
        public static string IntsToString(int[] StrArr)
        {
            return string.Join(",", StrArr);
        }
        public static int[] StrToInts(string Str)
        {
            return Array.ConvertAll(Str.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries), c => int.Parse(c));
        }
        public static string GenerateMD5(string txt)
        {
            using MD5 mi = MD5.Create();
            byte[] buffer = Encoding.Default.GetBytes(txt);
            //开始加密
            byte[] newBuffer = mi.ComputeHash(buffer);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < newBuffer.Length; i++)
            {
                sb.Append(newBuffer[i].ToString("x2"));
            }
            return sb.ToString();
        }

        public static async Task<List<string>> SaveAsync(IFormFileCollection Files)
        {
            List<string> RePath = new List<string>();
            if (Files.Count > 0)
            {
                string UPath = string.Format("wwwroot/Files/{0}/{1}", DateTime.Now.Year, DateTime.Now.ToString("MMdd"));
                if (!System.IO.Directory.Exists(UPath))
                {
                    System.IO.Directory.CreateDirectory(UPath);
                }
                foreach (var item in Files)
                {
                    string FilePath = UPath + "/" + Guid.NewGuid().ToString("N") + System.IO.Path.GetExtension(item.FileName);
                    using var stream = new System.IO.FileStream(FilePath, System.IO.FileMode.Create);
                    await item.CopyToAsync(stream);
                    RePath.Add(FilePath.Replace("wwwroot", ""));
                }
            }
            return RePath;
        }
    }
}
