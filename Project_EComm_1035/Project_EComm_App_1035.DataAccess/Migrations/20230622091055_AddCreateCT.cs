using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project_EComm_App_1035.DataAccess.Migrations
{
    public partial class AddCreateCT : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"CREATE PROCEDURE CreateCoverType
                                 @name varchar(50)
                                 AS
                               insert CoverTypes values(@name)");

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
