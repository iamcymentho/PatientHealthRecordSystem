using AutoMapper;
using PHR.Application.AccessRequests.Commands;
using PHR.Application.AccessRequests.Queries;
using PHR.Application.Auth.Commands;
using PHR.Application.DTOs;
using PHR.Application.PatientRecords.Commands;
using PHR.Application.PatientRecords.Queries;
namespace PHR.Application.Mapping
{
	public class ApiMappingProfile : Profile
	{
		public ApiMappingProfile()
		{
			// Access Request mappings
			CreateMap<CreateAccessRequestDto, CreateAccessRequestCommand>()
				.ConstructUsing((src, context) => new CreateAccessRequestCommand(
					src.PatientRecordId,
					src.Reason,
					context.Items["UserId"] as Guid? ?? Guid.Empty));
			CreateMap<ApproveAccessRequestDto, ApproveAccessRequestCommand>()
				.ConstructUsing((src, context) => new ApproveAccessRequestCommand(
					context.Items["AccessRequestId"] as Guid? ?? Guid.Empty,
					src.ApprovedStartUtc,
					src.ApprovedEndUtc,
					context.Items["UserId"] as Guid? ?? Guid.Empty));
			CreateMap<DeclineAccessRequestDto, DeclineAccessRequestCommand>()
				.ConstructUsing((src, context) => new DeclineAccessRequestCommand(
					context.Items["AccessRequestId"] as Guid? ?? Guid.Empty,
					src.DeclineReason,
					context.Items["UserId"] as Guid? ?? Guid.Empty));
			// Patient Record mappings
			CreateMap<CreatePatientRecordRequest, CreatePatientRecordCommand>()
				.ConstructUsing((src, context) => new CreatePatientRecordCommand(
					src.PatientName,
					src.DateOfBirth,
					src.Diagnosis,
					src.TreatmentPlan,
					src.MedicalHistory,
					context.Items["UserId"] as Guid? ?? Guid.Empty));
			CreateMap<UpdatePatientRecordRequest, UpdatePatientRecordCommand>()
				.ConstructUsing((src, context) => new UpdatePatientRecordCommand(
					context.Items["Id"] as Guid? ?? Guid.Empty,
					src.PatientName,
					src.DateOfBirth,
					src.Diagnosis,
					src.TreatmentPlan,
					src.MedicalHistory,
					context.Items["UserId"] as Guid? ?? Guid.Empty));
			// Auth mappings
			CreateMap<RegisterUserCommand, RegisterUserCommand>(); // Already a command
			CreateMap<LoginCommand, LoginCommand>(); // Already a command
			CreateMap<AdminCreateUserCommand, AdminCreateUserCommand>(); // Already a command
			// Query mappings - create methods for these since queries need specific constructors
		}
		// Extension methods to create queries with context
		public static GetPendingAccessRequestsQuery CreateGetPendingAccessRequestsQuery(Guid userId)
		{
			return new GetPendingAccessRequestsQuery(userId);
		}
		public static GetAccessRequestByIdQuery CreateGetAccessRequestByIdQuery(Guid id, Guid userId)
		{
			return new GetAccessRequestByIdQuery(id, userId);
		}
		public static GetPatientRecordsQuery CreateGetPatientRecordsQuery(Guid userId)
		{
			return new GetPatientRecordsQuery(userId);
		}
		public static GetPatientRecordByIdQuery CreateGetPatientRecordByIdQuery(Guid id, Guid userId)
		{
			return new GetPatientRecordByIdQuery(id, userId);
		}
		public static DeletePatientRecordCommand CreateDeletePatientRecordCommand(Guid id, Guid userId)
		{
			return new DeletePatientRecordCommand(id, userId);
		}
	}
}
