using BusinessObject;
using DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public interface ITagRepository
    {
        public Tag GetTag(int id);
        public IEnumerable<Tag> GetAllTag();
        public void AddTag(Tag tag);
        public void UpdateTag(Tag newtag);
        public void DeleteTag(int id);
        public bool TagExists(int id);
    }
    public class TagRepository : ITagRepository
    {
        public Tag GetTag(int id) => TagManager.Instance.GetTag(id);

        public IEnumerable<Tag> GetAllTag() => TagManager.Instance.GetAllTag();
        public void AddTag(Tag tag) => TagManager.Instance.AddTag(tag);
        public void UpdateTag(Tag newtag) => TagManager.Instance.UpdateTag(newtag);
        public void DeleteTag(int id) => TagManager.Instance.DeleteTag(id);
        public bool TagExists(int id) => TagManager.Instance.TagExists(id);
    }
}
