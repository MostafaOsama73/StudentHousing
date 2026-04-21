using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Business.Models.Requests;
using Business.Models.Responses;
using Domain.Entities;

namespace Business.Mappers;

/// <summary>
/// AutoMapper configuration for mapping between entities, requests, and responses
/// </summary>
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        #region User Mappings

        // User to UserResponse
        CreateMap<User, UserResponse>()
            .ForMember(dest => dest.Roles, opt => opt.Ignore()); // Roles are handled separately in service

        #endregion

        #region Student Mappings

        // StudentRegisterRequest to Student
        CreateMap<StudentRegisterRequest, Student>()
            .ForMember(dest => dest.StudentId, opt => opt.Ignore()) // Generated in service
            .ForMember(dest => dest.UserId, opt => opt.Ignore()) // Set in service
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.Bookings, opt => opt.Ignore())
            .ForMember(dest => dest.Reviews, opt => opt.Ignore())
            .ForMember(dest => dest.Complaints, opt => opt.Ignore())
            .ForMember(dest => dest.Wishlists, opt => opt.Ignore());

        // Student to StudentResponse
        CreateMap<Student, StudentResponse>()
            .ForMember(dest => dest.VerificationStatus, opt => opt.MapFrom(src => src.VerificationStatus));

        // StudentUpdateRequest to Student
        CreateMap<StudentUpdateRequest, Student>()
            // 1. Put all your specific ignores FIRST
            .ForMember(dest => dest.StudentId, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.Bookings, opt => opt.Ignore())
            .ForMember(dest => dest.Reviews, opt => opt.Ignore())
            .ForMember(dest => dest.Complaints, opt => opt.Ignore())
            .ForMember(dest => dest.Wishlists, opt => opt.Ignore())
            // 2. Put ForAllMembers at the VERY END of the chain!
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        CreateMap<Student, StudentRegisterRequest>()
            .ForMember(dest => dest.Email, opt => opt.Ignore())
            .ForMember(dest => dest.PhoneNumber, opt => opt.Ignore())
            .ForMember(dest => dest.Password, opt => opt.Ignore())
            .ForMember(dest => dest.ConfirmPassword, opt => opt.Ignore())
            .ReverseMap();

        #endregion

        #region LandLord Mappings

        // LandLordRegisterRequest to LandLord
        CreateMap<LandLordRegisterRequest, LandLord>()
            .ForMember(dest => dest.LandLordId, opt => opt.Ignore()) // Generated in service
            .ForMember(dest => dest.UserId, opt => opt.Ignore()) // Set in service
            .ForMember(dest => dest.VerificationStatus, opt => opt.Ignore()) // Set to "Pending" in service
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.HousingUnits, opt => opt.Ignore());

        // LandLord to LandLordRegisterRequest (reverse if needed)
        CreateMap<LandLord, LandLordRegisterRequest>()
            .ForMember(dest => dest.Email, opt => opt.Ignore())
            .ForMember(dest => dest.PhoneNumber, opt => opt.Ignore())
            .ForMember(dest => dest.Password, opt => opt.Ignore())
            .ForMember(dest => dest.ConfirmPassword, opt => opt.Ignore())
            .ReverseMap();

        #endregion

        #region Token Mappings

        // Token-related mappings (if needed in the future)

        #endregion
    }
}

