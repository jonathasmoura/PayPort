using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PP.Domain.Exceptions
{
	public class DuplicateTransactionException : Exception
	{
		public string IdTransacao { get; }

		public DuplicateTransactionException(string idTransacao)
			: base($"A transação '{idTransacao}' já foi registrada anteriormente.")
		{
			IdTransacao = idTransacao;
		}
	}
}
