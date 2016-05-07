using System.Collections.Generic;
using TeammateOnlineIdentity.Models;

namespace TeammateOnlineIdentity.Database.Repositories
{
    public interface IUserProfileRepository
    {
        UserProfile Add(UserProfile userProfile);
        UserProfile FinBdyId(int id);
        UserProfile FindByEmailAddress(string emailAddress);
        UserProfile FindByGoogleId(string googleId);
        UserProfile FindByFacebookId(string facebookId);
        IEnumerable<UserProfile> GetAll();
        void Update(UserProfile userProfile);
    }
}
