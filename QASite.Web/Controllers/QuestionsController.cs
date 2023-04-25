using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using QASite.Data;
using QASite.Web.Models;
using Microsoft.AspNetCore.Authorization;

namespace QASite.Web.Controllers
{
    public class QuestionsController : Controller
    {
        private readonly IConfiguration _configuration;

        public QuestionsController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public IActionResult Index()
        {
            var connectionString = _configuration.GetConnectionString("ConStr");
            var repo = new QARepository(connectionString);
            return View(new QuestionsViewModel
            {
                Questions = repo.GetAll()
            });
        }

        [Authorize]
        public IActionResult AskAQuestion()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public IActionResult AddAnswer(Answer answer)
        {
            var connectionString = _configuration.GetConnectionString("ConStr");
            var repo = new QARepository(connectionString);
            var email = User.Identity.Name;
            var user = repo.GetByEmail(email);
            answer.UserId = user.Id;
            answer.DatePosted = DateTime.Now;
            repo.AddAnswer(answer);
            return Redirect($"/questions/ViewQuestion?id={answer.QuestionId}");
        }
        
        [Authorize]
        [HttpPost]
        public IActionResult Add(Question question, string tags)
        {
            var connectionString = _configuration.GetConnectionString("ConStr");
            var repo = new QARepository(connectionString);
            var email = User.Identity.Name;
            var user = repo.GetByEmail(email);
            var sepTags = tags.Split(" ").ToList();
            question.UserId = user.Id;
            question.DatePosted = DateTime.Now;
            repo.AddQuestion(question, sepTags);
            return RedirectToAction("index");
        }

        public IActionResult ViewQuestion(int id)
        {
            var connectionString = _configuration.GetConnectionString("ConStr");
            var repo = new QARepository(connectionString);
            var vm = new QuestionViewModel();
            vm.Question = repo.GetQuestion(id);
            if(vm.Question == null)
            {
                return RedirectToAction("Index");
            }
            if (User.Identity.IsAuthenticated)
            {
                var email = User.Identity.Name;
                var user = repo.GetByEmail(email);
                vm.CantLike = vm.Question.Likes.Any(l => l.UserId == user.Id && l.QuestionId == vm.Question.Id);
            }
            return View(vm);
        }

        [Authorize]
        [HttpPost]
        public void AddLike(int questionId)
        {
            var connectionString = _configuration.GetConnectionString("ConStr");
            var repo = new QARepository(connectionString);
            var email = User.Identity.Name;
            var user = repo.GetByEmail(email);
            repo.IncrementLikes(questionId, user.Id);
        }
        public ActionResult GetLikes(int questionId)
        {
            var connectionString = _configuration.GetConnectionString("ConStr");
            var repo = new QARepository(connectionString);
            return Json(repo.GetLikes(questionId));
        }
    }

    public static class SessionExtensions
    {
        public static void Set<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        public static T Get<T>(this ISession session, string key)
        {
            string value = session.GetString(key);

            return value == null ? default(T) :
                JsonConvert.DeserializeObject<T>(value);
        }
    }
}
