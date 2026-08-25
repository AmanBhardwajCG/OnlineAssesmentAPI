using DocumentFormat.OpenXml.Drawing.Diagrams;
using DocumentFormat.OpenXml.Office.SpreadSheetML.Y2023.MsForms;

namespace OnlineAssesmentAPI.ModelClass
{
    public class Question
    {
        public int QuestionId { get; set; }
        public string? QuestionText { get; set; }
        public string? QuestionType { get; set; }
        public string? Difficulty { get; set; }
        public int CreatedByUserId { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? Option1 { get; set; }
        public string? Option2 { get; set; }
        public string? Option3 { get; set; }
        public string? Option4 { get; set; }
        public string? CorrectAnswer { get; set; }
        public string? Topic { get; set; }
        //public long QuestionId { get; set; }

        //public string QuestionText { get; set; } = string.Empty;

        //public string QuestionType { get; set; } = string.Empty;

        //public string? Difficulty { get; set; }

        //public long CreatedByUserId { get; set; }

        //public string Status { get; set; } = "Draft";
        //public List<CreateQuestionOptionRequest> Options { get; set; }
        //  = new();
    }

    public class CodingQuestion
    {
        public int QuestionId { get; set; }
        public string? ProblemStatement { get; set; }
        public string? InputDescription { get; set; }
        public string? OutputDescription { get; set; }
        public string? Constraints { get; set; }
        public string? FunctionParameterType { get; set; }
        public string? FunctionReturnType { get; set; }
        public string? StarterCode { get; set; }
        public string? Difficulty { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? Status { get; set; }
        // public string? IsActive { get; set; }

    }

    //public class CreateQuestionRequest
    //{
    //    public string QuestionText { get; set; } = string.Empty;
    //    public string QuestionType { get; set; } = string.Empty;
    //    public string? Difficulty { get; set; }
    //    public List<CreateQuestionOptionRequest> Options { get; set; }
    //        = new();

    //}
    //public class QuestionOption
    //{
    //    public long OptionId { get; set; }

    //    public long QuestionId { get; set; }

    //    public string OptionText { get; set; } = string.Empty;

    //    public bool IsCorrect { get; set; } = false;
    //}


    //public class CreateQuestionOptionRequest
    //{
    //    public string OptionText { get; set; } = string.Empty;

    //    public bool IsCorrect { get; set; }
    //}

    //public class QuestionReview
    //{
    //    public long QuestionId { get; set; }
    //    public string Status { get; set; }
    //}

    //public class MCQQuestion
    //{

    //}
}

