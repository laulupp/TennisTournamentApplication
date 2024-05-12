namespace Backend;

public class Constants
{
    public static class Headers
    {
        public const string Username = "X-API-Username";
        public const string Token = "X-API-Token";
    }

    public static class Items
    {
        public const string IsAdmin = "IsAdmin";
        public const string IsReferee = "IsReferee";
    }

    public static class Permissions
    {
        public const string Admin = "Admin";
        public const string Player = "Player";
        public const string Referee = "Referee";
    }

    public static class EmailMessages
    {
        public const string ApprovalSubject = "Request to join tournament approved";
        public const string RejectionSubject = "Request to join tournament denied";

        public static string ApprovalMessage(string firstName, string lastName, string tournamentName)
        {
            return $"Dear {firstName} {lastName},\n\nYour request to join {tournamentName} was accepted.\nGood luck and have fun!\n\nTennis Tournament Team";
        }

        public static string RejectionMessage(string firstName, string lastName, string tournamentName)
        {
            return $"Dear {firstName} {lastName},\n\nUnfortunately, your request to join {tournamentName} was denied.\nYou could also try to join other tournaments.\n\nTennis Tournament Team";
        }
    }
}
