using BHP_API.Contracts;
using BHP_API.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BHP_API.Services
{
    public class AnswerRepository : IAnswerRepository
    {
        private readonly ApplicationDbContext _db;
        public AnswerRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<bool> Create(Answer entity)
        {
            await _db.AddAsync(entity);
            return await this.Save();
        }

        public async Task<bool> Delete(Answer entity)
        {
            _db.Answers.Remove(entity);
            return await this.Save();
        }

        public async Task<bool> Exists(int id)
        {
            return await _db.Answers.AnyAsync(q => q.Id == id);
        }

        public async Task<IList<Answer>> FindAll()
        {
            var answers = await _db.Answers.ToListAsync();
            return answers;
        }

        public async Task<Answer> FindById(int id)
        {
            var answers = await _db.Answers.Include(q => q.Question).FirstOrDefaultAsync(q => q.Id == id);
            return answers;
        }

        public async Task<bool> Save()
        {
            var changes = await _db.SaveChangesAsync();
            return changes > 0;
        }

        public async Task<bool> Update(Answer entity)
        {
            _db.Answers.Update(entity);
            return await Save();
        }
    }
}
