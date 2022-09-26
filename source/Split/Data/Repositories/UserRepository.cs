using Split.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Split.Data
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(AppDbContext context) : base(context)
        {

        }

        public User GetUserWithGUID(Guid userGuid)
        {
            try
            {
                var user = Context.Users.FirstOrDefault(u => u.GUID == userGuid);

                return user;
            }
            catch (NullReferenceException ex)
            {
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }


        }
    }
}
