using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;

namespace ZadatakNeki.Repository
{
    interface IRepository<TEntitet>
    {
        List<TEntitet> DajSveEntitete();
        TEntitet EntitetPoId(long id);
        TEntitet SacuvajEntitet(TEntitet entitet);
    }
}
