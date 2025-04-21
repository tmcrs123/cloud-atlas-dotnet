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
        Task<IResult> UpdateUser(UpdateUserCommand request);
        Task<IResult> DeleteUser(DeleteUserCommand request);
        Task<IResult> CreateAtlas(CreateAtlasCommand request);
        Task<IResult> GetAtlasForUser(Guid userId);
        Task<IResult> UpdateAtlas(UpdateAtlasCommand request);
        Task<IResult> DeleteAtlas(DeleteAtlasCommand request);
        Task<IResult> AddImageToAtlas(CreateImageCommand request);
        Task<IResult> GetImagesForAtlas([FromQuery] GetImagesForAtlasCommand request);
        Task<IResult> UpdateImageDetails(UpdateImageCommand request);
        Task<IResult> DeleteImage(DeleteImageCommand request);

    }
}
