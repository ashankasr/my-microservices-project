using System;
using System.ComponentModel.DataAnnotations;

namespace Play.Identity.Service.Dtos
{
    public record UserDto(Guid Id, string UserName, string Email, decimal gil, DateTimeOffset CreatedDate);
    public record UpdateUserDto(
        Guid Id,
        [Required][EmailAddress] string Email,
        [Range(1, 1000000)] decimal Gil);
}