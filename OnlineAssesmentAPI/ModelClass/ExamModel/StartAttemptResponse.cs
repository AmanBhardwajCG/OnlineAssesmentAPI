namespace OnlineAssesmentAPI.ModelClass.ExamModel
{
        public class StartAttemptResponse
        {
          public AttemptResponse? AttemptResponse { get; set; }
        public List<ExamQuestions?> ExamQuestions { get; set; } = new();
          
        }
    public class AttemptResponse
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
    public class ExamQuestions
    {
        public int? QuestionId { get; set; }
        public string? QuestionType { get; set; }
        public string? QuestionText { get; set; }
        public int AnswerFlag { get; set; }
        public string? options { get; set; }
    }
    
}
