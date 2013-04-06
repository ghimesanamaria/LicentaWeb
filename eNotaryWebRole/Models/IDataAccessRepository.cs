using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eNotaryWebRole.Models
{
    public interface IDataAccessRepository
    {
         string getRole(string username);
    }
}