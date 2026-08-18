using DocumentFormat.OpenXml.Drawing.Diagrams;
using DocumentFormat.OpenXml.Office.SpreadSheetML.Y2023.MsForms;

namespace OnlineAssesmentAPI.ModelClass
{
    public class Questions
    {
        public int QuestionId { get; set; }
        public string? QuestionText { get; set; }
        public string? QuestionType { get; set; }
        public string? Difficulty { get; set; }
        public int CreatedByUserId { get; set; }
        public bool Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? Option1 { get; set; }
        public string? Option2 { get; set; }
        public string? Option3 { get; set; }
        public string? Option4 { get; set; }
        public string? CorrectAnswer { get; set; }
        public string? Topic { get; set; }
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
}
