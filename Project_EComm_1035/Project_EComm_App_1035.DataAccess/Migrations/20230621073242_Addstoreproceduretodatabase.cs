using Microsoft.EntityFrameworkCore.Migrations;
using Project_EComm_App_1035.Model;

#nullable disable

namespace Project_EComm_App_1035.DataAccess.Migrations
{
    public partial class Addstoreproceduretodatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           migrationBuilder.Sql(@"CREATE PROCEDURE CoverType
                                 @name varchar(50)
                                 AS
                               insert CoverTypes values(@name)");

            migrationBuilder.Sql(@"CREATE PROCEDURE updateCoverType
                                 @id int,
                                 @name varchar(50)
                                 AS
                                 update CoverTypes set name=@name where id=@id");

            migrationBuilder.Sql(@"CREATE PROCEDURE DeleteCoverType
                                 @id int    
                                 AS
                                 Delete from CoverTypes where id=@id");

            migrationBuilder.Sql(@"CREATE PROCEDURE GetCoverTypes
                                 AS
                                 select * from CoverTypes");

            migrationBuilder.Sql(@"CREATE PROCEDURE GetCoverType
                                 @id int
                                 AS
                                select * from CoverTypes where id=@id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
