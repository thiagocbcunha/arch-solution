namespace Verx.Authentication.Service.Domain.Exceptions;


[Serializable]
public class UserAlreadyExistsException : DomainException
{
	public UserAlreadyExistsException() { }
	public UserAlreadyExistsException(string message) : base(message) { }
	public UserAlreadyExistsException(string message, Exception inner) : base(message, inner) { }
	protected UserAlreadyExistsException(
	  System.Runtime.Serialization.SerializationInfo info,
	  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}