using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace weatherstation.Application.LogicInterfaces
{
    internal interface IAccountService
    {
        Task RegisterAccount(User user);
    }
}
