using Application.DTOs;
using Core.Entities;
using Core.Interfaces;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class UserService
{
    private readonly IRepository<User> _userRepository;

    public UserService(IRepository<User> userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<User> Register(UserDto userDto)
    {
        var user = new User
        {
            Username = userDto.Username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(userDto.Password),
            Role = "User"
        };
        await _userRepository.AddAsync(user);
        return user;
    }

    public async Task<User?> Login(UserDto userDto)
    {
        var users = await _userRepository.GetAllAsync();
        var user = users.FirstOrDefault(u => u.Username == userDto.Username);
        if (user != null && BCrypt.Net.BCrypt.Verify(userDto.Password, user.PasswordHash))
        {
            return user;
        }
        return null;
    }

    public async Task<User?> GetUser(int userId) => await _userRepository.GetByIdAsync(userId);
}