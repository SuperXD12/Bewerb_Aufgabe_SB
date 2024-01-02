namespace Bewerb_Aufgabe_SB
{
    public class Notification
    {
        public int ID { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }

        public Person? Receiver { get; set; }

        public Person? Sender { get; set; }

        public Statuscode Status { get; set; }

        public Notification(int id, string title, string content, Person receiver, Person sender, Statuscode status) { 
            ID = id;
            Title = title;
            Content = content;
            Receiver = receiver;
            Sender = sender;
            Status = status;
        }

    }

    public class Person
    {

        public string? Mailaddress { get; set; }

        public Person(string mailaddress) { 
            Mailaddress = mailaddress;
        }

    }

    public enum Statuscode : ushort{ 
        New = 1, Success = 2, Error = 0
    
    }

    public class Resultmessage
    { 
        public string? Message { get; set; }

        public Resultmessage(string message)
        {
            Message = message;
        }
    }
}
