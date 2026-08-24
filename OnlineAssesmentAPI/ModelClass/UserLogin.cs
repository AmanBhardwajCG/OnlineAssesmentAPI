namespace OnlineAssesmentAPI.ModelClass
{
    public class UserLogin
    {
        public class UserLoginRequest
        {
            public string Email { get; set; }
            public string Password { get; set; }    
        }

        public class LoginResponse 
        {
            public string Message { get; set; }
            public long Userid { get; set; }
            public string Name { get; set; }
            public string Email { get; set; }
            public bool IsActive { get; set; }
            public string RoleName { get; set; }
            public string Token { get; set; }
        }

        public class GetUsers
        {
            public long Userid { get; set; }
            public string Name { get; set; }
            public string Email { get; set; }
            public bool IsActive { get; set; }
            public string RoleName { get; set; }
        }
    }
}
