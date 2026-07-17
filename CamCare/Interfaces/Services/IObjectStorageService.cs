namespace CamCare.Interfaces.Services
{
    public interface IObjectStorageService
    {
        Task UploadAsync(
            string objectKey,
            Stream content,
            string contentType,
            IReadOnlyDictionary<string, string>? metadata = null,
            CancellationToken cancellationToken = default);
        Task DeleteAsync(string objectKey, CancellationToken cancellationToken = default);
        Task<Stream> DownloadAsync(string objectKey, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(string objectKey, CancellationToken cancellationToken = default);
        Task<string> GetReadUrlAsync(string objectKey, TimeSpan expiresIn);
    }
}
