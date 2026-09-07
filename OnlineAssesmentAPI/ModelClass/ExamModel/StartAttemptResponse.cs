namespace OnlineAssesmentAPI.ModelClass.ExamModel
{
        public class StartAttemptResponse
        {
            public bool IsSuccess { get; set; }
            public string Message { get; set; } = string.Empty;

            public long? AttemptId { get; set; }
            public long? ExamId { get; set; }
            public long? StudentId { get; set; }

            public DateTime? StartedAt { get; set; }
            public DateTime? EndAt { get; set; }

            public string? Status { get; set; }
        }
    
}
