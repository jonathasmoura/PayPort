using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PP.Application.Utils.Validation
{
	public interface IValidationRule<in T>
	{
		ValidationResult Validate(T instance);
	}

	public interface IValidator<in T>
	{
		ValidationResult Validate(T instance);
	}
}
