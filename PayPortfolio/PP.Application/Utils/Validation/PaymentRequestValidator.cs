using System;
using System.Collections.Generic;
using PP.Application.DTOs;

namespace PP.Application.Utils.Validation
{
	public class PaymentRequestValidator : IValidator<PaymentRequestDto>
	{
		public ValidationResult Validate(PaymentRequestDto request)
		{
			var errors = new List<string>();

			if (request is null)
			{
				var result = new ValidationResult();
				result.IncludeError("Payload é nulo.");
				return result;
			}

			if (string.IsNullOrWhiteSpace(request.IdTransaction))
				errors.Add("IdTransaction é obrigatório.");

			if (string.IsNullOrWhiteSpace(request.IdContract))
				errors.Add("IdContract é obrigatório.");

			if (request.Amount <= 0m)
				errors.Add("Amount deve ser maior que zero.");

			if (request.PaymentDate == default)
				errors.Add("PaymentDate inválido.");

			if (string.IsNullOrWhiteSpace(request.Status))
				errors.Add("Status é obrigatório.");

			if (errors.Count == 0)
				return ValidationResult.Success();

			var finalResult = new ValidationResult();
			foreach (var err in errors)
				finalResult.IncludeError(err);

			return finalResult;
		}
	}
}
