using Blogg.Data;
using Blogg.Data.FileManager;
using Blogg.Models;
using Blogg.Models.Comments;
using Blogg.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blogg.Controllers
{
    public class HomeController : Controller
    {
        private IRepository<Post> _repo;
        private IFileManager _fileManager;
        private AppDbContext _context;

        public HomeController(IRepository<Post> repo, IFileManager fileManager, AppDbContext context)
        {
            if (repo != null)
                _repo = repo;
            else
                throw new ArgumentNullException(nameof(repo));
            _fileManager = fileManager;
            _context = context;
        }
        //public IActionResult Index(int pageNumber, string category)
        //{
        //    if (pageNumber < 1)
        //        return RedirectToAction("Index", new { pageNumber = 1, category });

        //    var vm = _repo.GetAllPosts(pageNumber, category);

        //    return View(vm);
        //}
        [HttpGet()]
        public ActionResult<IEnumerable<Post>> Index([FromQuery] GeneralResourceParameters resourceParameters)
        {
            IEnumerable<Post> posts;
            if (resourceParameters.Name == null)
                posts = _repo.GetAll(resourceParameters);
            else
                posts = _context.Posts.ToList();
            var indexViewmodel = new IndexViewModel();
            indexViewmodel.Posts = posts;

            return View(indexViewmodel);

            //int excludeRecords = pageSize * (pageNumber - 1);
            //var records = _context.Posts.Include(m => m.Title).Include(c => c.Category).Include(b => b.Body)
            //    .Skip(excludeRecords)
            //    .Take(pageSize);
            //return View(_context.Posts.ToList());
        }

        public IActionResult Post(int id) => 
            View(_repo.GetPost(id));

        [HttpGet("/Image/{image}")]
        [ResponseCache(CacheProfileName = "Monthly")]
        public IActionResult Image(string image) =>
            new FileStreamResult(_fileManager.ImageStream(image), $"image/{image.Substring(image.LastIndexOf('.') + 1)}");


        //TODO: Find a way to arrange the comments section in the front end
        // to look a bit more realistic 
        [HttpPost]
        public async Task<IActionResult> Comment(CommentViewModel vm)
        {
            if(!ModelState.IsValid)
            {
                return RedirectToAction("Post", new { Id = vm.PostId });
            }

            var post = _repo.GetPost(vm.PostId);
            if(vm.MainCommentId == 0)
            {
                post.MainComments = post.MainComments ?? new List<MainComment>();

                post.MainComments.Add(new MainComment
                {
                    Message = vm.Message,
                    Created = DateTime.Now
                });

                _repo.UpdatePost(post);

            }
            else
            {
                var comment = new SubComment
                {
                    MainCommentId = vm.MainCommentId,
                    Message = vm.Message,
                    Created = DateTime.Now
                };
                _repo.AddSubComment(comment);
            }

            await _repo.SaveChangesAsync();
            return RedirectToAction("Post", new { Id = vm.PostId });
        }

        //public IActionResult Index(string category)
        //{
        //    var posts = string.IsNullOrEmpty(category) ? _repo.GetAllPosts() : _repo.GetAllPosts(category);
        //    return View(posts);
        //}
        

        //[HttpGet("/Image/{image}")]
        //public IActionResult Image(string image)
        //{
        //    var mime = image.Substring(image.LastIndexOf('.') + 1);

        //    return new FileStreamResult(_fileManager.ImageStream(image), $"image/{mime}");
        //}
    }
}
