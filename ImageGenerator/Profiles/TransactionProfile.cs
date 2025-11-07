using AutoMapper;
using ImageGenerator.Dtos;
using ImageGenerator.Models;

namespace ImageGenerator.Profiles;

public class TransactionProfile : Profile
{
    public TransactionProfile()
    {
        CreateMap<Transaction, TransactionDto>();
    }
}
