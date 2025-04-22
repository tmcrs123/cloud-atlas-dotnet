using Cloud_Atlas_Dotnet.Application.Commands;
using Cloud_Atlas_Dotnet.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Cloud_Atlas_Dotnet.Infrastructure.Database
{
    public interface IRepository
    {
        Task<Guid> CreateUser(string name, string username, string email, string password);
        Task<bool> UsernameExists(string username);
        Task<User> GetUser([FromQuery] Guid id);
        Task<bool> UpdateUser(string password, Guid id);
        Task<bool> DeleteUser(Guid id);
        Task<bool> VerifyAccount(Guid userId);
        Task<Atlas> CreateAtlas(string title, Guid userId);
        Task<bool> UpdateAtlas(Guid atlasId, string title);
        Task<List<Atlas>> GetAtlasForUser(Guid userId);
        Task<bool> DeleteAtlas(Guid atlasId);
        Task<bool> AddImageToAtlas(Guid atlasId, string legend, Uri imageUri);
        Task<List<Image>> GetImagesForAtlas(Guid atlasId);
        Task<IResult> UpdateImageDetails(UpdateImageCommand request);
        Task<IResult> DeleteImage(DeleteImageCommand request);

    }
}
