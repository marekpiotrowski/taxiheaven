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
    public class CarRepository : Repository<Car>
    {
        protected override Expression<Func<Car, bool>> QueryById(int id)
        {
            return p => p.Id == id;
        }

        protected override Expression<Func<Car, bool>> QueryById(Car car)
        {
            return p => p.Id == car.Id;
        }

        public CarRepository(TaxiHeavenContext ctx)
            : base(ctx)
        {
        }
    }
}
