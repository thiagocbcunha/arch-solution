using Microsoft.AspNetCore.Identity;

namespace Verx.Authentication.Service.Domain.Exceptions;


[Serializable]
public abstract class DomainException : Exception
{
	public DomainException() { }
    public DomainException(string message, IEnumerable<IdentityError> errors) => Erros = errors;
    public DomainException(string message) : base(message) { }
	public DomainException(string message, Exception inner) : base(message, inner) { }
	protected DomainException(
	  System.Runtime.Serialization.SerializationInfo info,
	  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }

	public IEnumerable<IdentityError> Erros { get; } = [];
}