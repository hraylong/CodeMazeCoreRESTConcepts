using AccountOwner.Contracts;
using AccountOwner.Entities.Models;
using AccountOwner.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AccountOwner.ApiServer.Controllers
{
	[Route("api/owner/{ownerId}/account")]
	[ApiController]
	public class AccountController : ControllerBase
	{
		private ILoggerManager _logger;
		private IRepositoryWrapper _repository;
		private LinkGenerator _linkGenerator;

		public AccountController(ILoggerManager logger,
			IRepositoryWrapper repository,
			LinkGenerator linkGenerator)
		{
			_logger = logger;
			_repository = repository;
			_linkGenerator = linkGenerator;
		}

		[HttpGet]
		public IActionResult GetAccountsForOwner(Guid ownerId, [FromQuery] AccountParameters parameters)
		{
			var accounts = _repository.Account.GetAccountsByOwner(ownerId, parameters);

			var metadata = new
			{
				accounts.TotalCount,
				accounts.PageSize,
				accounts.CurrentPage,
				accounts.TotalPages,
				accounts.HasNext,
				accounts.HasPrevious
			};

			Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

			_logger.LogInfo($"Returned {accounts.TotalCount} accounts from database.");

			var shapedAccounts = accounts.Select(o => o.Entity).ToList();

			//foreach (var account in shapedAccounts)
			//{
			//	var someValue = account.Values.ToList();
			//	var someKey = account.Keys.ToList();
			//	var accountProperties = new Dictionary<string, object>();
			//	for (int index = 0; index < someKey.Count; index++)
			//	{
			//		account.Add(someKey[index], someValue[index]);
			//	}
			//	var accountId = (Guid)accountProperties.GetValueOrDefault("Id");
			//	var accountLinks = CreateLinksForAccount(ownerId, accountId, parameters.Fields);
			//	account.Add("Links", accountLinks);
			//}

			for (var index = 0; index < accounts.Count(); index++)
			{
				var accountLinks = CreateLinksForAccount(ownerId, accounts[index].Id, parameters.Fields);
				shapedAccounts[index].Add("Links", accountLinks);
			}

			var accountWrapper = new LinkCollectionWrapper<Entity>(shapedAccounts);

			return Ok(CreateLinksForAccounts(accountWrapper));
		}

		[HttpGet("{id}")]
		public IActionResult GetAccountForOwner(Guid ownerId, Guid id, [FromQuery] string fields)
		{
			var account = _repository.Account.GetAccountByOwner(ownerId, id, fields);

			if (account.Id == Guid.Empty)
			{
				_logger.LogError($"Account with id: {id}, hasn't been found in db.");
				return NotFound();
			}

			account.Entity.Add("Links", CreateLinksForAccount(ownerId, id, fields));

			return Ok(account);
		}

		private List<Link> CreateLinksForAccount(Guid ownerId, Guid id, string fields = "")
		{
			var links = new List<Link>
			{
				new Link(_linkGenerator.GetUriByAction(HttpContext, nameof(GetAccountForOwner), values: new { ownerId, id, fields }), "self", "GET"),
			};

			return links;
		}

		private LinkCollectionWrapper<Entity> CreateLinksForAccounts(LinkCollectionWrapper<Entity> accountsWrapper)
		{
			accountsWrapper.Links.Add(new Link(_linkGenerator.GetUriByAction(HttpContext, nameof(GetAccountsForOwner), values: new { }), "self", "GET"));

			return accountsWrapper;
		}
	}
}
