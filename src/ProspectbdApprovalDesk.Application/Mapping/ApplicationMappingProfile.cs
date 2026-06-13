using AutoMapper;
using ProspectbdApprovalDesk.Application.ActivityLogs.Dto;
using ProspectbdApprovalDesk.Application.Projects.Dto;
using ProspectbdApprovalDesk.Application.Users.Dto;
using ProspectbdApprovalDesk.Domain.Entities;

namespace ProspectbdApprovalDesk.Application.Mapping;

public sealed class ApplicationMappingProfile : Profile
{
    public ApplicationMappingProfile()
    {
        CreateMap<User, UserDto>();
        CreateMap<Project, ProjectListItemDto>();
        CreateMap<ActivityLog, ActivityLogDto>();
    }
}

