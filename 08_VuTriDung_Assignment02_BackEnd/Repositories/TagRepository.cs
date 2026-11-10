using _08_VuTriDung_Assignment02.DAOs;
using _08_VuTriDung_Assignment02.Models;

namespace _08_VuTriDung_Assignment02.Repositories
{
    public class TagRepository : ITagRepository
    {
        private readonly TagDAO _tagDAO;

        public TagRepository(TagDAO tagDAO)
        {
            _tagDAO = tagDAO;
        }

        public IQueryable<Tag> GetAll() => _tagDAO.GetAll();

        public Tag? GetById(int id) => _tagDAO.GetById(id);

        public Tag Create(Tag tag) => _tagDAO.Create(tag);

        public Tag? Update(int id, Tag tag) => _tagDAO.Update(id, tag);

        public bool Delete(int id) => _tagDAO.Delete(id);
    }
}
