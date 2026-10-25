using _08_VuTriDung_Assignment02.Models;

namespace _08_VuTriDung_Assignment02.Repositories
{
    public interface ITagRepository
    {
        IEnumerable<Tag> GetAll();
        Tag? GetById(int id);
        Tag Create(Tag tag);
        Tag? Update(Tag tag);
        bool Delete(int id);
    }
}
