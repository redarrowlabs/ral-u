using System.Web.Mvc;
using autofac110.mvc.Models;
using autofac110.shared.HelloWorld;

namespace autofac110.mvc.Controllers
{
    public class HelloController : Controller
    {
        // GET: Hello
        public ActionResult Index()
        {
            return View(new HelloModel
            {
	            Message = "Hello World!"
            });
        }
    }
}