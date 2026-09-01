namespace OnlineAssesmentAPI.ModelClass.ExamModel
{
     public class CreateExamRequest
        {
            public string ExamName { get; set; } = string.Empty;
            public string? Description { get; set; } = string.Empty;
            public int DurationMinutes { get; set; }
            public int TotalQuestions { get; set; }
            public int MCQCount { get; set; }
            public int CodingCount { get; set; }
            //public DateTime StartAt { get; set; }
            //public DateTime EndAt { get; set; }
        }
    public class AssignExamCollegeRequest
    {
        public long ExamId { get; set; }
        public int CollegeId { get; set; }
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
    }

    
        public class EnrollStudentResponse
        {
            public long StudentId { get; set; }

            public string RollNo { get; set; } = string.Empty;

            public string Name { get; set; } = string.Empty;

            public string Batch { get; set; } = string.Empty;

            public string Course { get; set; } = string.Empty;

            public string Email { get; set; } = string.Empty;

            public string MobileNo { get; set; } = string.Empty;

            public int CollegeId { get; set; }
        }

    public class StudentExamResponse
    {
        //public long StudentId { get; set; }

        //public string Name { get; set; }
        //public string RollNumber { get; set; }

        //public string Batch { get; set; }

        //public string Course { get; set; }

        //public string Email { get; set; }

        //public string MobileNo { get; set; }

        //public int CollegeId { get; set; }
        public long ExamId { get; set; }
        
        public string ExamName { get; set; } 
        public string? Description { get; set; } 
        public int DurationMinutes { get; set; }
        public int TotalQuestions { get; set; }
        public int MCQCount { get; set; }
        public int CodingCount { get; set; }
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
        public bool ExamStarted { get; set; }
    }

    public class StudentResponse
    {
        public long StudentId { get; set; }

        public string Name { get; set; }
        public string RollNumber { get; set; }

        public string Batch { get; set; }

        public string Course { get; set; }

        public string Email { get; set; }

        public string MobileNo { get; set; }

        public int CollegeId { get; set; }
    }

    public class StudentExamDTO
    {
        public StudentResponse? Student { get; set; }
        public List<StudentExamResponse> Exam { get; set; } = new();
        public string? Message { get; set; }
    }
           

    public class ExamReview
    {
        public long ExamId { get; set; }
        public int Status { get; set; }
    }

}
