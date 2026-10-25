using _08_VuTriDung_Assignment02.DAOs;
using _08_VuTriDung_Assignment02.Models;

namespace _08_VuTriDung_Assignment02.Repositories
{
    public class TagRepository : ITagRepository
    {
        public IEnumerable<Tag> GetAll()
        {
            return TagDAO.Instance.GetAll();
        }

        public Tag? GetById(int id)
        {
            return TagDAO.Instance.GetById(id);
        }

        public Tag Create(Tag tag)
        {
            return TagDAO.Instance.Create(tag);
        }

        public Tag? Update(Tag tag)
        {
            return TagDAO.Instance.Update(tag);
        }

        public bool Delete(int id)
        {
            return TagDAO.Instance.Delete(id);
        }
    }
}
