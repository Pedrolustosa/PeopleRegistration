namespace PeopleRegistration.Application.DTOs;

public record TokenResult(string Jwt, DateTime ExpiresAt);
