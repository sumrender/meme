using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "meme_templates",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    url = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    example = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_meme_templates", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    credits = table.Column<int>(type: "integer", nullable: false, defaultValue: 5)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id);
                });

            migrationBuilder.InsertData(
                table: "meme_templates",
                columns: new[] { "id", "description", "example", "name", "url" },
                values: new object[,]
                {
                    { 1, "A baby holding sand in his fist looking determined, representing unexpected success or minor victories.", "Me finding a 10-dollar bill in my old winter coat.", "successKid", "https://res.cloudinary.com/dplzxrvqy/image/upload/v1734749139/meme_templates/wgrmqej6jktwoh8qwdt8.webp" },
                    { 2, "A skeleton sitting on a bench, representing a long or endless wait for something.", "Me waiting for my food delivery that I ordered 1 minute ago.", "waitingSkeleton", "https://res.cloudinary.com/dplzxrvqy/image/upload/v1734749140/meme_templates/bsorrrb59szpbxvlqfzh.jpg" },
                    { 3, "An elderly woman looking closely at a laptop screen with a happy, naive expression.", "Me looking for the download button on a shady torrent site.", "grandmaFindsTheInternet", "https://res.cloudinary.com/dplzxrvqy/image/upload/v1734749140/meme_templates/ohjrh7u1s5yo2k8wg2qf.jpg" },
                    { 4, "A young girl smiling deviously at the camera while a house burns in the background.", "When you carry the team but your team acts like they contributed as much.", "disasterGirl", "https://res.cloudinary.com/dplzxrvqy/image/upload/v1734749140/meme_templates/b2eenvdnj2bwjobya5b9.jpg" },
                    { 5, "Two muscular arms shaking hands, representing agreement or common ground between two different parties.", "C++ developers and rust developers both hating JavaScript.", "epicHandshake", "https://res.cloudinary.com/dplzxrvqy/image/upload/v1734749140/meme_templates/dnhvaz7ifh2qbpzrvgue.jpg" },
                    { 6, "Leonardo DiCaprio laughing while holding a cocktail in Django Unchained, conveying mocking amusement or sarcasm.", "When your friend finally pays you back.", "laughingLeo", "https://res.cloudinary.com/dplzxrvqy/image/upload/v1734749141/meme_templates/qrwnaqiosoly5o4cwmjx.png" },
                    { 7, "A puppet monkey looking forward then turning its eyes away, representing avoiding eye contact or ignoring an obvious truth.", "Me ignoring my compile errors and hoping they go away on the next run.", "monkeyLookingAway", "https://res.cloudinary.com/dplzxrvqy/image/upload/v1734749140/meme_templates/mfmddhpqbba09ks3ytyi.jpg" },
                    { 8, "Leonardo DiCaprio smiling and raising a glass in The Great Gatsby, expressing approval, congratulations, or cheers.", "To the one guy on StackOverflow who answered my exact problem in 2012.", "leonardoDiCaprioCheers", "https://res.cloudinary.com/dplzxrvqy/image/upload/v1734749140/meme_templates/esnqhfjcm5w8bnzp7g7z.jpg" },
                    { 9, "A woman whispering into another woman's ear, causing an extreme reaction of goosebumps, expressing a highly exciting statement.", "Developer: \"We are finally deleting the legacy codebase tomorrow.\"", "whisperAndGoosebumps", "https://res.cloudinary.com/dplzxrvqy/image/upload/v1734749140/meme_templates/yigvofyucfigmwc0jee7.jpg" },
                    { 10, "A cartoon dog character wearing a sweater standing casually, representing a relaxed state of mind under pressure.", "Backend server crashing while I stay completely chill.", "chillGuy", "https://res.cloudinary.com/dplzxrvqy/image/upload/v1734749140/meme_templates/ovushouholjp0u08l9qr.jpg" },
                    { 11, "A man pointing to his head, indicating intelligence, smart decisions, or witty workarounds.", "You can't have bugs in your code if you don't write any code.", "thinkingBlackGuy", "https://res.cloudinary.com/dplzxrvqy/image/upload/v1734749140/meme_templates/eus5k9oectd3lbgozgx9.jpg" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_meme_templates_name",
                table: "meme_templates",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_email",
                table: "users",
                column: "email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "meme_templates");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
