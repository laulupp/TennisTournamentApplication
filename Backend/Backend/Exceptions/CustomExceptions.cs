using System.Net;

namespace Backend.Exceptions;

public class CustomExceptions
{
    public abstract class ApiException : Exception
    {
        public HttpStatusCode StatusCode { get; }

        protected ApiException(string message, HttpStatusCode statusCode) : base(message)
        {
            StatusCode = statusCode;
        }
    }

    public class UserNotFoundException : ApiException
    {
        public UserNotFoundException(string user = "User") : base(user + " not found", HttpStatusCode.NotFound) { }
    }

    public class InvalidPasswordException : ApiException
    {
        public InvalidPasswordException() : base("Invalid password", HttpStatusCode.Unauthorized) { }
    }

    public class UserAlreadyExistsException : ApiException
    {
        public UserAlreadyExistsException() : base("User already exists", HttpStatusCode.BadRequest) { }
    }

    public class EmailAlreadyRegisteredException : ApiException
    {
        public EmailAlreadyRegisteredException() : base("Email already registered", HttpStatusCode.BadRequest) { }
    }

    public class UserUnauthorizedException : ApiException
    {
        public UserUnauthorizedException() : base("You are not authorized", HttpStatusCode.Unauthorized) { }
    }

    public class InvalidAuthentication : ApiException
    {
        public InvalidAuthentication() : base("The token is invalid", HttpStatusCode.Unauthorized) { }
    }

    public class TournamentNotFoundException : ApiException
    {
        public TournamentNotFoundException() : base("Tournament not found", HttpStatusCode.NotFound) { }
    }

    public class MatchNotFoundException : ApiException
    {
        public MatchNotFoundException() : base("Match not found", HttpStatusCode.NotFound) { }
    }

    public class UserAlreadyRegisteredInTournamentException : ApiException
    {
        public UserAlreadyRegisteredInTournamentException() : base("You are already registered in the tournament", HttpStatusCode.BadRequest) { }
    }

    public class SamePlayersExceptions : ApiException
    {
        public SamePlayersExceptions() : base("The players are the same", HttpStatusCode.BadRequest) { }
    }

    public class LinkageNotFoundException : ApiException
    {
        public LinkageNotFoundException() : base("The request was not found", HttpStatusCode.BadRequest) { }
    }
}
