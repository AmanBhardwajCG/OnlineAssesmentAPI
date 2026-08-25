namespace OnlineAssesmentAPI.Class
{
    public class Users
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
    }


    public class Roles
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }
    }



}
