﻿using Enterprise_Web.DTOs;
using Enterprise_Web.Models;
using Enterprise_Web.Pagination.Filter;
using Enterprise_Web.Repository.IRepository;
using Enterprise_Web.ViewModels;
using EnterpriseWeb.Data;
using EnterpriseWeb.Models;
using Microsoft.EntityFrameworkCore;

namespace Enterprise_Web.Repository
{
    public class CommentRepository : ICommentRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly INotificationRepository _notificationRepository;

        public CommentRepository(ApplicationDbContext dbContext, INotificationRepository notificationRepository)
        {
            _dbContext = dbContext;
            _notificationRepository = notificationRepository;
        }
        public (List<CommentDTO>, PaginationFilter, int) GetAll(PaginationFilter filter)
        {
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize, filter.Search, filter.Role);
            var listComments = (from comment in _dbContext.Comments
                                select
                                 new CommentDTO
                                 {
                                     Id = comment.Id,
                                     Content = comment.Content,
                                     IsAnonymous = comment.IsAnonymous,
                                 })
                               .OrderByDescending(x => x.Id)
                               .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
                               .Take(validFilter.PageSize)
                               .ToList();
            var countComments = (from comment in _dbContext.Comments select comment).Count();

            return (listComments, validFilter, countComments);
        }

        public Comment GetById(int id)
        {
            var commentToGet = _dbContext.Comments.FirstOrDefault(i => i.Id == id);
            if (commentToGet == null)
            {
                return null;
            }
            return commentToGet;
        }

        public async Task Create(Comment comment, int userId, string userEmail)
        {
            var newComment = new Comment()
            {
                Content = comment.Content,
                IsAnonymous = comment.IsAnonymous,
                UserId = userId,
                IdeaId = comment.IdeaId,
                CreatedBy = userEmail,
                CreatedAt = DateTime.Now,
            };
            await _dbContext.AddAsync(newComment);
            await _dbContext.SaveChangesAsync();

            var newNoti = new NotificationViewModel()
            {
                IdeaId = 6,
                CreatedBy = userEmail
            };
            await _notificationRepository.CheckAndSend(newNoti);
        }

        public async Task Delete(int id)
        {
            var commentDelete = await _dbContext.Comments.FindAsync(id);
            _dbContext.Remove(commentDelete);
            await _dbContext.SaveChangesAsync();
        }

        public async Task Update(Comment comment)
        {
            var findComments = await _dbContext.Comments.FirstOrDefaultAsync(x => x.Id == comment.Id);
            if (findComments != null)
            {
                findComments.Content = comment.Content;
                findComments.CreatedAt = DateTime.Now;
            }
            _dbContext.Update(findComments);
            await _dbContext.SaveChangesAsync();
        }
    }
}
