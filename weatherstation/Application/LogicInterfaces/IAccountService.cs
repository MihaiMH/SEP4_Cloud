using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using weatherstation.Domain.Model;

namespace weatherstation.Application.LogicInterfaces
{
    internal interface IAccountService
    {
        Task RegisterAccount(dynamic data);
    }
}
