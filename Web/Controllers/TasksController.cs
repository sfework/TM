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
            return View(Model);
        }
    }
}