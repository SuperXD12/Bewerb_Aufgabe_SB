namespace Bewerb_Aufgabe_SB
{
    public class Notification
    {
        public int ID { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }

        public Person? Receiver { get; set; }

        public Person? Sender { get; set; }

        public string? SendDate { get; set; }

        public Statuscode Status { get; set; }

    }

    public class Person
    {

        public string? Mailaddress { get; set; }


    }

    public enum Statuscode : ushort{ 
        New = 1, Success = 2, Error = 0
    
    }
}
