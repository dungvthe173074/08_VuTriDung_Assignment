using Microsoft.EntityFrameworkCore;
using _08_VuTriDung_Assignment02.Models;

namespace _08_VuTriDung_Assignment02.DAOs
{
    public class TagDAO
    {
        private readonly FUNewsManagementDbContext _context;

        public TagDAO(FUNewsManagementDbContext context)
        {
            _context = context;
        }

        public IQueryable<Tag> GetAll()
        {
            return _context.Tags.AsNoTracking();
        }

        public Tag? GetById(int id)
        {
            return _context.Tags.AsNoTracking().FirstOrDefault(t => t.TagID == id);
        }

        public Tag Create(Tag tag)
        {
            if (tag.TagID == 0)
            {
                int maxId = _context.Tags.Any() ? _context.Tags.Max(t => t.TagID) : 0;
                tag.TagID = maxId + 1;
            }
            _context.Tags.Add(tag);
            _context.SaveChanges();
            return tag;
        }

        public Tag? Update(int id, Tag updated)
        {
            var existing = _context.Tags.FirstOrDefault(t => t.TagID == id);
            if (existing == null) return null;

            existing.TagName = updated.TagName;
            existing.Note = updated.Note;

            _context.SaveChanges();
            return existing;
        }

        public bool Delete(int id)
        {
            var existing = _context.Tags.FirstOrDefault(t => t.TagID == id);
            if (existing == null) return false;

            _context.Tags.Remove(existing);
            _context.SaveChanges();
            return true;
        }
    }
}
