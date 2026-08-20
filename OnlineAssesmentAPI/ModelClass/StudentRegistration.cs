namespace OnlineAssesmentAPI.ModelClass
{
    public class StudentRegistration
    {
        public int StudentID { get; set; }
        public string RollNumber { get; set; }
        public string CollegeName { get; set; }
        public string Course { get; set; }
        public string Batch { get; set; }
        public int CollegeID { get; set; }
    }

    public class RegisterCollegeRequest
    {
        public string CollegeName { get; set; } = string.Empty;
    }

    public class CollegeRegistrationResponse
    {
        public bool IsSuccess { get; set; }

        public string Message { get; set; } = string.Empty;

        public int? CollegeId { get; set; }
    }

    public class GetCollegebyNameID
    {
        public int CollegeID { get; set; }
        public string CollegeName { get; set; }
    }

    public class StudentExcelRow
    {
        public int RowNumber { get; set; }

        public string CollegeName { get; set; } = string.Empty;

        public string RollNo { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string Batch { get; set; } = string.Empty;

        public string Course { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string MobileNo { get; set; } = string.Empty;
    }

    public class StudentRegisterResponse
    {
        public int RowNumber { get; set; }

        public string RollNo { get; set; } = string.Empty;

        public bool IsSuccess { get; set; }

        public string Message { get; set; } = string.Empty;

        public long? StudentId { get; set; }
    }
}
