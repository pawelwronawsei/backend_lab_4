using ApplicationCore.Interfaces.AdminService;
using ApplicationCore.Models.QuizAggregate;
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using WebApi.Dto;

namespace WebApi.Controllers;

public class ApiQuizAdminController : Controller
{
    private readonly IQuizAdminService _service;
    private readonly IMapper _mapper;
    
    public ApiQuizAdminController(IQuizAdminService service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }
    
    //GET
    public IActionResult Index()
    {
        return View();
    }
    
    //POST
    [HttpPost]
    public ActionResult<object> AddQuiz(LinkGenerator link, NewQuizDto dto)
    {
        var quiz = _service.AddQuiz(_mapper.Map<Quiz>(dto));
        return Created(
            link.GetPathByAction(HttpContext, nameof(GetQuiz), null, new { quiId = quiz.Id }),
            quiz
        );
    }

    //GET
    [HttpGet]
    [Route("{quizId}")]
    public ActionResult<Quiz> GetQuiz(int quizId)
    {
        var quiz = _service.FindAllQuizzes().FirstOrDefault(q => q.Id == quizId);
        return quiz is null ? NotFound() : quiz;
    }
    
    //PATCH
    [HttpPatch]
    [Route("{quizId}")]
    [Consumes("application/json-patch+json")]
    public ActionResult<Quiz> AddQuizItem(int quizId, JsonPatchDocument<Quiz>? patchDoc)
    {
        var quiz = _service.FindAllQuizzes().FirstOrDefault(q => q.Id == quizId);
        if (quiz is null || patchDoc is null)
        {
            return NotFound(new
            {
                error = $"Quiz width id {quizId} not found"
            });
        }
        int previousCount = quiz.Items.Count;
        patchDoc.ApplyTo(quiz, ModelState);
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        if (previousCount < quiz.Items.Count)
        {
            QuizItem item = quiz.Items[^1];
            quiz.Items.RemoveAt(quiz.Items.Count - 1);
            _service.AddQuizItemToQuiz(quizId, item);
        }
        return Ok(_service.FindAllQuizzes().FirstOrDefault(q => q.Id == quizId));
    }
}