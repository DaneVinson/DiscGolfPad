using Dgp.Domain.Core;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dgp.Service.Rest
{
    public static class Extensions
    {
        public static string GetPlayerId(this ControllerBase controller)
        {
            return Bilbo.Id;
        }
    }
}
