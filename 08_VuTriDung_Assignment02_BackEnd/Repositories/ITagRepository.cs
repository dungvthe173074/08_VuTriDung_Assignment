using _08_VuTriDung_Assignment02.Models;

namespace _08_VuTriDung_Assignment02.Repositories
{
    public interface ITagRepository
    {
        IQueryable<Tag> GetAll();
        Tag? GetById(int id);
        Tag Create(Tag tag);
        Tag? Update(int id, Tag tag);
        bool Delete(int id);
    }
}
