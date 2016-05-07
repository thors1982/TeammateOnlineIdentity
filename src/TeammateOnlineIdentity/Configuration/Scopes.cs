using IdentityServer3.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TeammateOnlineIdentity.Configuration
{
    public static class Scopes
    {
        public static IEnumerable<Scope> Get()
        {
            return new[]
            {
                StandardScopes.OpenId,
                StandardScopes.ProfileAlwaysInclude,
                StandardScopes.EmailAlwaysInclude,

                StandardScopes.OfflineAccess,

                // Api - access token will contain roles of user
                new Scope
                {
                    Name = "api",
                    DisplayName = "API user",
                    Description = "Allow the user to manage his/her own information via the API",
                    Type = ScopeType.Resource,
                    Claims = new List<ScopeClaim>
                    {
                        new ScopeClaim("role", false)
                    }
                },
            };
        }
    }
}
