using PP.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PP.Application.Utils.Validation.Rules.PaymentRules
{
	public sealed class ValidatePaymentDateRule : IValidationRule<PaymentRequestDto>
	{
		private static readonly TimeSpan ClockTolerance = TimeSpan.FromDays(1);

		public ValidationResult Validate(PaymentRequestDto instance)
		{
			if (instance.PaymentDate == default)
				return ValidationResult.Failed("Data Pagamento é obrigatória.");

			if (instance.PaymentDate > DateTime.UtcNow.Add(ClockTolerance))
				return ValidationResult.Failed("data Pagamento não pode estar no futuro.");

			return ValidationResult.Success();
		}
	}
}
