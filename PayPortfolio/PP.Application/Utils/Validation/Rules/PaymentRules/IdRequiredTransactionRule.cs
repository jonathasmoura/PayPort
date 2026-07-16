using PP.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PP.Application.Utils.Validation.Rules.PaymentRules
{
	public sealed class IdRequiredTransactionRule : IValidationRule<PaymentRequestDto>
	{
		public ValidationResult Validate(PaymentRequestDto instance)
		{
			return string.IsNullOrWhiteSpace(instance.IdTransaction)
				? ValidationResult.Failed("id_transacao é obrigatório.")
				: ValidationResult.Success();
		}
	}
}
