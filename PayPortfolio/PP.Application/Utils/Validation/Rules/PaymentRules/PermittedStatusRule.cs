using PP.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PP.Application.Utils.Validation.Rules.PaymentRules
{
	public sealed class PermittedStatusRule : IValidationRule<PaymentRequestDto>
	{
		private static readonly HashSet<string> PermittedStatus = new(StringComparer.OrdinalIgnoreCase)
		{
			"SETTLED", "PENDING", "CANCELLED", "REVERSED"
		};

		public ValidationResult Validate(PaymentRequestDto instance)
		{
			if (string.IsNullOrWhiteSpace(instance.Status))
				return ValidationResult.Failed("status é obrigatório.");

			return PermittedStatus.Contains(instance.Status)
				? ValidationResult.Success()
				: ValidationResult.Failed(
					$"status '{instance.Status}' não reconhecido. Valores aceitos: {string.Join(", ", StatusPermitidos)}.");
		}
	}
}
