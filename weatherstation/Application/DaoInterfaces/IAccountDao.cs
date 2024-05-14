using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace weatherstation.Application.DaoInterfaces
{
    internal interface IAccountDao
    {
        Task RegisterAccount(User user);
    }
}
