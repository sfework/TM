using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    public class BUGController : ControllersBase
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}