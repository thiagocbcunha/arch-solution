namespace Verx.Authentication.Service.Domain.Exceptions;


[Serializable]
public class InvalidUserTokenException : DomainException
{
	public InvalidUserTokenException() { }
	public InvalidUserTokenException(string message) : base(message) { }
	public InvalidUserTokenException(string message, Exception inner) : base(message, inner) { }
	protected InvalidUserTokenException(
	  System.Runtime.Serialization.SerializationInfo info,
	  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}