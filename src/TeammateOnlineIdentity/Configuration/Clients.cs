using IdentityServer3.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TeammateOnlineIdentity.Configuration
{
    public class Clients
    {
        public static List<Client> Get()
        {
            return new List<Client>()
            {
                new Client
                {
                    ClientId = "TeammateOnlineUi",
                    ClientName = "TeammateOnlineUi",
                    Flow = Flows.Implicit,
                    //IdentityTokenLifetime = 10,
                    //AccessTokenLifetime = 120,
                    IdentityTokenLifetime = 300,  // default 300
                    AccessTokenLifetime = 3600,   // default 3600
                    RedirectUris = new List<string>
                    {
                        "http://localhost:4200/auth/callback",
                        // for silent refresh
                        "http://localhost:4200/auth/silentrefresh",
                    },
                    PostLogoutRedirectUris = new List<string>
                    {
                        "http://localhost:4200/"
                    },
                    AllowAccessToAllScopes = true,
                    AllowedCorsOrigins = new List<string>
                    {
                        "http://localhost:4200"
                    }
                }
            };
        }
    }
}
