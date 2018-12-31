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
    public class BasementRepository : Repository<Basement>
    {
        protected override Expression<Func<Basement, bool>> QueryById(int id)
        {
            return p => p.Id == id;
        }

        protected override Expression<Func<Basement, bool>> QueryById(Basement basement)
        {
            return p => p.Id == basement.Id;
        }

        public BasementRepository(TaxiHeavenContext ctx)
            : base(ctx)
        {
        }
    }
}
