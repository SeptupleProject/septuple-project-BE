﻿using CsvHelper;
using Enterprise_Web.DTOs;
using Enterprise_Web.Models;
using Enterprise_Web.Pagination.Filter;
using Enterprise_Web.Repository.IRepository;
using Enterprise_Web.Services;
using Enterprise_Web.ViewModels;
using EnterpriseWeb.Data;
using Firebase.Auth;
using Firebase.Storage;
using ICSharpCode.SharpZipLib.Zip;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO.Compression;
using System.Reflection;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
using System.Globalization;
using Microsoft.AspNetCore.JsonPatch.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Linq;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;
using System.Text;
using ZipFile = System.IO.Compression.ZipFile;
using System.IO;

namespace Enterprise_Web.Repository
{
    public class IdeaRepository : IIdeaRepository
    {
        private static string apiKey = "AIzaSyD1EOiY1Q7dfMLPtTg_RXLpW0HmlyCpQmo";
        private static string Bucket = "enterprise-web-c3a66.appspot.com";
        private static string AuthEmail = "dinhgiabao@gmail.com";
        private static string AuthPassword = "app123";

        private readonly IWebHostEnvironment _env;
        private readonly IHostingEnvironment _hosting;
        private readonly ApplicationDbContext _dbContext;
        private readonly INotificationRepository _notificationRepository;

        public IdeaRepository(ApplicationDbContext dbContext, INotificationRepository notificationRepository, IWebHostEnvironment env, IHostingEnvironment hosting)
        {
            _dbContext = dbContext;
            _notificationRepository = notificationRepository;
            _env = env;
            _hosting = hosting;
        }
        public (List<IdeaDTO>, PaginationFilter, int) GetAll(PaginationFilter filter)
        {
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize, filter.Search, filter.Role);
            var listIdeas = (from idea in _dbContext.Ideas
                             select
                                 new IdeaDTO
                                 {
                                     Id = idea.Id,
                                     Title = idea.Title,
                                     Content = idea.Content,
                                     CategoryName = idea.Category.Name,
                                     Image = idea.Image,
                                     CreatedBy = idea.CreatedBy,
                                     Like = idea.Reactions.Count(x => x.Like == true),
                                     DisLike = idea.Reactions.Count(x => x.Like == false),
                                     Comments = idea.Comments.Count(),
                                     IsAnonymous = idea.IsAnonymos,
                                     Views = idea.Views
                                 })
                               .OrderByDescending(x => x.Id)
                               .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
                               .Take(validFilter.PageSize)
                               .ToList();
            var countIdeas = (from idea in _dbContext.Ideas select idea).Count();

            return (listIdeas, validFilter, countIdeas);
        }

        public Idea GetById(int id)
        {
            var ideaToGet = _dbContext.Ideas.Include(x => x.Comments).FirstOrDefault(i => i.Id == id);
            if (ideaToGet == null)
            {
                return null;
            }
            return ideaToGet;

        }

        public async Task Create(Idea idea, int userId, string userEmail)
        {
            if (idea.File != null)
            {
                var fileUpload = idea.File;
                var name = DateTime.Now.ToFileTime() + fileUpload.FileName;
                FileStream fileStream = null;
                string image = "";
                if (fileUpload.Length > 0)
                {
                    var path = Path.Combine(_env.ContentRootPath, "Images");

                    if (Directory.Exists(path))
                    {
                        using (fileStream = new FileStream(Path.Combine(path, name), FileMode.Create))
                        {
                            await fileUpload.CopyToAsync(fileStream);
                        }
                        fileStream = new FileStream(Path.Combine(path, name), FileMode.Open);
                    }
                    else
                    {
                        Directory.CreateDirectory(path);
                        using (fileStream = new FileStream(Path.Combine(path, name), FileMode.Create))
                        {
                            await fileUpload.CopyToAsync(fileStream);
                        }
                    }

                    var auth = new FirebaseAuthProvider(new FirebaseConfig(apiKey));
                    var a = await auth.SignInWithEmailAndPasswordAsync(AuthEmail, AuthPassword);

                    var cancel = new CancellationTokenSource();

                    var task = new FirebaseStorage(
                        Bucket,
                        new FirebaseStorageOptions
                        {
                            AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                            ThrowOnCancel = true
                        })
                        .Child("images")
                        .Child(name)
                        .PutAsync(fileStream, cancel.Token);

                    try
                    {
                        var link = await task;
                        image = link;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Error:{e.Message}");
                    }
                }

                var acadYearIds = _dbContext.AcademicYears.OrderByDescending(x => x.StartDate).Select(x => x.Id).First();

                var newIdeas = new Idea()
                {
                    Title = idea.Title,
                    Content = idea.Content,
                    Views = 0,
                    Image = image,
                    IsAnonymos = idea.IsAnonymos,
                    AcademicYearId = acadYearIds,
                    CategoryId = idea.CategoryId,
                    UserId = userId,
                    CreatedBy = userEmail,
                    CreatedAt = DateTime.Now,
                };

                await _dbContext.AddAsync(newIdeas);
                await _dbContext.SaveChangesAsync();

                var newNotis = new NotificationViewModel()
                {
                    IdeaId = null,
                    CreatedBy = userEmail
                };
                await _notificationRepository.CheckAndSend(newNotis);
            }

            else
            {
                var acadYearId = _dbContext.AcademicYears.OrderByDescending(x => x.StartDate).Select(x => x.Id).First();

                var newIdea = new Idea()
                {
                    Title = idea.Title,
                    Content = idea.Content,
                    Views = 0,
                    Image = "",
                    IsAnonymos = idea.IsAnonymos,
                    AcademicYearId = acadYearId,
                    CategoryId = idea.CategoryId,
                    UserId = userId,
                    CreatedBy = userEmail,
                    CreatedAt = DateTime.Now,
                };

                await _dbContext.AddAsync(newIdea);
                await _dbContext.SaveChangesAsync();

                var foundUser = _dbContext.Users.FirstOrDefault(x => x.Id == userId);

                var newNoti = new NotificationViewModel()
                {
                    IdeaId = null,
                    CreatedBy = userEmail,
                    DepartmentId = foundUser?.DepartmentId
                };
                await _notificationRepository.CheckAndSend(newNoti);
            }
        }


        public async Task Update(Idea idea)
        {
            if(idea.File != null)
            {
                var fileUpload = idea.File;
                var name = DateTime.Now.ToFileTime() + fileUpload.FileName;
                FileStream fileStream = null;
                string image = "";
                if (fileUpload.Length > 0)
                {
                    var path = Path.Combine(_env.ContentRootPath, "Images");

                    if (Directory.Exists(path))
                    {
                        using (fileStream = new FileStream(Path.Combine(path, name), FileMode.Create))
                        {
                            await fileUpload.CopyToAsync(fileStream);
                        }
                        fileStream = new FileStream(Path.Combine(path, name), FileMode.Open);
                    }
                    else
                    {
                        Directory.CreateDirectory(path);
                        using (fileStream = new FileStream(Path.Combine(path, name), FileMode.Create))
                        {
                            await fileUpload.CopyToAsync(fileStream);
                        }
                    }

                    var auth = new FirebaseAuthProvider(new FirebaseConfig(apiKey));
                    var a = await auth.SignInWithEmailAndPasswordAsync(AuthEmail, AuthPassword);

                    var cancel = new CancellationTokenSource();

                    var task = new FirebaseStorage(
                        Bucket,
                        new FirebaseStorageOptions
                        {
                            AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                            ThrowOnCancel = true
                        })
                        .Child("images")
                        .Child(name)
                        .PutAsync(fileStream, cancel.Token);

                    try
                    {
                        var link = await task;
                        image = link;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Error:{e.Message}");
                    }
                }

                var findIdeas = await _dbContext.Ideas.FirstOrDefaultAsync(x => x.Id == idea.Id);
                if (findIdeas != null)
                {
                    findIdeas.Title = idea.Title;
                    findIdeas.Content = idea.Content;
                    findIdeas.Image = image;
                    findIdeas.IsAnonymos = idea.IsAnonymos;
                }
                _dbContext.Update(findIdeas);
                await _dbContext.SaveChangesAsync();
            }
            else
            {
                var findIdea = await _dbContext.Ideas.FirstOrDefaultAsync(x => x.Id == idea.Id);
                if (findIdea != null)
                {
                    findIdea.Title = idea.Title;
                    findIdea.Content = idea.Content;
                    findIdea.IsAnonymos = idea.IsAnonymos;
                }
                _dbContext.Update(findIdea);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task Delete(int id)
        {
            var findReaction = _dbContext.Reactions.Where(x => x.IdeaId == id).ToList();
            _dbContext.RemoveRange(findReaction);
            await _dbContext.SaveChangesAsync();

            var ideaDelete = await _dbContext.Ideas.FindAsync(id);
            _dbContext.Remove(ideaDelete);
            await _dbContext.SaveChangesAsync();
        }

        public Task Download(string zipFile)
        {
            var findImage = Directory.GetFiles(zipFile).ToList();
            var fileName = "Image.zip";
            string pathUser = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string tempOutput = Path.Combine(pathUser, $"Downloads\\{fileName}");

            using (ZipOutputStream IzipOutputStream = new ZipOutputStream(System.IO.File.Create(tempOutput)))
            {
                IzipOutputStream.SetLevel(9);
                byte[] buffer = new byte[4096];

                for (int i = 0; i < findImage.Count; i++)
                {
                    ZipEntry entry = new ZipEntry(Path.GetFileName(findImage[i]));
                    entry.DateTime = DateTime.Now;
                    entry.IsUnicodeText = true;
                    IzipOutputStream.PutNextEntry(entry);

                    using (FileStream oFileStream = System.IO.File.OpenRead(findImage[i]))
                    {
                        int sourceBytes;
                        do
                        {
                            sourceBytes = oFileStream.Read(buffer, 0, buffer.Length);
                            IzipOutputStream.Write(buffer, 0, sourceBytes);
                        } while (sourceBytes > 0);
                    }
                }
                IzipOutputStream.Finish();
                IzipOutputStream.Flush();
                IzipOutputStream.Close();
            }
            return Task.CompletedTask;
        }

        public List<IdeaViewModel> GetIdeasToDownload()
        {
            var ideas = _dbContext.Ideas.Include(x => x.Category).Include(x => x.AcademicYear).ToList();
            var ideaViewModels = new List<IdeaViewModel>();
            foreach (var idea in ideas)
            {
                var ideaViewModel = new IdeaViewModel()
                {
                    Id = idea.Id,
                    Title = idea.Title,
                    Content = idea.Content,
                    Views = idea.Views,
                    Category = idea.Category?.Name,
                    AcademicYear = idea.AcademicYear?.Name,
                    CreatedBy = idea.CreatedBy,
                    CreatedAt = idea.CreatedAt,
                };
                ideaViewModels.Add(ideaViewModel);
            }
            return ideaViewModels;
        }

        public Task DownloadIdeas(List<IdeaViewModel> ideaViewModels, List<CommentViewModel> commentViewModels)
        {
            string pathUser = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string tempOutput = Path.Combine(pathUser, $"Downloads\\ideas_comments{DateTime.Now.ToFileTime()}.zip");

            using (var zf = ZipFile.Open(tempOutput, ZipArchiveMode.Create))
            {
                var ze1 = zf.CreateEntry("ideas.csv");
                using (var zs = ze1.Open())
                {
                    using (var writ = new StreamWriter(zs, Encoding.UTF8))
                    {
                        using (var csvWriter = new CsvWriter(writ, CultureInfo.InvariantCulture))
                        {
                            csvWriter.Context.RegisterClassMap<IdeasClassMap>();
                            csvWriter.WriteRecords(ideaViewModels);
                        }                   
                    }
                }

                var ze2 = zf.CreateEntry("comments.csv");
                using (var zs = ze2.Open())
                {
                    using (var writ = new StreamWriter(zs, Encoding.UTF8))
                    {
                        using (var csvWriter = new CsvWriter(writ, CultureInfo.InvariantCulture))
                        {
                            csvWriter.Context.RegisterClassMap<CommentsClassMap>();
                            csvWriter.WriteRecords(commentViewModels);
                        }
                    }
                }
            }

            return Task.CompletedTask;
        }

        public async Task IcrementedView(int id)
        {
            var findIdeas = await _dbContext.Ideas.FirstOrDefaultAsync(x => x.Id == id);
            if (findIdeas != null)
            {
                findIdeas.Views += 1;
            }
            _dbContext.Update(findIdeas);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<IdeaDTO>> MostLikeIdea()
        {
            var findIdeaId = (from r in _dbContext.Reactions
                              where r.Like == true
                              group r by r.IdeaId into GroupIdea
                              select new Idea
                              {
                                  Id = GroupIdea.Key
                              }).ToArray();

            var mostLikeIdea = _dbContext.Ideas.Include(x => x.Reactions).Where(x => findIdeaId.Contains(x)).OrderByDescending(x => x.Reactions.Count(x => x.Like == true))
                                .Select(x=> new IdeaDTO
                                {
                                    Id = x.Id,
                                    Title = x.Title,
                                    Content = x.Content,
                                    CategoryName = x.Category.Name,
                                    CreatedBy = x.CreatedBy,
                                    Image = x.Image,
                                    Like = x.Reactions.Count(x=>x.Like == true),
                                    DisLike = x.Reactions.Count(x => x.Like == false),
                                    Comments = x.Comments.Count(),
                                    Views = x.Views,
                                    IsAnonymous = x.IsAnonymos
                                }).Take(3).ToList();

            return mostLikeIdea;

        }

        public async Task<List<IdeaDTO>> MostDislikeIdea()
        {
            var findIdeaId = (from r in _dbContext.Reactions
                              where r.Like == false
                              group r by r.IdeaId into GroupIdea
                              select new Idea
                              {
                                  Id = GroupIdea.Key
                              }).ToArray();

            var mostDislikeIdea = _dbContext.Ideas.Include(x=>x.Reactions).Where(x => findIdeaId.Contains(x)).OrderByDescending(x => x.Reactions.Count(x => x.Like == false))
                                  .Select(x => new IdeaDTO
                                  {
                                      Id = x.Id,
                                      Title = x.Title,
                                      Content = x.Content,
                                      CategoryName = x.Category.Name,
                                      CreatedBy = x.CreatedBy,
                                      Image = x.Image,
                                      Like = x.Reactions.Count(x => x.Like == true),
                                      DisLike = x.Reactions.Count(x => x.Like == false),
                                      Comments = x.Comments.Count(),
                                      Views = x.Views,
                                      IsAnonymous = x.IsAnonymos
                                  }).Take(3).ToList();

            return mostDislikeIdea;
        }

        public async Task<List<IdeaDTO>> MostCommentIdea()
        {
            var findIdeaId = _dbContext.Comments
                                .GroupBy(r => r.IdeaId)
                                .Select(x => x.Key)
                                .ToArray();

            var mostCommentIdea = _dbContext.Ideas.Include(x => x.Comments).Where(x => findIdeaId.Contains(x.Id)).OrderByDescending(x => x.Comments.Count())
                                  .Select(x => new IdeaDTO
                                  {
                                      Id = x.Id,
                                      Title = x.Title,
                                      Content = x.Content,
                                      CategoryName = x.Category.Name,
                                      CreatedBy = x.CreatedBy,
                                      Image = x.Image,
                                      Like = x.Reactions.Count(x => x.Like == true),
                                      DisLike = x.Reactions.Count(x => x.Like == false),
                                      Comments = x.Comments.Count(),
                                      Views = x.Views,
                                      IsAnonymous = x.IsAnonymos
                                  }).Take(3).ToList();

            return mostCommentIdea;
        }

        public async Task<List<IdeaDTO>> MostViewsIdea()
        {
            var mostViewsIdea =  _dbContext.Ideas.OrderByDescending(i => i.Views)
                                 .Select(x => new IdeaDTO
                                 {
                                     Id = x.Id,
                                     Title = x.Title,
                                     Content = x.Content,
                                     CategoryName = x.Category.Name,
                                     CreatedBy = x.CreatedBy,
                                     Image = x.Image,
                                     Like = x.Reactions.Count(x => x.Like == true),
                                     DisLike = x.Reactions.Count(x => x.Like == false),
                                     Comments = x.Comments.Count(),
                                     Views = x.Views,
                                     IsAnonymous = x.IsAnonymos
                                 }).Take(3).ToList();

            return mostViewsIdea;
        }


        public async Task<Reaction> LikeIdea(int userId, Idea idea)
        {
            var userReaction = _dbContext.Reactions.Where(i => (i.UserId == userId && i.IdeaId == idea.Id)).FirstOrDefault();

            if (userReaction == null)
            {
                var reactionToLike = new Reaction
                {
                    UserId = userId,
                    IdeaId = idea.Id,
                    Like = true
                };

                _dbContext.Reactions.Add(reactionToLike);
                await _dbContext.SaveChangesAsync();
                return reactionToLike;
            }

            if (userReaction.Like == true)
            {
                _dbContext.Reactions.Remove(userReaction);
                await _dbContext.SaveChangesAsync();
                return userReaction;
            }

            if (userReaction.Like == false)
            {
                userReaction.Like = true;
                _dbContext.Reactions.Update(userReaction);
                await _dbContext.SaveChangesAsync();
                return userReaction;
            }

            return userReaction;
        }

        public async Task<Reaction> DislikeIdea(int userId, Idea idea)
        {
            var userReaction = _dbContext.Reactions.Where(i => (i.UserId == userId && i.IdeaId == idea.Id)).FirstOrDefault();

            if (userReaction == null)
            {
                var reactionToDislike = new Reaction
                {
                    UserId = userId,
                    IdeaId = idea.Id,
                    Like = false
                };

                _dbContext.Reactions.Add(reactionToDislike);
                await _dbContext.SaveChangesAsync();
                return reactionToDislike;
            }

            if (userReaction.Like == false)
            {
                _dbContext.Reactions.Remove(userReaction);
                await _dbContext.SaveChangesAsync();
                return userReaction;
            }

            if (userReaction.Like == true)
            {
                userReaction.Like = false;
                _dbContext.Reactions.Update(userReaction);
                await _dbContext.SaveChangesAsync();
                return userReaction;
            }

            return userReaction;
        }

        public async Task<List<IdeasCmtsPerDeptDTO>> IdeasCmtsPerDept()
        {
            var ideasByDept = (
                    from i in _dbContext.Ideas
                    join u in _dbContext.Users on i.UserId equals u.Id
                    join d in _dbContext.Departments on u.Department.Id equals d.Id
                    group d by d.Name into ideaGroup
                    select new IdeasPerDeptDTO
                    {
                        Name = ideaGroup.First().Name,
                        NumOfIdeas = ideaGroup.Count()
                    });

            var cmtsByDept = (
                    from c in _dbContext.Comments
                    join u in _dbContext.Users on c.UserId equals u.Id
                    join d in _dbContext.Departments on u.Department.Id equals d.Id
                    group d by d.Name into cmtGroup
                    select new CmtsPerDeptDTO
                    {
                        Name = cmtGroup.First().Name,
                        NumOfCmts = cmtGroup.Count()
                    });

            return (from i in ideasByDept
                    join c in cmtsByDept on i.Name equals c.Name
                    select new IdeasCmtsPerDeptDTO
                    {
                        Name = i.Name,
                        NumOfIdeas = i.NumOfIdeas,
                        NumOfCmts = c.NumOfCmts
                    }).ToList();
        }
    }
}
