using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PP.Application.Utils.Validation
{
	public sealed class ValidationResult
	{
		private readonly List<string> _errors = new();

		public IReadOnlyList<string> Errors => _errors;

		public bool IsValid => _errors.Count == 0;

		public static ValidationResult Success() => new();

		public static ValidationResult Failed(string errorMessage)
		{
			var result = new ValidationResult();
			result.IncludeError(errorMessage);
			return result;
		}


		public void IncludeError(string errorMessage)
		{
			if (!string.IsNullOrWhiteSpace(errorMessage))
				_errors.Add(errorMessage);
		}

		public void Merge(ValidationResult other)
		{
			foreach (var erro in other.Errors)
				IncludeError(erro);
		}
	}
}