using System;

namespace PP.Domain.Exceptions
{
	public class DuplicateTransactionException : Exception
	{
		public string IdTransaction { get; }

		public DuplicateTransactionException(string idTransaction)
			: base($"A transação '{idTransaction}' já foi registrada anteriormente.")
		{
			IdTransaction = idTransaction;
		}
	}
}
