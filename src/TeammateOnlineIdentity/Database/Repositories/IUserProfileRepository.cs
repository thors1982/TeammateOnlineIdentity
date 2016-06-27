using System.Collections.Generic;
using TeammateOnlineIdentity.Models;

namespace TeammateOnlineIdentity.Database.Repositories
{
    public interface IUserProfileRepository
    {
        UserProfiles Add(UserProfiles userProfile);
        UserProfiles FinBdyId(int id);
        UserProfiles FindByEmailAddress(string emailAddress);
        UserProfiles FindByGoogleId(string googleId);
        UserProfiles FindByFacebookId(string facebookId);
        IEnumerable<UserProfiles> GetAll();
        void Update(UserProfiles userProfile);
    }
}
