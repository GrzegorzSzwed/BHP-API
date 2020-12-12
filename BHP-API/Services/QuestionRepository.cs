using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using BHP_API.Contracts;
using BHP_API.Data;

namespace BHP_API.Services
{
    public class QuestionRepository : IQuestionRepository
    {
        private readonly ApplicationDbContext _db;
        public QuestionRepository(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<bool> Create(Question entity)
        {
            await _db.AddAsync(entity);
            return await this.Save();
        }

        public async Task<bool> Delete(Question entity)
        {
            _db.Questions.Remove(entity);
            return await this.Save();
        }

        public async Task<bool> Exists(int id)
        {
            return await _db.Questions.AnyAsync(q => q.Id == id);
        }

        public async Task<IList<Question>> FindAll()
        {
            var questions = await _db.Questions.ToListAsync();
            return questions;
        }

        public async Task<Question> FindById(int id)
        {
            var question = await _db.Questions.Include(q => q.Answers).FirstOrDefaultAsync(q => q.Id == id);
            return question;
        }

        public async Task<bool> Save()
        {
            var changes = await _db.SaveChangesAsync();
            return changes > 0;
        }

        public async Task<bool> Update(Question entity)
        {
            _db.Questions.Update(entity);
            return await Save();
        }
    }
}
