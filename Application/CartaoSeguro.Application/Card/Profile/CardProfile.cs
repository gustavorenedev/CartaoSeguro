using CartaoSeguro.Application.Card.Request;
using CartaoSeguro.Application.Card.Response;

namespace CartaoSeguro.Application.Card.Profile;

public class CardProfile : AutoMapper.Profile
{
    public CardProfile()
    {
        CreateMap<Domain.Card.Card, CreateCardRequest>().ReverseMap();
        CreateMap<CreateCardResponse, Domain.Card.Card>().ReverseMap();
        CreateMap<Domain.Card.Card, CardResponse>().ReverseMap();

        CreateMap<List<Domain.Card.Card>, CardsByUserResponse>()
            .ForMember(dest => dest.Cards, opt => opt.MapFrom(src => src));

        CreateMap<CardsByUserResponse, List<Domain.Card.Card>>()
            .ConstructUsing(src => src.Cards!.Select(cardResponse => new Domain.Card.Card
            {
                Id = cardResponse.Id,
                Number = cardResponse.Number,
                Type = cardResponse.Type,
                CardFlag = cardResponse.CardFlag,
                Status = cardResponse.Status
            }).ToList() ?? new List<Domain.Card.Card>());

        CreateMap<CardByIdResponse, Domain.Card.Card>().ReverseMap();
    }
}
