using PP.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PP.Application.Utils.Validation.Rules.PaymentRules
{
	public sealed class IdRequiredContractRule : IValidationRule<PaymentRequestDto>
	{
		public ValidationResult Validate(PaymentRequestDto instance)
		{
			return string.IsNullOrWhiteSpace(instance.IdContract)
				? ValidationResult.Failed("id_contrato é obrigatório.")
				: ValidationResult.Success();
		}
	}
}
