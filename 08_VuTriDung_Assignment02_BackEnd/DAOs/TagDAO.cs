using Microsoft.EntityFrameworkCore;
using _08_VuTriDung_Assignment02.Models;

namespace _08_VuTriDung_Assignment02.DAOs
{
    public class TagDAO
    {
        private static TagDAO? _instance;
        private static readonly object _instanceLock = new object();

        private TagDAO() { }

        public static TagDAO Instance
        {
            get
            {
                lock (_instanceLock)
                {
                    if (_instance == null)
                    {
                        _instance = new TagDAO();
                    }
                    return _instance;
                }
            }
        }

        public List<Tag> GetAll()
        {
            using var db = new FUNewsManagementDbContext();
            return db.Tags.AsNoTracking().ToList();
        }

        public Tag? GetById(int id)
        {
            using var db = new FUNewsManagementDbContext();
            return db.Tags.AsNoTracking().FirstOrDefault(t => t.TagID == id);
        }

        public Tag Create(Tag tag)
        {
            using var db = new FUNewsManagementDbContext();
            if (tag.TagID == 0)
            {
                int maxId = db.Tags.Any() ? db.Tags.Max(t => t.TagID) : 0;
                tag.TagID = maxId + 1;
            }
            db.Tags.Add(tag);
            db.SaveChanges();
            return tag;
        }

        public Tag? Update(Tag tag)
        {
            using var db = new FUNewsManagementDbContext();
            var existing = db.Tags.FirstOrDefault(t => t.TagID == tag.TagID);
            if (existing == null) return null;

            existing.TagName = tag.TagName;
            existing.Note = tag.Note;

            db.SaveChanges();
            return existing;
        }

        public bool Delete(int id)
        {
            using var db = new FUNewsManagementDbContext();
            var existing = db.Tags.FirstOrDefault(t => t.TagID == id);
            if (existing == null) return false;

            db.Tags.Remove(existing);
            db.SaveChanges();
            return true;
        }
    }
}
