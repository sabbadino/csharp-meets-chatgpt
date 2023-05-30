using AutoMapper;
using ChatGptBot.Repositories.Entities;
using ChatGptBot.Services;

namespace ChatGptBot.AutoMapperProfiles
{
    public class AppProfile : Profile
    {
        public AppProfile()
        {
            CreateMap<EmbeddingForDb, Embedding>();
        }
    }
}
