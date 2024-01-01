using System.Net.Mail;
using System.Net;
using Microsoft.Data.Sqlite;

namespace Bewerb_Aufgabe_SB
{
    public class DBCommands
    {

        public static Statuscode Checkstatus(int id)
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
                    while (reader.Read())
                    {
                        var status = Int32.Parse(reader.GetString(0));

                        currentStatus = (Statuscode)status;
                    }
                }
                return currentStatus;
            }


        }

        //public static int 

            //returns HTTP Status Code 409 = Conflict if exists
        /*using (var connection = new SqliteConnection("Data Source=hello.db"))
{
    connection.Open();

    var command = connection.CreateCommand();
    command.CommandText =
    @"
        SELECT name
        FROM user
        WHERE id = $id
    ";
    command.Parameters.AddWithValue("$id", id);

    using (var reader = command.ExecuteReader())
    {
        while (reader.Read())
        {
            var name = reader.GetString(0);

            Console.WriteLine($"Hello, {name}!");
        }
    }
}
        */



        // smtp client after saying i wantto send email with new mail account:
        //var smtpClient = new SmtpClient("smtp.gmail.com")
        //{
        //    Port = 587,
        //    Credentials = new NetworkCredential("username", "password"),
        //    EnableSsl = true,
        //};

        //smtpClient.Send("email", "recipient", "subject", "body");

    }
}
