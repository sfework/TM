using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TMT
{
    /// <summary>
    /// 分页
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PageModel<T> : PageBase
    {
        /// <summary>
        /// 分页数据
        /// </summary>
        public IQueryable<T> List { get; set; }

        public void Build()
        {
            TotalCount = List.Count();
            if (TotalCount > 0 && PageSize > 0)
            {
                PageCount = (int)Math.Ceiling(TotalCount * 1.0 / PageSize);
            }
            else
            {
                PageCount = 1;
            }
            List = List.Skip((PageNum - 1) * PageSize).Take(PageSize);
        }
    }
    public class PageBase
    {
        /// <summary>
        /// 分页数
        /// </summary>
        public int PageCount { get; set; }
        /// <summary>
        /// 总数
        /// </summary>
        public int TotalCount { get; set; }
        /// <summary>
        /// 页码
        /// </summary>
        public int PageNum { get; set; } = 1;
        /// <summary>
        /// 页大小
        /// </summary>
        public int PageSize { get; set; } = 20;


        public void FirstPage()
        {
            PageNum = 1;
            ReLoad?.Invoke();
        }


        public void LastPage()
        {
            PageNum = PageCount;
            ReLoad?.Invoke();
        }

        public void NextPage()
        {
            if (PageNum < PageCount)
            {
                PageNum++;
                ReLoad?.Invoke();
            }
        }

        public void PrevPage()
        {
            if (PageNum > 1)
            {
                PageNum--;
                ReLoad?.Invoke();
            }
        }

        public void SetPage(int Page)
        {
            if (PageNum != Page)
            {
                PageNum = Page;
                ReLoad?.Invoke();
            }
        }

        public void SetPageSize(int Size)
        {
            if (PageSize != Size)
            {
                PageNum = 1;
                PageSize = Size;
                ReLoad?.Invoke();
            }
        }

        public Action ReLoad { get; set; }
    }
}
