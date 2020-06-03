using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Capex.Web.Controllers
{
    public class MensajeController : Controller
    {
        // GET: Mensajes
        public ActionResult Index()
        {
            return View();
        }
    }
}