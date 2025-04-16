
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using WebApplication3.Repository;

namespace WebApplication5.Repository
{
    public class CloudImageRepository : IImageRepository
    {
        private readonly IConfiguration configuration;
        private readonly Account account;
        public CloudImageRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
            account = new Account(
                configuration.GetSection("Cloudinary")["CloudName"],
                configuration.GetSection("Cloudinary")["ApiKey"],
                configuration.GetSection("Cloudinary")["ApiSecret"]);
        }

        public IConfiguration Configuration => configuration;

        public async Task<string?> UploadAsync(IFormFile file)
        {
            var client = new Cloudinary(account);
            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(file.FileName, file.OpenReadStream()),
                DisplayName = file.FileName,
                UniqueFilename = false,
                Overwrite = true
            };
            var uploadResult = await client.UploadAsync(uploadParams);
            if (uploadResult != null && uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return uploadResult.SecureUrl.ToString();
            }
            return null;
        }
    }
}
