using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APICatalogo.Migrations
{
    /// <inheritdoc />
    public partial class PopulaProducts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder mb)
        {
            mb.Sql(" Insert into Product(Name, Description, Price, ImageUrl, Balance, DateRegister, CategoryId) " +
                "Values('Coca-Cola Diet', '350ml Lata', 5.45, 'cocacola.png',50, now(), 1)");

            mb.Sql(" Insert into Product(Name, Description, Price, ImageUrl, Balance, DateRegister, CategoryId) " +
                "Values('Lanche Natural de Atum', 'Lanche com atum elatado', 8.50, 'lancheatum.png',10, now(), 2)");

            mb.Sql(" Insert into Product(Name, Description, Price, ImageUrl, Balance, DateRegister, CategoryId) " +
                "Values('Pudim 200 g', 'Pudim ao Leite', 7.25, 'pudimleite.png',20, now(), 3)");




        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder mb)
        {
            mb.Sql("Delete from Product");
        }
    }
}
