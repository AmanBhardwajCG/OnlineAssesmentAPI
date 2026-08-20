namespace OnlineAssesmentAPI.ModelClass
{
    public class Question
    {
        public long QuestionId { get; set; }

        public string QuestionText { get; set; } = string.Empty;

        public string QuestionType { get; set; } = string.Empty;

        public string? Difficulty { get; set; }

        public long CreatedByUserId { get; set; }

        public string Status { get; set; } = "Draft";
        public List<CreateQuestionOptionRequest> Options { get; set; }
          = new();
    }

    public class CreateQuestionRequest
    {
        public string QuestionText { get; set; } = string.Empty;
        public string QuestionType { get; set; } = string.Empty;
        public string? Difficulty { get; set; }
        public List<CreateQuestionOptionRequest> Options { get; set; }
            = new();

    }
    public class QuestionOption
    {
        public long OptionId { get; set; }

        public long QuestionId { get; set; }

        public string OptionText { get; set; } = string.Empty;

        public bool IsCorrect { get; set; } = false;
    }


    public class CreateQuestionOptionRequest
    {
        public string OptionText { get; set; } = string.Empty;

        public bool IsCorrect { get; set; }
    }

    public class QuestionReview
    {
        public long QuestionId { get; set; }
        public string Status { get; set; }
    }

    public class MCQQuestion
    {

    }
}

