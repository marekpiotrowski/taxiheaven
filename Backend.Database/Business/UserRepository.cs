using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Backend.Database.Base;
using Backend.Database.Model;

namespace Backend.Database.Business
{
    public class UserRepository : Repository<User>
    {
        protected override Expression<Func<User, bool>> QueryById(int id)
        {
            return p => p.Id == id;
        }

        protected override Expression<Func<User, bool>> QueryById(User user)
        {
            return p => p.Id == user.Id;
        }

        public UserRepository(TaxiHeavenContext ctx) : base(ctx)
        {
        }
    }
}
