using Blogg.Models;
using Blogg.Models.Comments;
using Blogg.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Blogg
{
    public interface IRepository<T> where T : class
    {
        Post GetPost(int id);
        IEnumerable<T> GetAll(GeneralResourceParameters resourceParameters);
        List<Post> GetAllPosts();
        IndexViewModel<T> GetAllPosts(int pageNumber, string category);
        void AddPost(Post post);
        void UpdatePost(Post post);
        void RemovePost(int id);
        void AddSubComment(SubComment comment);
        Task<bool> SaveChangesAsync();
    }
}
