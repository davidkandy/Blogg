using Blogg.Data;
using Blogg.Models;
using Blogg.Models.Comments;
using Blogg.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blogg
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private DbSet<T> table = null
            ;
        private AppDbContext _context = null;

        public Repository(AppDbContext context)
        {
            _context = context;
        }
        public void AddPost(Post post)
        {
            _context.Posts.Add(post);
        }

        public List<Post> GetAllPosts()
        {
            return _context.Posts.ToList();
        }
        public IEnumerable<T> GetAll(GeneralResourceParameters resourceParameters)
        {
            var collection = table.AsQueryable();
            return IndexViewModel<T>.Create(collection, resourceParameters.PageNumber, resourceParameters.PageSize);
        }

        //OBSOLETE

        //public IndexViewModel<T> GetAllPosts(int pageNumber, string category)
        //{
        //    Func<Post, bool> InCategory = (post) => { return post.Category.ToLower().Equals(category.ToLower()); };

        //    int pageSize = 5;
        //    int skipAmount = pageSize * (pageNumber - 1);

        //    var query = _context.Posts.AsQueryable();

        //    if (String.IsNullOrEmpty(category))
        //        query = query.Where(x => InCategory(x));

        //    int postsCount = query.Count();

        //    return new IndexViewModel<T>
        //    {
        //        PageNumber = pageNumber,
        //        Category = category,
        //        NextPage = postsCount > skipAmount + pageSize,
        //        Posts = query
        //            .Skip(skipAmount)
        //            .Take(pageSize)
        //            .ToList()
        //    };
        //}

        public Post GetPost(int id)
        {
            return _context.Posts.Include(p => p.MainComments).ThenInclude(mc => mc.SubComments).FirstOrDefault(p => p.Id == id);  
        }

        public void RemovePost(int id)
        {
            _context.Posts.Remove(GetPost(id));
        }
        public void UpdatePost(Post post)
        {
            _context.Posts.Update(post);
        }

        public async Task<bool> SaveChangesAsync()
        {
            if(await _context.SaveChangesAsync() > 0) { return true; }
            return false;
        }

        public void AddSubComment(SubComment comment)
        {
            _context.SubComments.Add(comment);
        }


        IndexViewModel<T> IRepository<T>.GetAllPosts(int pageNumber, string category)
        {
            throw new NotImplementedException();
        }

        // THIS METHOD DIDN't Work for the Pagination fix that I was trying to do 
        //public IndexViewModel GetAllPosts(int pageNumber, string category)
        //{
        //    Func<Post, bool> InCategory = (post) => { return post.Category.ToLower().Equals(category.ToLower()); };

        //    int pageSize = 5;
        //    int skipAmount = pageSize * (pageNumber - 1);

        //    var query = _context.Posts.AsQueryable();

        //    if (String.IsNullOrEmpty(category))
        //        query = query.Where(x => InCategory(x));

        //    int postsCount = query.Count();

        //    return new IndexViewModel
        //    {
        //        PageNumber = pageNumber,
        //        Category = category,
        //        NextPage = postsCount > skipAmount + pageSize,
        //        Posts = query
        //            .Skip(skipAmount)
        //            .Take(pageSize)
        //            .ToList()
        //    };
        //}

    }
}
