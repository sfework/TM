using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    [NeedSetProject]
    public class TasksController : ControllersBase
    {
        public IActionResult Index(ParamModels.Tasks.Tasks_Index Model)
        {
            Model.List = DB.TMT_Tasks;
            return View(Model);
        }

        public IActionResult Add(string TaskID, string RequirementID)
        {
            var Model = new ParamModels.Tasks.Tasks_Add_View
            {
                TaskID = TaskID,
                RequirementID = RequirementID,
                Requirements = DB.TMT_Requirements.Where(c => c.Status == Models.DBEnums.RequirementStatus.通过)
            };
            if (!string.IsNullOrWhiteSpace(RequirementID) && !Model.Requirements.Any(c => c.RequirementID == RequirementID))
            {
                Model.RequirementID = null;
            }
            if (!string.IsNullOrWhiteSpace(TaskID))
            {
                Model.Task = DB.TMT_Tasks.Find(TaskID);
            }
            return View(Model);
        }
        [HttpPost]
        public IActionResult Publish()
        {

            return Json();
        }
    }
}