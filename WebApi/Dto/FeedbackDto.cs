using System.Collections;

namespace WebApi.Dto;

public class FeedbackDto
{
    private int quizId;
    private int userId;
    private int totalQuestions;
    private IEnumerable answers;
}