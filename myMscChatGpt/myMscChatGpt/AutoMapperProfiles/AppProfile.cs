using AutoMapper;
using myMscChatGpt.Repositories.Entities;
using myMscChatGpt.Services;

namespace myMscChatGpt.AutoMapperProfiles
{
    public class AppProfile : Profile
    {
        public AppProfile()
        {
            CreateMap<EmbeddingForDb, Embedding>();
        }
    }
}
