using Microsoft.AspNetCore.Identity;

namespace Verx.Authentication.Service.Domain.Exceptions;


[Serializable]
public class ValidationUserException : DomainException
{
	public ValidationUserException() { }
	public ValidationUserException(string message, IEnumerable<IdentityError> errors) : base(message, errors) { }
	public ValidationUserException(string message, Exception inner) : base(message, inner) { }
	protected ValidationUserException(
	  System.Runtime.Serialization.SerializationInfo info,
	  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}
