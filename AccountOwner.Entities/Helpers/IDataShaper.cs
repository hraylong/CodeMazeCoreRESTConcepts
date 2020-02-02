using AccountOwner.Models;
using System.Collections.Generic;

namespace AccountOwner.Helpers
{
	public interface IDataShaper<T>
	{
		IEnumerable<ShapedEntity> ShapeData(IEnumerable<T> entities, string fieldsString);
		ShapedEntity ShapeData(T entity, string fieldsString);
	}
}