using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IdentityDotnet7.Api.Models.SeedDefaultUser;

public static class SeedDefaultUser
{
    public static void RegisterDefaultUser(ModelBuilder modelBuilder)
    {
        var user = UserInfo();
        var adminRole = RoleAdmin();
        var userRoles = RoleUser();
        var userRole = UserRoleInfo(user.Id, adminRole.Id);

        modelBuilder.Entity<IdentityUser>().HasData(user);
        modelBuilder.Entity<IdentityRole>().HasData(adminRole, userRoles);
        // User HasNoKey() since the userId and roleId is a unique foreign key in a many-to-many relationship
        modelBuilder.Entity<IdentityUserRole<string>>().HasData(userRole);
    }

    //Add User
    private static IdentityUser UserInfo()
    {
        var user = new IdentityUser
        {
            Id = Guid.NewGuid().ToString(),
            Email = "admin@admin.com",
            EmailConfirmed = true,
            UserName = "Administrator"
        };

        var hasher = new PasswordHasher<IdentityUser>();
        hasher.HashPassword(user, "admin123");

        return user;
    }
    
    //Add Admin Role
    private static IdentityRole RoleAdmin() =>
        new()
        {
            Id = Guid.NewGuid().ToString(),
            Name = "admin",
            NormalizedName = "Administrator"
        };
    
    //Add Admin Role
    private static IdentityRole RoleUser() =>
        new()
        {
            Id = Guid.NewGuid().ToString(),
            Name = "user",
            NormalizedName = "User"
        };
    
    //Add User Role
    private static IdentityUserRole<string> UserRoleInfo(string userId, string roleId) =>
        new()
        {
            RoleId = roleId,
            UserId = userId
        };
}