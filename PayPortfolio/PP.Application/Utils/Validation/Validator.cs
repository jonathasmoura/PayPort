using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PP.Application.Utils.Validation
{
	public sealed class Validator<T> : IValidator<T>
	{
		private readonly IEnumerable<IValidationRule<T>> _rules;

		public Validator(IEnumerable<IValidationRule<T>> rules)
		{
			_rules = rules;
		}

		public ValidationResult Validate(T instance)
		{
			var result = new ValidationResult();

			foreach (var rules in _rules)
				result.Merge(rules.Validate(instance));

			return result;
		}
	}
}
