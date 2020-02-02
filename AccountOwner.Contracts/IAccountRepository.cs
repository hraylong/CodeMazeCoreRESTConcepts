﻿using AccountOwner.Helpers;
using AccountOwner.Models;
using System;

namespace AccountOwner.Contracts
{
	public interface IAccountRepository : IRepositoryBase<Account>
	{
		PagedList<ShapedEntity> GetAccountsByOwner(Guid ownerId, AccountParameters parameters);
		ShapedEntity GetAccountByOwner(Guid ownerId, Guid id, string fields);
		Account GetAccountByOwner(Guid ownerId, Guid id);
	}
}
