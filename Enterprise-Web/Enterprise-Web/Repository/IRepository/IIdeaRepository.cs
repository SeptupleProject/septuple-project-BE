﻿using Enterprise_Web.DTOs;
using Enterprise_Web.Models;
using Enterprise_Web.Pagination.Filter;
using Enterprise_Web.ViewModels;

namespace Enterprise_Web.Repository.IRepository
{
    public interface IIdeaRepository
    {
        (List<IdeaDTO>, PaginationFilter, int) GetAll(PaginationFilter filter);
        Idea GetById(int id);
        Task Create(Idea idea, int userId, string userEmail);
        Task Update(Idea idea);
        Task Delete(int id);
        Task Download(string zipFile); 
        List<IdeaViewModel> GetIdeasToDownload();
        Task DownloadIdeas(List<IdeaViewModel> ideaViewModels, List<CommentViewModel> commentViewModels);
        Task IcrementedView(int id);
        Task<List<IdeaDTO>> MostLikeIdea();
        Task<List<IdeaDTO>> MostDislikeIdea();
        Task<List<IdeaDTO>> MostCommentIdea();
        Task<List<IdeaDTO>> MostViewsIdea();
        Task<Reaction> LikeIdea(int userId, Idea idea); 
        Task<Reaction> DislikeIdea(int userId, Idea idea); 
        Task<List<IdeasCmtsPerDeptDTO>> IdeasCmtsPerDept();
    }
}
