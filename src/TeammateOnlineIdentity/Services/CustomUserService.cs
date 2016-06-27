using IdentityServer3.Core.Services.Default;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer3.Core.Models;
using TeammateOnlineIdentity.Database;
using TeammateOnlineIdentity.Models;
using IdentityServer3.Core.Extensions;

namespace TeammateOnlineIdentity.Services
{
    public class CustomUserService : UserServiceBase
    {
        TeammateOnlineContext dbContext;
        
        public CustomUserService(TeammateOnlineContext newDbContext)
        {
            dbContext = newDbContext;
        }
        
        public override Task AuthenticateExternalAsync(ExternalAuthenticationContext context)
        {
            var provider = context.ExternalIdentity.Provider;
            UserProfiles existingUser = null;
            switch(provider)
            {
                case "Facebook":
                    existingUser = dbContext.UserProfiles.FirstOrDefault(x => x.FacebookId == context.ExternalIdentity.ProviderId);
                    break;
                case "Google":
                    existingUser = dbContext.UserProfiles.FirstOrDefault(x => x.GoogleId == context.ExternalIdentity.ProviderId);
                    break;
            }

            if(existingUser == null)
            {
                var newUser = new UserProfiles();
                // Add other claims
                newUser.EmailAddress = context.ExternalIdentity.Claims.FirstOrDefault(x => x.Type == IdentityServer3.Core.Constants.ClaimTypes.Email).Value;
                newUser.FirstName = context.ExternalIdentity.Claims.FirstOrDefault(x => x.Type == IdentityServer3.Core.Constants.ClaimTypes.GivenName).Value;
                newUser.LastName = context.ExternalIdentity.Claims.FirstOrDefault(x => x.Type == IdentityServer3.Core.Constants.ClaimTypes.FamilyName).Value;
                newUser.CreatedDate = DateTime.UtcNow;

                switch (provider)
                {
                    case "Facebook":
                        newUser.FacebookId = context.ExternalIdentity.ProviderId;
                        break;
                    case "Google":
                        newUser.GoogleId = context.ExternalIdentity.ProviderId;
                        break;
                }

                dbContext.UserProfiles.Add(newUser);
                dbContext.SaveChanges();

                existingUser = newUser;
            }

            // Add other claims
            List<Claim> existingUserClaims = new List<Claim>();
            existingUserClaims.Add(new Claim(ClaimTypes.Email, existingUser.EmailAddress));

            context.AuthenticateResult = new AuthenticateResult(
                existingUser.Id.ToString(),
                existingUser.FirstName,
                existingUserClaims,
                provider,
                provider);

            return Task.FromResult(0);
            //return base.AuthenticateExternalAsync(context);
        }

        public override Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var currentUser = dbContext.UserProfiles.FirstOrDefault(x => x.Id.ToString() == context.Subject.GetSubjectId());

            var claims = new List<Claim>
            {
                new Claim("sub", currentUser.Id.ToString())
            };

            // Add other claims
            claims.Add(new Claim(ClaimTypes.Email, currentUser.EmailAddress));

            if(!context.AllClaimsRequested)
            {
                claims = claims.Where(x => context.RequestedClaimTypes.Contains(x.Type)).ToList();
            }

            context.IssuedClaims = claims;
            return Task.FromResult(0);
        }

        public override Task IsActiveAsync(IsActiveContext context)
        {
            if(context.Subject == null)
            {
                throw new ArgumentException("subject");
            }

            //var currentUser = dbContext.UserProfiles.FirstOrDefault(x => x.Id.ToString() == context.Subject.GetSubjectId());
            // Todo: Check active flag on the user and set it
            context.IsActive = true;

            return Task.FromResult(0);
        }
    }
}
