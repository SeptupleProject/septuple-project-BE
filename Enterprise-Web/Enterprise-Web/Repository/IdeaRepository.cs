using CsvHelper;
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
                             CategoryName = idea.Category.Name
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
      var fileUpload = idea.File;
      var name = fileUpload.FileName;
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

      var newIdea = new Idea()
      {
        Title = idea.Title,
        Content = idea.Content,
        Views = idea.Views,
        Image = image,
        IsAnonymos = idea.IsAnonymos,
        AcademicYearId = idea.AcademicYearId,
        CategoryId = idea.CategoryId,
        UserId = userId,
        CreatedBy = userEmail,
        CreatedAt = DateTime.Now,
      };

      await _dbContext.AddAsync(newIdea);
      await _dbContext.SaveChangesAsync();

      var newNoti = new NotificationViewModel()
      {
        IdeaId = null,
        CreatedBy = userEmail
      };
      await _notificationRepository.CheckAndSend(newNoti);
    }


    public async Task Update(Idea idea)
    {
      var fileUpload = idea.File;
      var name = fileUpload.FileName;
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
        findIdeas.CategoryId = idea.CategoryId;
      }
      _dbContext.Update(findIdeas);
      await _dbContext.SaveChangesAsync();
    }

    public async Task Delete(int id)
    {
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

    public Task DownloadIdeas(List<IdeaViewModel> ideaViewModels)
    {
      string pathUser = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
      string csvPath = Path.Combine(pathUser, $"Downloads\\ideas-{DateTime.Now.ToFileTime()}.csv");

      using (var streamWriter = new StreamWriter(csvPath))
      {
        using (var csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture))
        {
          csvWriter.Context.RegisterClassMap<IdeasClassMap>();
          csvWriter.WriteRecords(ideaViewModels);
        }
      }
      return Task.CompletedTask;
    }

    public async Task IcrementedView(Idea idea)
    {
      var findIdeas = await _dbContext.Ideas.FirstOrDefaultAsync(x => x.Id == idea.Id);
      if (findIdeas != null)
      {
        findIdeas.Views = idea.Views + 1;
      }
      _dbContext.Update(findIdeas);
      await _dbContext.SaveChangesAsync();
    }
  }
}
