using AutoMapper;
using SocialNetworkBE.Models;

namespace SocialNetworkBE.Profiles
{
    public class AutomapperProfiles : Profile
    {
        public AutomapperProfiles()
        {
            CreateMap<RegisterNewAccountModel, Account>();
            CreateMap<UpdateAccountModel, Account>();
            CreateMap<Account, GetAccountModel>();
            CreateMap<TransactionRequestDto, Transaction>();
        }
    }
}
