using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;

namespace Bewerb_Aufgabe_SB.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NotificationController : ControllerBase
    {


        //returns HTTP Status Code 409 = Conflict if exists
        //return Conflict("Conflict Error - Your inserted id was wrong");
    }

    public class StatusCodeController : ControllerBase
    {
        [HttpPost("{id}", Name = "GetStatusByID")]

        public IActionResult GetStatusByID(int id)
        {
            Statuscode currentStatus = Statuscode.Error;
            using (var connection = new SqliteConnection("Data Source=sqlitedatabase_sb_bew.db"))
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText =
                @"
                SELECT Status
                FROM Notifications
                WHERE id = $id
                 ";
                command.Parameters.AddWithValue("$id", id);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            var status = UInt16.Parse(reader.GetString(0));
                            if (Enum.IsDefined(typeof(Statuscode), status))
                            {
                                currentStatus = (Statuscode)status;
                            }
                            //else automatically sending error status | this should be prevented by only correctly altering statuscodes 
                            
                        }
                        return Ok(currentStatus.ToString());
                    }
                    else {
                        return NotFound("404 Not Found | The given ID could not be found");
                    }
                       
                }
                
            }
            
        }
    }


}
