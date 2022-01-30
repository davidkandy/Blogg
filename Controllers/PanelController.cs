global using Microsoft.AspNetCore.Mvc;
using Blogg.Data.FileManager;
using Blogg.Models;
using Blogg.ViewModels;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blogg.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PanelController : Controller
    {
        private readonly IFileManager _fileManager;
        private readonly IRepository<Post> _repository;

        public PanelController(IRepository<Post> repository, IFileManager fileManager)
        {
            _fileManager = fileManager;
            _repository = repository;
        }
        public IActionResult Index()
        {
            var posts = _repository.GetAllPosts();
            return View(posts);
        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id == null)
                return View(new PostViewModel());
            else
            {
                var post = _repository.GetPost((int)id);
                return View(new PostViewModel
                {
                    Title = post.Title,
                    Body = post.Body,
                    Id = post.Id,
                    CurrentImage = post.Image,
                    Description = post.Description,
                    Category = post.Category,
                    Tags = post.Tags
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(PostViewModel vm)
        {
            var post = new Post
            {
                Id = vm.Id,
                Title = vm.Title,
                Body = vm.Body,
                Image = await _fileManager.SaveImage(vm.Image),
                Description = vm.Description,
                Tags = vm.Tags,
                Category = vm.Category
            };

            if (vm.Image == null)
                post.Image = vm.CurrentImage;
            else
                await _fileManager.SaveImage(vm.Image);

            if (post.Id > 0)
                _repository.UpdatePost(post);
            else
                _repository.AddPost(post);

            if (await _repository.SaveChangesAsync())
                return RedirectToAction("Index");
            else
                return View(post);
        }

        [HttpGet]
        public async Task<IActionResult> Remove(int id)
        {
            _repository.RemovePost(id);
            await _repository.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
