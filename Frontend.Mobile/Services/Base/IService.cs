using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frontend.Mobile.Services.Base
{
    public interface IService<T>
    {
        T Get(int id);
        List<T> Get();
        T Post(T model);
        T Put(T model);
        int Delete(int id);
    }
}
