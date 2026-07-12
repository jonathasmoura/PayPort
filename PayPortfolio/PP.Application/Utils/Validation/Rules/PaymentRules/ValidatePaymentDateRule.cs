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
		// Tolerância para pequena diferença de relógio entre os servidores do banco parceiro e o nosso.
		private static readonly TimeSpan ClockTolerance = TimeSpan.FromDays(1);

		public ValidationResult Validate(PaymentRequestDto instance)
		{
			if (instance.PaymentDate == default)
				return ValidationResult.Failed("data_pagamento é obrigatória.");

			if (instance.PaymentDate > DateTime.UtcNow.Add(ClockTolerance))
				return ValidationResult.Failed("data_pagamento não pode estar no futuro.");

			return ValidationResult.Success();
		}
	}
}
