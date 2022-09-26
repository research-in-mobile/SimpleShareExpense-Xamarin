using Split.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Split.Data
{
     public interface IUserRepository : IRepository<User>
    {
        User GetUserWithGUID(Guid userGuid);
    }
}
