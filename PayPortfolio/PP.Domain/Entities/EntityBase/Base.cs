using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PP.Domain.Entities.EntityBase
{
	public class Base
	{
		public Base()
		{
			Id = Guid.NewGuid();
			IsActive = true;
			Created = DateTime.UtcNow;
			ActivationDate = Created;
		}

		public Guid Id { get; set; }
		public bool IsActive { get; set; }
		public DateTime? ActivationDate { get; set; }
		public DateTime? InactivationDate { get; set; }
		public DateTime? UpdatedAt { get; set; }
		public DateTime Created { get; set; }

		public virtual void Activate()
		{
			if (IsActive) return;

			IsActive = true;
			ActivationDate = DateTime.UtcNow;
			InactivationDate = null;
			RegisterUpdate();
		}

		public virtual void Inactivate()
		{
			if (!IsActive) return;

			IsActive = false;
			InactivationDate = DateTime.UtcNow;
			RegisterUpdate();
		}

		protected void RegisterUpdate() => UpdatedAt = DateTime.UtcNow;
	}
}
