using System;
using System.Collections.Generic;
using System.Text;

namespace AccountOwner.Helpers
{
    public interface IEntity
    {
        Guid Id { get; set; }
    }
}
