using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ecomm_project_clothes.Dataaccess.Migrations
{
    public partial class sp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"CREATE PROCEDURE CreateClothestypes
                 @name varchar(50)
                 AS
                  Insert ClothesTypes values(@name)");

            migrationBuilder.Sql(@"CREATE PROCEDURE UpdateClothestypes
                 @id int,
                 @name varchar(50)
                 AS
                  Update ClothesTypes set name=@name where id=@id");

            migrationBuilder.Sql(@"CREATE PROCEDURE DeleteClothestypes
                 @id int
                 AS
                  Delete ClothesTypes where id=@id");

            migrationBuilder.Sql(@"CREATE PROCEDURE GetClothestypes
                 AS
                  Select * from ClothesTypes");
            migrationBuilder.Sql(@"CREATE PROCEDURE GetClothestype
                 @id int
                 AS
                  Select * from ClothesTypes where id=@id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
