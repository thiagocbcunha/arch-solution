namespace Verx.Authentication.Service.Domain.Exceptions;


[Serializable]
public class InvalidPasswordException : DomainException
{
	public InvalidPasswordException() { }
	public InvalidPasswordException(string message) : base(message) { }
	public InvalidPasswordException(string message, Exception inner) : base(message, inner) { }
	protected InvalidPasswordException(
	  System.Runtime.Serialization.SerializationInfo info,
	  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}
