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
    public class UserRoleRepository : Repository<UserRole>
    {
        protected override Expression<Func<UserRole, bool>> QueryById(int id)
        {
            return p => p.Id == id;
        }

        protected override Expression<Func<UserRole, bool>> QueryById(UserRole userRole)
        {
            return p => p.Id == userRole.Id;
        }

        public UserRoleRepository(TaxiHeavenContext ctx)
            : base(ctx)
        {
        }
    }
}
