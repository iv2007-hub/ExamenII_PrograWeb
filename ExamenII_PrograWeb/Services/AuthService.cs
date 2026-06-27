using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using ExamenII_PrograWeb.DTOs;
using ExamenII_PrograWeb.Models;


namespace ExamenII_PrograWeb.Services;


public class AuthService
{
    private readonly FirestoreService _firestoreService;
    private readonly IConfiguration _configuration;

    public AuthService(FirestoreService firestoreService, IConfiguration configuration)
    {
        _firestoreService = firestoreService;
        _configuration = configuration;
    }

    public async Task<User> Register(RegisterRequestDto dto)
    {
        var collection = _firestoreService.GetCollection("users");
        var existing = await collection.WhereEqualTo("Email", dto.Email).GetSnapshotAsync();

        if (existing.Count > 0)
            throw new Exception("Ya existe un usuario con ese correo");

        var user = new User
        {
            Id = Guid.NewGuid().ToString(),
            FullName = dto.FullName,
            Email = dto.Email,
            PasswordHash = HashPassword(dto.Password)
        };

        await collection.Document(user.Id).SetAsync(new Dictionary<string, object>
        {
            { "Id", user.Id },
            { "FullName", user.FullName },
            { "Email", user.Email },
            { "PasswordHash", user.PasswordHash }
        });

        return user;
    }

    public async Task<string> Login(LoginRequestDto dto)
    {
        var collection = _firestoreService.GetCollection("users");
        var snapshot = await collection.WhereEqualTo("Email", dto.Email).GetSnapshotAsync();

        if (snapshot.Count == 0)
            throw new Exception("Credenciales inválidas");

        var data = snapshot.Documents[0].ToDictionary();

        var user = new User
        {
            Id = data["Id"].ToString()!,
            FullName = data["FullName"].ToString()!,
            Email = data["Email"].ToString()!,
            PasswordHash = data["PasswordHash"].ToString()!
        };

        if (!VerifyPassword(dto.Password, user.PasswordHash))
            throw new Exception("Credenciales inválidas");

        return GenerateToken(user);
    }

    private string GenerateToken(User user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    
    private bool VerifyPassword(string dtoPassword, string userPasswordHash)
    {
        return HashPassword(dtoPassword) == userPasswordHash;
    }

    private string HashPassword(string password)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }
}