using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QASite.Data
{
    public class QARepository
    {
        private string _connectionString;

        public QARepository(string connectionString)
        {
            _connectionString = connectionString;
        }
        public List<Question> GetAll()
        {
            using var context = new QAContext(_connectionString);

            var qs = context.Questions.Include(q => q.QuestionsTags);
            return qs.Include(q => q.Likes).ToList();
        }

        public User Login(string email, string password)
        {
            var user = GetByEmail(email);
            if (user == null)
            {
                return null;
            }

            bool isValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
            return isValid ? user : null;

        }


        public User GetByEmail(string email)
        {
            using var context = new QAContext(_connectionString);
            return context.Users.FirstOrDefault(u => u.Email == email);
        }

        public int GetLikes(int questionId)
        {
            using var context = new QAContext(_connectionString);
            return context.Likes.Where(l => l.QuestionId == questionId).Count();
        }
        public void IncrementLikes(int questionId, int userId)
        {
            using var context = new QAContext(_connectionString);
            context.Likes.Add(new Like { QuestionId = questionId, UserId = userId });
            context.SaveChanges();
        }

        public void AddUser(User user, string password)
        {
            using var context = new QAContext(_connectionString);
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
            context.Users.Add(user);
            context.SaveChanges();
        }

        public void AddQuestion(Question question, IEnumerable<string> tags)
        {
            using var context = new QAContext(_connectionString);

            context.Questions.Add(question);
            context.SaveChanges();
            foreach (string tag in tags)
            {
                Tag t = GetTag(tag);
                int tagId;
                if (t == null)
                {
                    tagId = AddTag(tag);
                }
                else
                {
                    tagId = t.Id;
                }
                context.QuestionsTags.Add(new QuestionsTags
                {
                    QuestionId = question.Id,
                    TagId = tagId
                });
            }

            context.SaveChanges();
        }

        public Tag GetTag(string tag)
        {
            using var context = new QAContext(_connectionString);
            return context.Tags.FirstOrDefault(t => t.Name == tag);
        }

        public int AddTag(string tag)
        {
            using var context = new QAContext(_connectionString);
            Tag t = new Tag { Name = tag };
            context.Tags.Add(t);
            context.SaveChanges();
            return t.Id;
        }

        public Question GetQuestion(int id)
        {
            using var context = new QAContext(_connectionString);
            var questions = context.Questions.Include(q => q.User);
            var q2 = questions.Include(q => q.Likes);
            return q2.Include(q => q.Answers)
                .ThenInclude(a => a.User)
                .FirstOrDefault(q => q.Id == id);           
        }
        
        public void AddAnswer(Answer answer)
        {
            using var context = new QAContext(_connectionString);
            context.Answers.Add(answer);
            context.SaveChanges();
        }


    }
}
