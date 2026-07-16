using PP.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PP.Application.Utils.Validation.Rules.PaymentRules
{
	public sealed class ValueGreaterThanZeroRule : IValidationRule<PaymentRequestDto>
	{
		public ValidationResult Validate(PaymentRequestDto instance)
		{
			return instance.Amount<= 0
				? ValidationResult.Failed("valor deve ser maior que zero.")
				: ValidationResult.Success();
		}
	}
}
