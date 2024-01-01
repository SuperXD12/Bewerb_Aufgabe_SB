using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using System.Reflection;

namespace Bewerb_Aufgabe_SB.Controllers
{
    [ApiController]
    [Route("[controller]")]
    

    public class NotificationsController : ControllerBase
    {
        [HttpPost("~/CreateNotification")]

        public IActionResult CreateNotification(string content, string title, string receiver, string sender)
        {



            using (var connection = new SqliteConnection("Data Source=sqlitedatabase_sb_bew.db"))
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText =
                            @"
                INSERT INTO Notifications (Content, Title, Receiver, Sender, Status)
                VALUES ($content, $title, $receiver, $sender, $status)
                 ";
                command.Parameters.AddWithValue("$content", content);
                command.Parameters.AddWithValue("$title", title);
                command.Parameters.AddWithValue("$receiver", receiver);
                command.Parameters.AddWithValue("$sender", sender);
                command.Parameters.AddWithValue("$status", 1);
                int exe = command.ExecuteNonQuery();
                if (exe <= 0)
                {
                    return BadRequest("Something went wrong while creating the message");
                }
                else
                {
                    return Ok("Message to " + receiver + " was successfully created");
                }



            }
        }

        [HttpDelete("~/DeleteNotification")]
        public IActionResult DeleteNotification(int id)
        {



            using (var connection = new SqliteConnection("Data Source=sqlitedatabase_sb_bew.db"))
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText =
                            @"
                DELETE FROM Notifications WHERE ID = $id
                 ";
                command.Parameters.AddWithValue("$id", id);

                int exe = command.ExecuteNonQuery();
                if (exe <= 0)
                {
                    return NotFound("The given ID could not be found");
                }
                else
                {
                    return Ok("Message with the index " + id + " was successfully deleted");
                }



            }
        }

        [HttpDelete("~/CancelNotification")]
        public IActionResult CancelNotification(int id)
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
                        
                    }
                    else
                    {
                        return NotFound("The given ID could not be found");
                    }

                }

            }
            if (currentStatus != Statuscode.New) { 
                return Conflict("The message can not be canceled due to the status"+ currentStatus);
            }

            using (var connection = new SqliteConnection("Data Source=sqlitedatabase_sb_bew.db"))
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText =
                            @"
                DELETE FROM Notifications WHERE ID = $id
                 ";
                command.Parameters.AddWithValue("$id", id);

                int exe = command.ExecuteNonQuery();
                if (exe <= 0)
                {
                    return BadRequest("Something went wrong while deleting the message");
                }
                else
                {
                    return Ok("Message with the index " + id + " was successfully deleted");
                }



            }
        }
    }





    /*public static int GetPersonByName(string address) {
            
            
        using (var connection = new SqliteConnection("Data Source=sqlitedatabase_sb_bew.db"))
        {
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText =
                        @"
            SELECT ID
            FROM Persons
            WHERE Mailaddress = $address
                ";
            command.Parameters.AddWithValue("$address", address);

            using (var reader = command.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                       return Int32.Parse(reader.GetString(0));
                        
                    }
                    
                }
                return -1;
                

            }

            }
        }

        public static string GetPersonByID(int id)
        {


            using (var connection = new SqliteConnection("Data Source=sqlitedatabase_sb_bew.db"))
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText =
                            @"
            SELECT Mailaddress
            FROM Persons
            WHERE ID = $id
                ";
                command.Parameters.AddWithValue("$id", id);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            return reader.GetString(0);

                        }

                    }
                    return 


                }

            }
        }
    */



        public class StatusCodeController : ControllerBase
    {
        [HttpGet("~/getStatuscode")] //"{id}", Name = "GetStatusByID"

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
