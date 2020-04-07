using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    public class DocumentController : ControllersBase
    {
        public IActionResult Index(ParamModels.Document.Document_Request Model)
        {
            if (!string.IsNullOrWhiteSpace(Model.KeyWord))
            {
                Model.IsSearch = true;
                Model.Paths = new List<(int, string, string, bool)>();
                Model.List = DB.TMT_Documents.Where(c => c.DocumentName.Contains(Model.KeyWord) && (c.CreateUserID == G.User.UserID || c.IsPublic)).OrderBy(c => c.DocumentType);
            }
            else
            {
                string TempID = Model.DocumentID;
                int ID = 0;
                List<(int, string, string, bool)> Paths = new List<(int, string, string, bool)>();
                while (!string.IsNullOrWhiteSpace(TempID))
                {
                    var Temp = DB.TMT_Documents.Find(TempID);
                    Paths.Add((ID, Temp.DocumentID, Temp.DocumentName, TempID != Model.DocumentID));
                    TempID = Temp.PathID;
                    ID++;
                }
                Model.Paths = Paths.OrderByDescending(c => c.Item1).ToList();
                Model.List = DB.TMT_Documents.Where(c => c.PathID == Model.DocumentID && (c.CreateUserID == G.User.UserID || c.IsPublic)).OrderBy(c => c.DocumentType);
            }
            Model.SharePath = HttpContext.Request.Scheme + "://" + HttpContext.Request.Host.ToString() + "/Document/Share?DocumentID=";
            return View(Model);
        }

        [HttpGet]
        public IActionResult Add(string PathID, int DocumentType, string DocumentID)
        {
            ViewBag.DocumentType = DocumentType;
            ViewBag.PathID = PathID;
            Models.TMT_Documents Model = null;
            if (!string.IsNullOrWhiteSpace(DocumentID))
            {
                Model = DB.TMT_Documents.Find(DocumentID);
                if (Model.CreateUserID != G.User.UserID)
                {
                    return Json("无权编辑此文档，必须是文档的创建者！");
                }
            }
            return View(Model);
        }
        [HttpPost]
        public async Task<IActionResult> Add(Models.TMT_Documents Model)
        {
            if (!string.IsNullOrWhiteSpace(Model.DocumentID))
            {
                var TempModel = DB.TMT_Documents.Find(Model.DocumentID);
                if (TempModel.CreateUserID != G.User.UserID)
                {
                    return Json("无权编辑此文档，必须是文档的创建者！");
                }
            }
            switch (Model.DocumentType)
            {
                case 0:
                    if (string.IsNullOrWhiteSpace(Model.DocumentName))
                    {
                        return Json("[文件夹名称]不可为空！");
                    }
                    if (!string.IsNullOrWhiteSpace(Model.DocumentID))
                    {
                        if (DB.TMT_Documents.Any(c => c.DocumentName == Model.DocumentName && c.DocumentType == Model.DocumentType && c.PathID == Model.PathID && c.DocumentID != Model.DocumentID))
                        {
                            return Json($"[{Model.DocumentName}]已经存在！");
                        }
                        var Temp = DB.TMT_Documents.Find(Model.DocumentID);
                        Temp.DocumentName = Model.DocumentName;
                        Temp.IsPublic = Model.IsPublic;
                    }
                    else
                    {
                        if (DB.TMT_Documents.Any(c => c.DocumentName == Model.DocumentName && c.DocumentType == Model.DocumentType && c.PathID == Model.PathID))
                        {
                            return Json($"[{Model.DocumentName}]已经存在！");
                        }
                        Model.DocumentID = Guid.NewGuid().ToString("N");
                        Model.Class = GetIcon(Model.DocumentType, Model.DocumentName);
                        Model.CreateUserID = G.User.UserID;
                        DB.TMT_Documents.Add(Model);
                    }
                    DB.SaveChanges();
                    break;
                case 1:
                    if (string.IsNullOrWhiteSpace(Model.DocumentName))
                    {
                        return Json("[文档名称]不可为空！");
                    }
                    if (!string.IsNullOrWhiteSpace(Model.DocumentID))
                    {
                        if (DB.TMT_Documents.Any(c => c.DocumentName == Model.DocumentName && c.DocumentType == Model.DocumentType && c.PathID == Model.PathID && c.DocumentID != Model.DocumentID))
                        {
                            return Json($"[{Model.DocumentName}]已经存在！");
                        }
                        var Temp = DB.TMT_Documents.Find(Model.DocumentID);
                        Temp.DocumentName = Model.DocumentName;
                        Temp.IsPublic = Model.IsPublic;
                        Temp.IsShare = Model.IsShare;
                    }
                    else
                    {
                        if (DB.TMT_Documents.Any(c => c.DocumentName == Model.DocumentName && c.DocumentType == Model.DocumentType && c.PathID == Model.PathID))
                        {
                            return Json($"[{Model.DocumentName}]已经存在！");
                        }
                        Model.DocumentID = Guid.NewGuid().ToString("N");
                        Model.Class = GetIcon(Model.DocumentType, Model.DocumentName);
                        Model.Content = "";
                        Model.CreateUserID = G.User.UserID;
                        Model.IsPublic = Model.IsPublic;
                        Model.IsShare = Model.IsShare;
                        DB.TMT_Documents.Add(Model);
                    }
                    DB.SaveChanges();
                    break;
                case 2:
                    if (string.IsNullOrWhiteSpace(Model.DocumentName))
                    {
                        return Json("[附件名称]不可为空！");
                    }
                    if (!string.IsNullOrWhiteSpace(Model.DocumentID))
                    {
                        if (DB.TMT_Documents.Any(c => c.DocumentName == Model.DocumentName && c.DocumentType == Model.DocumentType && c.PathID == Model.PathID && c.DocumentID != Model.DocumentID))
                        {
                            return Json($"[{Model.DocumentName}]已经存在！");
                        }
                        var Temp = DB.TMT_Documents.Find(Model.DocumentID);
                        Temp.DocumentName = Model.DocumentName;
                        Temp.IsPublic = Model.IsPublic;
                        if (HttpContext.Request.Form.Files.Count > 0)
                        {
                            var ReFiles = await Command.Helper.SaveAsync(HttpContext.Request.Form.Files);
                            Temp.Content = ReFiles.First();
                            Temp.Class = GetIcon(Model.DocumentType, Model.DocumentName);
                        }
                        Temp.IsShare = Model.IsShare;
                    }
                    else
                    {
                        if (DB.TMT_Documents.Any(c => c.DocumentName == Model.DocumentName && c.DocumentType == Model.DocumentType && c.PathID == Model.PathID))
                        {
                            return Json($"[{Model.DocumentName}]已经存在！");
                        }
                        if (HttpContext.Request.Form.Files.Count < 1)
                        {
                            return Json($"未检测到上传的附件！");
                        }
                        var ReFiles = await Command.Helper.SaveAsync(HttpContext.Request.Form.Files);
                        Model.DocumentID = Guid.NewGuid().ToString("N");
                        Model.Class = GetIcon(Model.DocumentType, Model.DocumentName);
                        Model.Content = ReFiles.First();
                        Model.CreateUserID = G.User.UserID;
                        DB.TMT_Documents.Add(Model);
                    }
                    DB.SaveChanges();
                    break;
                case 3:
                    if (string.IsNullOrWhiteSpace(Model.DocumentName))
                    {
                        return Json("[链接名称]不可为空！");
                    }
                    if (string.IsNullOrWhiteSpace(Model.Content))
                    {
                        return Json("[链接Url]不可为空！");
                    }
                    if (!string.IsNullOrWhiteSpace(Model.DocumentID))
                    {
                        if (DB.TMT_Documents.Any(c => c.DocumentName == Model.DocumentName && c.DocumentType == Model.DocumentType && c.PathID == Model.PathID && c.DocumentID != Model.DocumentID))
                        {
                            return Json($"[{Model.DocumentName}]已经存在！");
                        }
                        var Temp = DB.TMT_Documents.Find(Model.DocumentID);
                        Temp.DocumentName = Model.DocumentName;
                        Temp.IsPublic = Model.IsPublic;
                        Temp.Content = Model.Content;
                    }
                    else
                    {
                        if (DB.TMT_Documents.Any(c => c.DocumentName == Model.DocumentName && c.DocumentType == Model.DocumentType && c.PathID == Model.PathID))
                        {
                            return Json($"[{Model.DocumentName}]已经存在！");
                        }
                        Model.DocumentID = Guid.NewGuid().ToString("N");
                        Model.Class = GetIcon(Model.DocumentType, Model.DocumentName);
                        Model.CreateUserID = G.User.UserID;
                        DB.TMT_Documents.Add(Model);
                    }
                    DB.SaveChanges();
                    break;
            }
            return Json(new { Model.DocumentType, Model.DocumentID });
        }

        private string GetIcon(int DocumentType, string FileName)
        {
            switch (DocumentType)
            {
                case 0:
                    return "folder";
                case 1:
                    return "gdoc";
                case 2:
                    var Ext = System.IO.Path.GetExtension(FileName).Replace(".", "");
                    Dictionary<string, string> Dic = new Dictionary<string, string>
                    {
                        //音频,audio
                        { "mp3", "audio" },
                        //video，video
                        { "mp4", "video" },
                        //csv,csv
                        { "csv", "csv" },
                        //excel,excel
                        { "xls", "excel" },
                        { "xlsx", "excel" },
                        //word,word
                        { "doc", "word" },
                        { "docx", "word" },
                        //zip,zip
                        { "zip", "zip" },
                        { "rar", "zip" },
                        { "7z", "zip" },
                        //code,html

                        //image,image
                        { "jpg", "image" },
                        //pdf，pdf
                        { "pdf", "pdf" },
                        //ppt，ppt
                        { "ppt", "ppt" },
                        //txt，txt
                        { "txt", "txt" }
                    };
                    if (Dic.TryGetValue(Ext, out string value))
                    {
                        return value;
                    }
                    else
                    {
                        return "attachment";
                    }
                case 3:
                    return "link";
                default:
                    return "attachment";
            }
        }

        [HttpPost]
        public IActionResult Delete(string DocumentID)
        {
            var Model = DB.TMT_Documents.Find(DocumentID);
            if (Model.CreateUserID != G.User.UserID)
            {
                return Json("无权删除，必须是创建者！");
            }
            Model.IsDelete = true;
            DB.SaveChanges();
            return Json();
        }

        [HttpPost]
        public IActionResult Document_Save(string DocumentID, string Content)
        {
            var Model = DB.TMT_Documents.Find(DocumentID);
            if (Model.CreateUserID != G.User.UserID)
            {
                return Json("无权编辑此文档，必须是文档的创建者才可编辑！");
            }
            Model.Content = Content;
            DB.SaveChanges();
            return Json();
        }
        [AllowAnonymous]
        public IActionResult Share(string DocumentID)
        {
            var Model = DB.TMT_Documents.Find(DocumentID);
            if (Model.IsShare)
            {
                return View(Model);
            }
            else
            {
                var Re = new ContentResult
                {
                    Content = "无权查看此文档，文档必须是分享状态！"
                };
                return Re;
            }
        }
        public IActionResult Document_View(string DocumentID)
        {
            var Model = DB.TMT_Documents.Find(DocumentID);
            if (Model.CreateUserID == G.User.UserID || Model.IsPublic)
            {
                return View(Model);
            }
            else
            {
                return Json("无权查看此文档，必须是文档的创建者或此文档是公开状态！");
            }
        }
        public IActionResult Attachment_View(string DocumentID)
        {
            var Model = DB.TMT_Documents.Find(DocumentID);
            if (Model.CreateUserID == G.User.UserID || Model.IsPublic)
            {
                return View(Model);
            }
            else
            {
                return Json("无权查看此文档，必须是文档的创建者或此文档是公开状态！");
            }
        }
    }
}