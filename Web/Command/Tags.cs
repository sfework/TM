using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
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
        [HtmlAttributeName("Page")]
        public int Page { get; set; } = 1;
        [HtmlAttributeName("PageSize")]
        public int PageSize { get; set; } = 20;
        [HtmlAttributeName("TotalCount")]
        public int TotalCount { get; set; }

        [HtmlAttributeName("LeftColspan")]
        public int LeftColspan { get; set; } = 2;
        [HtmlAttributeName("RightColspan")]
        public int RightColspan { get; set; } = 2;

        public int MaxPage { get; set; }

        //public PagingTagHelper(IHtmlHelper htmlHelper)
        //{
        //    Html = htmlHelper;
        //}
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
        [HtmlAttributeName("ID")]
        public string ID { get; set; } = null;
        /// <summary>
        /// 默认提示语
        /// </summary>
        [HtmlAttributeName("Placeholder")]
        public string Placeholder { get; set; } = null;

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
        public object Default { get; set; } = null;
        [HtmlAttributeName("Clearable")]
        public bool Clearable { get; set; } = true;
    }
    [HtmlTargetElement("RoleSelect")]
    public class RoleSelectTagHelper : SelectModel
    {
        public RoleSelectTagHelper(IHtmlHelper htmlHelper) : base(htmlHelper)
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

    [HtmlTargetElement("AvatarSelect")]
    public class AvatarSelectTagHelper : SelectModel
    {
        public AvatarSelectTagHelper(IHtmlHelper htmlHelper) : base(htmlHelper)
        {
        }

        public override async Task ProcessAsync(TagHelperContext Context, TagHelperOutput Output)
        {
            foreach (var item in new System.IO.DirectoryInfo("wwwroot/Avatar").GetFiles())
            {
                Items.Add(item.Name, item.FullName.Replace(System.IO.Directory.GetCurrentDirectory() + "\\wwwroot", ""));
            }
            Output.TagName = "";
            Output.Content.SetHtmlContent(await Html.PartialAsync("/Views/Shared/_Select_Avatar.cshtml", this));
        }
    }
    [HtmlTargetElement("SelectEnum")]
    public class SelectEnumTagHelper : SelectModel
    {
        [HtmlAttributeName("Enum")]
        public Type EnumValue { get; set; }


        public SelectEnumTagHelper(IHtmlHelper htmlHelper) : base(htmlHelper)
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
}
