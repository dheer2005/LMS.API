using LMS.API.DTOs;
using LMS.API.LMSDbContext;
using LMS.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace LMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuestionController : Controller
    {
        private readonly LMSContext _context;
        public QuestionController(LMSContext context)
        {
            _context = context;
        }

        [HttpPost("add-question")]
        public async Task<IActionResult> AddQuestion([FromBody] QuestionDto model)
        {
            var question = new Question
            {
                QuizId = model.QuizId,
                QuestionText = model.QuestionText,
                OptionA = model.OptionA,
                OptionB = model.OptionB,
                OptionC = model.OptionC,
                OptionD = model.OptionD,
                CorrectOption = model.CorrectOption
            };

            _context.Questions.Add(question);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Question added", question });
        }

        [HttpGet("quiz/{quizId}")]
        public IActionResult GetQuestionsForQuiz(int quizId)
        {
            var questions = _context.Questions
                .Where(q => q.QuizId == quizId)
                .Select(q => new
                {
                    q.Id,
                    q.QuestionText,
                    q.OptionA,
                    q.OptionB,
                    q.OptionC,
                    q.OptionD,
                    q.CorrectOption
                })
                .ToList();

            return Ok(questions);
        }
    }
}
