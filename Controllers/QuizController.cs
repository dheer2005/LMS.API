using LMS.API.DTOs;
using LMS.API.LMSDbContext;
using LMS.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuizController : Controller
    {
        private readonly LMSContext _context;

        public QuizController(LMSContext context)
        {
            _context = context;
        }

        [HttpPost("create")]
        public IActionResult CreateQuiz([FromBody] QuizDto dto)
        {
            var quiz = new Quiz
            {
                CourseId = dto.CourseId,
                Title = dto.Title,
                Questions = new List<Question>()
            };

            _context.Quizzes.Add(quiz);
            _context.SaveChanges();
            return Ok(quiz);
        }

        [HttpPost("add-question")]
        public IActionResult AddQuestion([FromBody] QuestionDto dto)
        {
            var question = new Question
            {
                QuizId = dto.QuizId,
                QuestionText = dto.QuestionText,
                OptionA = dto.OptionA,
                OptionB = dto.OptionB,
                OptionC = dto.OptionC,
                OptionD = dto.OptionD,
                CorrectOption = dto.CorrectOption
            };

            _context.Questions.Add(question);
            _context.SaveChanges();
            return Ok(question);
        }

        [HttpPost("submit")]
        public IActionResult SubmitQuiz([FromBody] SubmitQuizDto submission)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var total = submission.Answers.Count();
            var correct = 0;

            foreach (var answer in submission.Answers)
            {
                var question = _context.Questions.FirstOrDefault(q => q.Id == answer.QuestionId);
                if(question != null && question.CorrectOption == answer.SelectedOption)
                    correct++;
            }

            return Ok(new
            {
                TotalQuestions = total,
                CorrectAnswers = correct
            });
        }


        [HttpGet("attempt/{quizId}/{userId}")]
        public IActionResult GetQuizWithQuestions(int quizId, int userId)
        {
            var submittedQuiz = _context.QuizSubmissions.FirstOrDefault(x => x.UserId == userId && x.QuizId == quizId);
            var systemSetting = _context.SystemSettings.First();
            if(submittedQuiz != null && systemSetting != null)
            {
                if (submittedQuiz.AttemptNumber >= systemSetting.QuizRetakeLimit)
                    return BadRequest(new { message = $"you cannot attempt this quiz more then {systemSetting.QuizRetakeLimit} times" });
            }

            var quiz = _context.Quizzes
                .Include(q => q.Questions)
                .FirstOrDefault(q => q.Id == quizId);

            if (quiz == null) return NotFound("Quiz not found");

            var result = new QuizDto
            {
                CourseId = quiz.Id,
                Title = quiz.Title,
                AttemptNumber = submittedQuiz?.AttemptNumber ?? 0,
                Questions = quiz.Questions.Select(q => new QuestionDto
                {
                    QuestionId = q.Id,
                    QuizId = q.QuizId,
                    QuestionText = q.QuestionText,
                    OptionA = q.OptionA,
                    OptionB = q.OptionB,
                    OptionC = q.OptionC,
                    OptionD = q.OptionD,
                    CorrectOption = q.CorrectOption
                }).ToList()
            };

            return Ok(result);
        }

        [HttpPost("quizSubmission")]
        public async Task<IActionResult> SubmitQuiz([FromBody] QuizSubmissionDto dto)
        {
            var quiz = await _context.Quizzes.FirstOrDefaultAsync(x => x.Id == dto.QuizId);
            if (quiz == null) 
                return BadRequest(new { message = "Quiz not found" });

            var settings = await _context.SystemSettings.FirstAsync();
            

            var submittedQuiz = await _context.QuizSubmissions.FirstOrDefaultAsync(x => x.UserId == dto.UserId && x.QuizId == dto.QuizId);

            if (submittedQuiz?.AttemptNumber >= settings?.QuizRetakeLimit)
                return BadRequest(new { message = $"You cannot attempt this quiz more then {settings.QuizRetakeLimit} times" });

            if (submittedQuiz != null)
            {
                submittedQuiz.UserId = dto.UserId;
                submittedQuiz.QuizId = dto.QuizId;
                submittedQuiz.Score = dto.Score > submittedQuiz.Score ? dto.Score : submittedQuiz.Score;
                submittedQuiz.AttemptNumber = submittedQuiz.AttemptNumber + 1;

                await _context.SaveChangesAsync();
            }
            else
            {
                var submission = new QuizSubmission
                {
                    UserId = dto.UserId,
                    QuizId = dto.QuizId,
                    Score = dto.Score,
                    AttemptNumber = 1,
                };
                _context.QuizSubmissions.Add(submission);
                await _context.SaveChangesAsync();
            }
            return Ok(new {message = "Quiz submitted succesfully"});
        }


        [HttpGet("course/{courseId}/{studentId}")]
        public IActionResult GetQuizzesByCourse(int courseId, int studentId)
        {
            var setting = _context.SystemSettings.FirstOrDefault(x=>x.Id == 1);

            var quizzes = _context.Quizzes
                .Where(q => q.CourseId == courseId &&
                            q.Questions.Any()) 
                .Select(q => new
                {
                    q.Id,
                    q.Title,
                    CanAttempt = setting.QuizRetakeLimit,
                    AttemptCount = _context.QuizSubmissions
                                    .Where(sub => sub.QuizId == q.Id && sub.UserId == studentId)
                                    .Select(sub => (int?)sub.AttemptNumber)
                                    .Max() ?? 0
                })
                .ToList();

            return Ok(quizzes);
        }

        [HttpGet("results/{quizId}")]
        public IActionResult GetQuizResults(int quizId)
        {
            var quiz = _context.Quizzes.FirstOrDefault(q => q.Id == quizId);
            if (quiz == null) return NotFound("Quiz not found");

            var totalQuestions = _context.Questions
                .Count(q => q.QuizId == quizId);

            var results = _context.QuizSubmissions
                .Where(qs => qs.QuizId == quizId)
                .Select(qs => new
                {
                    StudentId = qs.UserId,
                    StudentName = _context.Users
                        .Where(u => u.Id == qs.UserId)
                        .Select(u => u.FullName)
                        .FirstOrDefault(),
                    Attempts = qs.AttemptNumber,
                    BestScore = qs.Score,
                    TotalQuestions = totalQuestions
                })
                .ToList();

            return Ok(new
            {
                QuizTitle = quiz.Title,
                Results = results
            });
        }
    }
}
