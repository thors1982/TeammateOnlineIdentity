using IdentityServer3.Core.Services.InMemory;
using IdentityServer3.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;

namespace TeammateOnlineIdentity.Configuration
{
    public class Users
    {
        public static List<InMemoryUser> Get()
        {
            return new List<InMemoryUser>()
            {
                new InMemoryUser
                {
                    Username = "ironman",
                    Password = "ironman",
                    Subject = "1",
                    Claims = new[]
                    {
                        new Claim(Constants.ClaimTypes.GivenName, "Tony"),
                        new Claim(Constants.ClaimTypes.FamilyName, "Stark"),
                        new Claim(Constants.ClaimTypes.Email, "tony.stark@ironman.com"),
                        new Claim("role", "default")
                    }
                },
                new InMemoryUser
                {
                    Username = "captainamerica",
                    Password = "captainamerica",
                    Subject = "2",
                    Claims = new[]
                    {
                        new Claim(Constants.ClaimTypes.GivenName, "Steve"),
                        new Claim(Constants.ClaimTypes.FamilyName, "Rogers"),
                        new Claim(Constants.ClaimTypes.Email, "steve.rogers@captainamerica.com"),
                        new Claim("role", "default")
                    }
                },
                new InMemoryUser
                {
                    Username = "hulk",
                    Password = "hulk",
                    Subject = "3",
                    Claims = new[]
                    {
                        new Claim(Constants.ClaimTypes.GivenName, "Bruce"),
                        new Claim(Constants.ClaimTypes.FamilyName, "Banner"),
                        new Claim(Constants.ClaimTypes.Email, "bruce.banner@hulk.com"),
                        new Claim("role", "default")
                    }
                }
            };
        }
    }
}
