using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Epam.ItMarathon.ApiService.Application.UseCases.User.Commands;
using Epam.ItMarathon.ApiService.Domain.Abstract;
using Epam.ItMarathon.ApiService.Domain.Shared.ValidationErrors;
using FluentValidation.Results;
using MediatR;
using RoomAggregate = Epam.ItMarathon.ApiService.Domain.Aggregate.Room.Room;

namespace Epam.ItMarathon.ApiService.Application.UseCases.User.Handlers
{
    public class DeleteUserHandler : IRequestHandler<DeleteUserRequest, Result<RoomAggregate, ValidationResult>>
    {
        private readonly IRoomRepository _roomRepository;

        public DeleteUserHandler(IRoomRepository roomRepository)
        {
            _roomRepository = roomRepository;
        }

        ///<inheritdoc/>
        public async Task<Result<RoomAggregate, ValidationResult>> Handle(DeleteUserRequest request,
            CancellationToken cancellationToken)
        {
            // 1. Get room by user code
            var roomResult = await _roomRepository.GetByUserCodeAsync(request.UserCode, cancellationToken);
            if (roomResult.IsFailure)
            {
                return Result.Failure<RoomAggregate, ValidationResult>(roomResult.Error);
            }

            // 2. Delete user in room's users
            var room = roomResult.Value;
            var deleteResult = room.DeleteUser(request.UserId);
            if (deleteResult.IsFailure)
            {
                return Result.Failure<RoomAggregate, ValidationResult>(deleteResult.Error);
            }

            // 3. Update room in repository
            var updateResult = await _roomRepository.UpdateAsync(room, cancellationToken);
            if (updateResult.IsFailure)
            {
                // Convert update error (string) into ValidationFailure for API error payload
                return Result.Failure<RoomAggregate, ValidationResult>(
                    new BadRequestError(new[] { new ValidationFailure(string.Empty, updateResult.Error) })
                );
            }

            // 4. Get updated room
            var updatedRoomResult = await _roomRepository.GetByUserCodeAsync(request.UserCode, cancellationToken);
            if (updatedRoomResult.IsFailure)
            {
                return Result.Failure<RoomAggregate, ValidationResult>(updatedRoomResult.Error);
            }

            return Result.Success<RoomAggregate, ValidationResult>(updatedRoomResult.Value);
        }
    }
}
