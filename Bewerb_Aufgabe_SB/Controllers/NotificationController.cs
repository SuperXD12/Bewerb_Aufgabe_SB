﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Text.Json;

namespace Bewerb_Aufgabe_SB.Controllers
{
    [Produces("application/json")]
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
                    return BadRequest(new Resultmessage("Something went wrong while creating the message"));
                }
                else
                {
                    return Ok(new Resultmessage("Message to " + receiver + " was successfully created"));
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
                    return NotFound(new Resultmessage("The given ID could not be found"));
                }
                else
                {
                    return Ok(new Resultmessage("Message with the index " + id + " was successfully deleted"));
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
                        return NotFound(new Resultmessage("The given ID could not be found"));
                    }

                }

            }
            if (currentStatus != Statuscode.New)
            {
                return Conflict(new Resultmessage("The message can not be canceled due to the status " + currentStatus));
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
                    return BadRequest(new Resultmessage("Something went wrong while deleting the message"));
                }
                else
                {
                    return Ok(new Resultmessage("Message with the index " + id + " was successfully deleted"));
                }



            }
        }


        [HttpPost("~/SendNotification")]

        public IActionResult SendNotification(int id)
        {
            string receiver="";
            string sender = "";
            string title = "";
            string content = "";
            Statuscode currentStatus = Statuscode.Error;
            using (var connection = new SqliteConnection("Data Source=sqlitedatabase_sb_bew.db"))
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText =
                @"
                SELECT Content, Title, Receiver, Sender, Status
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
                           content= (reader.GetString(0));
                           title= reader.GetString(1);
                           receiver= reader.GetString(2);
                           sender= reader.GetString(3);
                           currentStatus= (Statuscode)Int16.Parse(reader.GetString(4));

                        }

                    }
                    else
                    {

                        return NotFound(new Resultmessage("The given ID could not be found"));
                    }

                }

            }
            if (currentStatus != Statuscode.New) { 
                return Conflict(new Resultmessage("The message can not be send due to the status " + currentStatus));
            }
            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("restapi.send.notifications@gmail.com", "wciu oezv csps hchx"),
                EnableSsl = true,
            };
            try
            {
               smtpClient.Send("restapi.send.notifications@gmail.com", receiver, "Message sent by: " + sender + " | " + title, content);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                return BadRequest(new Resultmessage(ex.Message));

            }


            using (var connection = new SqliteConnection("Data Source=sqlitedatabase_sb_bew.db"))
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText =
                @"
                UPDATE Notifications
                SET Status= 2
                WHERE id = $id
                 ";
                command.Parameters.AddWithValue("$id", id);
                command.ExecuteNonQuery();

            }



            return Ok(new Resultmessage("Message was successfully send"));

        }

        [HttpGet("~/getMessages")] 

        public IActionResult GetMessages(string sender)
        {

            using (var connection = new SqliteConnection("Data Source=sqlitedatabase_sb_bew.db"))
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText =
                @"
                SELECT ID, Title, Content, Receiver, Sender, Status
                FROM Notifications
                WHERE Sender = $sender
                 ";
                command.Parameters.AddWithValue("$sender", sender);
                using (var reader = command.ExecuteReader())
                {
                    
                    if (reader.HasRows)
                    {
                        List<Notification> messagesbyperson = new List<Notification>();
                        while (reader.Read())
                        {
                            Person temp_receiver = new Person(reader.GetString(3));
                            Person temp_sender = new Person(reader.GetString(4));

                            messagesbyperson.Add(new Notification(Int32.Parse(reader.GetString(0)), reader.GetString(1), reader.GetString(2), temp_receiver, temp_sender, (Statuscode)Int16.Parse(reader.GetString(5))));

                        }
                        return Ok(messagesbyperson);
                    }
                    else
                    {
                        return NotFound(new Resultmessage("The given ID could not be found"));
                    }

                }

            }

        }





    }
    [Produces("application/json")]
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
                        return Ok(currentStatus);
                    }
                    else {

                        return NotFound(new Resultmessage("The given ID could not be found"));
                    }
                       
                }
                
            }
            
        }
    }


}
