using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmpresaT3.Migrations
{
    public partial class SeedRoles : Migration
    {
        private string ManagerRoleId = Guid.NewGuid().ToString();
        private string UserRoleId = Guid.NewGuid().ToString();
        private string AdminRoleId = Guid.NewGuid().ToString();

        private string User1Id = Guid.NewGuid().ToString();
        private string User2Id = Guid.NewGuid().ToString();

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            SeedRolesSQL(migrationBuilder);

            SeedUser(migrationBuilder);

            SeedUserRoles(migrationBuilder);
        }

        private void SeedRolesSQL(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@$"INSERT INTO [dbo].[AspNetRoles] ([Id],[Name],[NormalizedName],[ConcurrencyStamp])
            VALUES ('{AdminRoleId}', 'Administrator', 'ADMINISTRATOR', null);");
            migrationBuilder.Sql(@$"INSERT INTO [dbo].[AspNetRoles] ([Id],[Name],[NormalizedName],[ConcurrencyStamp])
            VALUES ('{ManagerRoleId}', 'Manager', 'MANAGER', null);");
            migrationBuilder.Sql(@$"INSERT INTO [dbo].[AspNetRoles] ([Id],[Name],[NormalizedName],[ConcurrencyStamp])
            VALUES ('{UserRoleId}', 'User', 'USER', null);");
        }

        private void SeedUser(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.Sql(
                @$"INSERT [dbo].[AspNetUsers] ([Id], [FirstName], [LastName], [UserName], [NormalizedUserName], 
[Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], 
[PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount]) 
VALUES 
(N'{User1Id}', N'Admin', N'Admin', N'empresat3@gmail.com', N'EMPRESAT3@GMAIL.COM', 
N'empresat3@gmail.com', N'EMPRESAT3@GMAIL.COM', 0, 
N'AQAAAAEAACcQAAAAEE0IhLXvhvDC71LV+2M8gOj65TJ8v6fBvpiVj0stFcdrAOseEdMY2f+Fdn1JJREvrw==', 
N'VUMWTI3D7G6R5JGR2VLNA5RTXC3PFDNM', N'7fcb9bb1-be91-4848-baae-ff12ad75b6d3', NULL, 0, 0, NULL, 1, 0)");

        }


        private void SeedUserRoles(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.Sql(@$"
        INSERT INTO [dbo].[AspNetUserRoles]
           ([UserId]
           ,[RoleId])
        VALUES
           ('{User1Id}', '{AdminRoleId}');
        INSERT INTO [dbo].[AspNetUserRoles]
           ([UserId]
           ,[RoleId])
        VALUES
           ('{User1Id}', '{ManagerRoleId}');");

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
