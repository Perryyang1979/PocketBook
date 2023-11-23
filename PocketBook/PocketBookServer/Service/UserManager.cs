namespace PocketBookServer.Services
{
    public class UsersManager
    {
        public List<UserPerson>? UserPerson { get; set; }
        public UsersManager()
        {
            UserPerson = new List<UserPerson>();

            UserPerson.Add(new UserPerson() { UserId = 1, UserName = "Perry", UserPassword = "1234" });
            UserPerson.Add(new UserPerson() { UserId = 2, UserName = "Sammy", UserPassword = "1234" });
            UserPerson.Add(new UserPerson() { UserId = 3, UserName = "Ivar", UserPassword = "1234" });
            UserPerson.Add(new UserPerson() { UserId = 4, UserName = "Jenny", UserPassword = "1234" });
        }
    }
}
