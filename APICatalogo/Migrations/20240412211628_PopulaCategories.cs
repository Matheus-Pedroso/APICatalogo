using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APICatalogo.Migrations
{
    /// <inheritdoc />
    public partial class PopulaCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder mb)
        {
            mb.Sql(" Insert into Categories(Name, ImageUrl) Values('Bebidas', 'bebidas.jpg')");
            mb.Sql(" Insert into Categories(Name, ImageUrl) Values('Lanches', 'lanches.jpg')");
            mb.Sql(" Insert into Categories(Name, ImageUrl) Values('Sobremesas', 'sobremesas.jpg')");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder mb)
        {
            mb.Sql("Delete from Categories");
        }
    }
}
