using System;
using Bogus;
using Microsoft.EntityFrameworkCore.Migrations;
using Razorpage.models;

#nullable disable

namespace CS048_RazorPage8_EF.Migrations
{
    /// <inheritdoc />
    public partial class Initdb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "articles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Create = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Content = table.Column<string>(type: "ntext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_articles", x => x.Id);
                });
                //Insert Data
                /*migrationBuilder.InsertData(
                    table: "articles",
                    columns: new[] {"Title","Create","Content"},
                    values: new object[]{
                        "Bai viet 1",
                        new DateTime(2024,4,20),
                        "Noi dung thu 1"
                    }
                );

                migrationBuilder.InsertData(
                    table: "articles",
                    columns: new[] {"Title","Create","Content"},
                    values: new object[]{
                        "Bai viet 2",
                        new DateTime(2024,4,22),
                        "Noi dung thu 2"
                    }
                );*/

                //thư viện dùng để fake Data: Bogus
                Randomizer.Seed = new Random(8675309);

                var fakerArticle = new Faker<Article>();
                fakerArticle.RuleFor(a => a.Title, f => f.Lorem.Sentence(5,3)); //câu văn có tối thiểu 10 từ và có thể thêm bớt 5 từ
                fakerArticle.RuleFor(a => a.Create, f => f.Date.Between(new DateTime(2023,1,1), new DateTime(2024,1,1)));
                fakerArticle.RuleFor(a => a.Content, f => f.Lorem.Paragraphs(1, 4));// dài từ 1 đến 4 paragraphs

                for (int i = 0; i < 150; i++)
                {
                    Article article = fakerArticle.Generate();
                    migrationBuilder.InsertData(
                        table: "articles",
                        columns: new[] {"Title","Create","Content"},
                        values: new object[]{
                            article.Title,
                            article.Create,
                            article.Content
                        }
                    );
                }
        
        }


        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "articles");
        }
    }
}
