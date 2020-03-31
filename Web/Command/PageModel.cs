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
        public int PageSize { get; set; } = 15;
        /// <summary>
        /// 分页数据
        /// </summary>
        public IQueryable<T> List { get; set; }

        public int TotalCount { get; set; }
        /// <summary>
        /// 分页控制
        /// </summary>
        //public HtmlString PageHtml { get; set; }
        /// <summary>
        /// 分页模型初始化
        /// </summary>
        public void Create()
        {
            TotalCount = List.Count();
            //int MaxPage = 1;
            //if (TotalCount > 0 && PageSize > 0)
            //{
            //    MaxPage = (int)Math.Ceiling(TotalCount * 1.0 / PageSize);
            //}
            //var PageStr = "<div class=\"ui pagination menu paging\">";
            //PageStr += $"<div class=\"item\" style=\"font-size:12px;\">共计：{TotalCount}条，{Page}/{MaxPage}页</div>";
            //PageStr += $"<div class=\"item fitted\">";
            //PageStr += $"<div class=\"ui selection dropdown\">";
            //PageStr += $"<input type=\"hidden\" value=\"{PageSize}\">";
            //PageStr += $"<i class=\"dropdown icon\"></i>";

            //PageStr += $"<div class=\"text\">{PageSize}/页</div>";
            //PageStr += $"<div class=\"menu\">";
            //PageStr += $"<div class=\"item active selected\" data-value=\"{PageSize}\">{PageSize}/页</div>";
            //PageStr += $"<div class=\"item\" data-value=\"50\">50/页</div>";
            //PageStr += $"<div class=\"item\" data-value=\"100\">100/页</div>";
            //PageStr += "</div></div></div>";
            //if (Page <= 1)
            //{
            //    PageStr += $"<a class=\"icon item disabled\"><i class=\"left double angle icon\"></i></a>";
            //    PageStr += $"<a class=\"icon item disabled\"><i class=\"left angle icon\"></i></a>";
            //}
            //else
            //{
            //    PageStr += $"<a class=\"icon item\" href=\"javascript:url.load(1)\"><i class=\"left double angle icon\"></i></a>";
            //    PageStr += $"<a class=\"icon item\" href=\"javascript:url.load({Page - 1})\"><i class=\"left angle icon\"></i></a>";
            //}

            //if (Page < MaxPage)
            //{
            //    PageStr += $"<a class=\"icon item\" href=\"javascript:url.load({Page + 1})\"><i class=\"right angle icon\"></i></a>";
            //    PageStr += $"<a class=\"icon item\" href=\"javascript:url.load({MaxPage})\"><i class=\"right double angle icon\"></i></a>";
            //}
            //else
            //{
            //    PageStr += $"<a class=\"icon item disabled\"><i class=\"right angle icon\"></i></a>";
            //    PageStr += $"<a class=\"icon item disabled\"><i class=\"right double angle icon\"></i></a>";
            //}
            //PageStr += $"</div>";
            //PageHtml = new HtmlString(PageStr);
            List = List.Skip((Page - 1) * PageSize).Take(PageSize);
        }
    }

    public class PagingModel
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public int TotalCount { get; set; }
        public int MaxPage { get; set; }
    }
}
