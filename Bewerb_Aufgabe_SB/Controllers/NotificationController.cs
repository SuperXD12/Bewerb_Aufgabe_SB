using Microsoft.AspNetCore.Mvc;

namespace Bewerb_Aufgabe_SB.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NotificationController : ControllerBase
    {
        
    }

    public class StatusCodeController : ControllerBase
    {
        [HttpPost("{id}", Name = "GetStatusByID")]

        public IActionResult GetStatusByID(int id)
        {

            return Ok(DBCommands.Checkstatus(id));
        }
    }
}
