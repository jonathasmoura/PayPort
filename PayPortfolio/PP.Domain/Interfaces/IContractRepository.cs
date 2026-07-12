using PP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PP.Domain.Interfaces
{
	public interface IContractRepository
	{
		Task<ContractStatus?> GetByContractAsync(string idContract, CancellationToken ct = default);
		Task IncludeAsync(ContractStatus status, CancellationToken ct = default);
		Task UpdateAsync(ContractStatus status, CancellationToken ct = default);
		Task<IReadOnlyList<ContractStatus>> GetAllAsync(CancellationToken ct = default);
	}
}
