﻿using AccountOwner.Contracts;
using AccountOwner.Entities;
using AccountOwner.Extensions;
using AccountOwner.Helpers;
using AccountOwner.Models;
using System;
using System.Linq;

namespace AccountOwner.Repository
{
	public class OwnerRepository : RepositoryBase<Owner>, IOwnerRepository
	{
		private ISortHelper<Owner> _sortHelper;
		private IDataShaper<Owner> _dataShaper;

		public OwnerRepository(RepositoryContext repositoryContext, 
			ISortHelper<Owner> sortHelper,
			IDataShaper<Owner> dataShaper)
			: base(repositoryContext)
		{
			_sortHelper = sortHelper;
			_dataShaper = dataShaper;
		}

		public PagedList<ShapedEntity> GetOwners(OwnerParameters ownerParameters)
		{
			var owners = FindByCondition(o => o.DateOfBirth.Year >= ownerParameters.MinYearOfBirth &&
										o.DateOfBirth.Year <= ownerParameters.MaxYearOfBirth);

			SearchByName(ref owners, ownerParameters.Name);

			_sortHelper.ApplySort(owners, ownerParameters.OrderBy);
			var shapedOwners = _dataShaper.ShapeData(owners, ownerParameters.Fields);

			return PagedList<ShapedEntity>.ToPagedList(shapedOwners,
				ownerParameters.PageNumber,
				ownerParameters.PageSize);
		}

		private void SearchByName(ref IQueryable<Owner> owners, string ownerName)
		{
			if (!owners.Any() || string.IsNullOrWhiteSpace(ownerName))
				return;

			if (string.IsNullOrEmpty(ownerName))
				return;

			owners = owners.Where(o => o.Name.ToLowerInvariant().Contains(ownerName.Trim().ToLowerInvariant()));
		}

		public ShapedEntity GetOwnerById(Guid ownerId, OwnerParameters ownerParameters)
		{
			var owner = FindByCondition(owner => owner.Id.Equals(ownerId))
				.FirstOrDefault();

			return _dataShaper.ShapeData(owner, ownerParameters.Fields);
		}

		public Owner GetOwnerById(Guid ownerId)
		{
			return FindByCondition(owner => owner.Id.Equals(ownerId))
				.FirstOrDefault();
		}

		public void CreateOwner(Owner owner)
		{
			Create(owner);
		}

		public void UpdateOwner(Owner dbOwner, Owner owner)
		{
			dbOwner.Map(owner);
			Update(dbOwner);
		}

		public void DeleteOwner(Owner owner)
		{
			Delete(owner);
		}
	}
}