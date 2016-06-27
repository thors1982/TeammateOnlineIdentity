using System;
using System.Collections.Generic;
using System.Linq;
using TeammateOnlineIdentity.Models;

namespace TeammateOnlineIdentity.Database.Repositories
{
    public class UserProfileRepository : IUserProfileRepository
    {
        private TeammateOnlineContext context;

        public UserProfileRepository(TeammateOnlineContext newContext)
        {
            context = newContext;
        }

        public UserProfiles Add(UserProfiles userProfile)
        {
            context.UserProfiles.Add(userProfile);
            context.SaveChanges();

            return userProfile;
        }

        public UserProfiles FinBdyId(int id)
        {
            return context.UserProfiles.FirstOrDefault(x => x.Id == id);
        }

        public UserProfiles FindByEmailAddress(string emailAddress)
        {
            return context.UserProfiles.FirstOrDefault(x => x.EmailAddress == emailAddress);
        }

        public UserProfiles FindByGoogleId(string googleId)
        {
            return context.UserProfiles.FirstOrDefault(x => x.GoogleId == googleId);
        }

        public UserProfiles FindByFacebookId(string facebookId)
        {
            return context.UserProfiles.FirstOrDefault(x => x.FacebookId == facebookId);
        }

        public IEnumerable<UserProfiles> GetAll()
        {
            return context.UserProfiles.ToList();
        }

        public void Update(UserProfiles userProfile)
        {
            context.UserProfiles.Update(userProfile);
            context.SaveChanges();
        }
    }
}
