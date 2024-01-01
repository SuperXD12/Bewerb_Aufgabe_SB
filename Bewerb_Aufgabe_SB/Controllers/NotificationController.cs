/*using Microsoft.AspNetCore.Mvc;

namespace Bewerb_Aufgabe_SB.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NotificationController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }

    public class StatusCodeController : ControllerBase
    {
        [HttpPost("{id}", Name = "GetStatusByID")]

        public IActionResult GetStatusByID(int id)
        {

            return Ok(DBCommands.Checkstatus(id));
        }
    }
}*/
