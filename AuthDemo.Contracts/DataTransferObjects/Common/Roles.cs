using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthDemo.Contracts.DataTransferObjects.Common
{
    public enum Roles : long
    {
        Administrator = 1L,
        Manager = 2L,
        Employee = 3L
    }
}
