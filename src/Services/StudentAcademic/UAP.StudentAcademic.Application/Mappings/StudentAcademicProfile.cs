using AutoMapper;
using UAP.StudentAcademic.Application.Queries;
using UAP.StudentAcademic.Domain.Entities;

namespace UAP.StudentAcademic.Application.Mappings;

public class StudentAcademicProfile : Profile
{
    public StudentAcademicProfile()
    {
        CreateMap<Student, StudentDto>()
            .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => "Department Name")) // Would come from department service
            .ForMember(dest => dest.IsGoodStanding, opt => opt.MapFrom(src => src.CurrentCGPA >= 2.0m))
            .ForMember(dest => dest.IsDeanList, opt => opt.MapFrom(src => src.CurrentCGPA >= 3.5m));

        CreateMap<CourseGrade, GradeDto>()
            .ForMember(dest => dest.Grade, opt => opt.MapFrom(src => src.Grade.ToString()))
            .ForMember(dest => dest.GradePoints, opt => opt.MapFrom(src => src.GetGradePoints()))
            .ForMember(dest => dest.IsPassing, opt => opt.MapFrom(src => src.IsPassingGrade()));

        CreateMap<AcademicSemester, AcademicSemesterDto>()
            .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.GetDisplayName()));
    }
}