namespace WebApplication3.Repository
{
    public interface IImageRepository
    {
        Task<string> UploadAsync(IFormFile file);
    }
}
