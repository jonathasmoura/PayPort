using PP.Domain.Entities.EntityBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PP.Domain.Entities
{
	public class ContractStatus : Base
	{
		public string IdContrato { get; private set; } = string.Empty;
		public string StatusAtual { get; private set; } = string.Empty;
		public decimal ValorPago { get; private set; }
		public DateTime DataUltimoPagamento { get; private set; }
		public string UltimoIdTransacao { get; private set; } = string.Empty;

		private ContractStatus() { }

		public static ContractStatus CriarNovo(
			string idContrato, string status, decimal valor, DateTime dataPagamento, string idTransacao)
		{
			return new ContractStatus
			{
				IdContrato = idContrato,
				StatusAtual = status,
				ValorPago = valor,
				DataUltimoPagamento = dataPagamento,
				UltimoIdTransacao = idTransacao
			};
		}
		public void AtualizarComNovoPagamento(string status, decimal valor, DateTime dataPagamento, string idTransacao)
		{
			if (dataPagamento < DataUltimoPagamento)
				return; 

			StatusAtual = status;
			ValorPago = valor;
			DataUltimoPagamento = dataPagamento;
			UltimoIdTransacao = idTransacao;
			RegistrarAtualizacao(); 
		}
	}
}
