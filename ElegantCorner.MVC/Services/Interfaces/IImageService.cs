using Microsoft.AspNetCore.Http;

namespace ElegantCorner.MVC.Interfaces
{
    public interface IImageService
    {
        Task<string?> UploadAsync(IFormFile file, string folderName);

        void Delete(string imagePath);
    }
}