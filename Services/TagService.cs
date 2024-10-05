using BusinessObject;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface ITagService
    {
        IEnumerable<Tag>? GetAllTag();
        Tag? GetTagById(int id);
        void CreateTag(Tag tag);
        void UpdateTag(Tag tag);
        void DeleteTag(int id);

    }
    public class TagService : ITagService
    {
        public ITagRepository _tagRepository;
        public TagService(ITagRepository tagRepository)
        {
            _tagRepository = tagRepository;
        }

        public IEnumerable<Tag>? GetAllTag()
        {
            IEnumerable<Tag> tags = _tagRepository.GetAllTag();
            return tags;
        }

        public Tag? GetTagById(int id)
        {
            Tag tag = _tagRepository.GetTag(id);
            return tag;
        }

        public void CreateTag(Tag tag)
        {
            _tagRepository.AddTag(tag);
        }

        public void UpdateTag(Tag tag)
        {
            _tagRepository.UpdateTag(tag);
        }

        public void DeleteTag(int id)
        {
            _tagRepository.DeleteTag(id);
        }
    }
}
