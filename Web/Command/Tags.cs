using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web
{
    public class TagHelperBase : TagHelper
    {
        [HtmlAttributeNotBound]
        public IHtmlHelper Html { get; set; }
        [HtmlAttributeNotBound]
        private ViewContext _ViewContext { get; set; }
        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext VContext { get { return _ViewContext; } set { _ViewContext = value; (Html as IViewContextAware).Contextualize(_ViewContext); } }
        public TagHelperBase(IHtmlHelper htmlHelper)
        {
            Html = htmlHelper;
        }
    }


    [HtmlTargetElement("Paging")]
    public class PagingTagHelper : TagHelperBase
    {
        public PagingTagHelper(IHtmlHelper htmlHelper) : base(htmlHelper)
        {
        }
        [HtmlAttributeNotBound]
        public int MaxPage { get; set; }
        [HtmlAttributeName("Page")]
        public int Page { get; set; }
        [HtmlAttributeName("PageSize")]
        public int PageSize { get; set; }
        [HtmlAttributeName("TotalCount")]
        public int TotalCount { get; set; }

        public override async Task ProcessAsync(TagHelperContext Context, TagHelperOutput Output)
        {
            MaxPage = TotalCount / PageSize;
            if (TotalCount % PageSize > 0)
            {
                MaxPage++;
            }
            Output.TagName = "";
            Output.Content.SetHtmlContent(await Html.PartialAsync("/Views/Shared/_Paging.cshtml", this));
        }
    }

    public class SelectModel : TagHelperBase
    {
        public SelectModel(IHtmlHelper htmlHelper) : base(htmlHelper)
        {
        }
        [HtmlAttributeName("Class")]
        public string Class { get; set; }
        [HtmlAttributeName("ID")]
        public string ID { get; set; }
        /// <summary>
        /// 默认提示语
        /// </summary>
        [HtmlAttributeName("Placeholder")]
        public string Placeholder { get; set; }

        /// <summary>
        /// 指定选择框的最大宽度
        /// </summary>
        [HtmlAttributeName("Width")]
        public int Width { get; set; } = 0;
        /// <summary>
        /// 是否支持多选
        /// </summary>
        [HtmlAttributeName("Multiple")]
        public bool Multiple { get; set; } = false;
        /// <summary>
        /// 选择项
        /// </summary>
        [HtmlAttributeNotBound]
        public Dictionary<string, object> Items { get; set; } = new Dictionary<string, object>();
        /// <summary>
        /// 默认选中的枚举数值（可为int类型的枚举数值，也可以是枚举值）
        /// </summary>
        [HtmlAttributeName("Default")]
        public object Default { get; set; }
        [HtmlAttributeName("Clearable")]
        public bool Clearable { get; set; } = true;

        [HtmlAttributeName("Columns")]
        public int Columns { get; set; }
        [HtmlAttributeName("Search")]
        public bool Search { get; set; }
        [HtmlAttributeName("Fluid")]
        public bool Fluid { get; set; } = true;
    }
    [HtmlTargetElement("SelectRoles")]
    public class SelectRolesTagHelper : SelectModel
    {
        public SelectRolesTagHelper(IHtmlHelper htmlHelper) : base(htmlHelper)
        {
        }

        public override async Task ProcessAsync(TagHelperContext Context, TagHelperOutput Output)
        {
            var DB = (Models.DBContext)VContext.HttpContext.RequestServices.GetService(typeof(Models.DBContext));
            foreach (var item in DB.TMT_Roles)
            {
                Items.Add(item.RoleName, item.RoleID);
            }
            Output.TagName = "";
            Output.Content.SetHtmlContent(await Html.PartialAsync("/Views/Shared/_Select.cshtml", this));
        }
    }

    [HtmlTargetElement("SelectAvatars")]
    public class SelectAvatarsTagHelper : SelectModel
    {
        [HtmlAttributeName("RandomSet")]
        public bool RandomSet { get; set; }

        public SelectAvatarsTagHelper(IHtmlHelper htmlHelper) : base(htmlHelper)
        {
        }

        public override async Task ProcessAsync(TagHelperContext Context, TagHelperOutput Output)
        {
            foreach (var item in new System.IO.DirectoryInfo("wwwroot/Avatar").GetFiles())
            {
                Items.Add(item.Name, item.FullName.Replace(System.IO.Directory.GetCurrentDirectory() + "\\wwwroot", ""));
            }
            if (RandomSet && Default == null)
            {
                Default = Items.Skip(new Random().Next(0, Items.Count)).Take(1).FirstOrDefault().Value;
            }
            Output.TagName = "";
            Output.Content.SetHtmlContent(await Html.PartialAsync("/Views/Shared/_Select.cshtml", this));
        }
    }
    [HtmlTargetElement("SelectEnums")]
    public class SelectEnumsTagHelper : SelectModel
    {
        [HtmlAttributeName("Enum")]
        public Type EnumValue { get; set; }


        public SelectEnumsTagHelper(IHtmlHelper htmlHelper) : base(htmlHelper)
        {
        }
        public override async Task ProcessAsync(TagHelperContext Context, TagHelperOutput Output)
        {
            foreach (var item in Enum.GetValues(EnumValue))
            {
                Items.Add(item.ToString(), (int)item);
            }
            Output.TagName = "";
            Output.Content.SetHtmlContent(await Html.PartialAsync("/Views/Shared/_Select.cshtml", this));
        }
    }


    [HtmlTargetElement("SelectUsers")]
    public class SelectUsersTagHelper : SelectModel
    {
        public new List<(int, string, string)> Items { get; set; } = new List<(int, string, string)>();

        public SelectUsersTagHelper(IHtmlHelper htmlHelper) : base(htmlHelper)
        {
        }

        public override async Task ProcessAsync(TagHelperContext Context, TagHelperOutput Output)
        {
            var DB = (Models.DBContext)VContext.HttpContext.RequestServices.GetService(typeof(Models.DBContext));
            foreach (var item in DB.TMT_Users.AsNoTracking().Where(c => c.Enable))
            {
                Items.Add((item.UserID, item.UserName, item.Avatar));
            }
            Output.TagName = "";
            Output.Content.SetHtmlContent(await Html.PartialAsync("/Views/Shared/_Select.cshtml", this));
        }
    }
}
