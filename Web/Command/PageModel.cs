using Microsoft.AspNetCore.Html;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web
{
    /// <summary>
    /// 分页统一基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PageModel<T>
    {
        /// <summary>
        /// 页码
        /// </summary>
        public int Page { get; set; } = 1;
        /// <summary>
        /// 页大小
        /// </summary>
        public int PageSize { get; set; } = 20;
        /// <summary>
        /// 分页数据
        /// </summary>
        public IQueryable<T> List { get; set; }

        public int TotalCount { get; set; }
        /// <summary>
        /// 分页模型初始化
        /// </summary>
        public void Create()
        {
            TotalCount = List.Count();
            List = List.Skip((Page - 1) * PageSize).Take(PageSize);
        }
    }
}
