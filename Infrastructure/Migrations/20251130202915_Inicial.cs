using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CategoriaPersonalidades",
                columns: table => new
                {
                    Id = table.Column<byte>(type: "tinyint", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoriaPersonalidades", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CategoriasEdad",
                columns: table => new
                {
                    Id = table.Column<byte>(type: "tinyint", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoriasEdad", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CategoriasGenero",
                columns: table => new
                {
                    Id = table.Column<byte>(type: "tinyint", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoriasGenero", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CategoriasTipoAnimal",
                columns: table => new
                {
                    Id = table.Column<byte>(type: "tinyint", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoriasTipoAnimal", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Imagenes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Imagenes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tamanio",
                columns: table => new
                {
                    Id = table.Column<byte>(type: "tinyint", nullable: false),
                    TamanioMascota = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tamanio", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Correo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Telefono = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Rol = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Mascotas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EdadMeses = table.Column<byte>(type: "tinyint", nullable: false),
                    ImagenUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DescripcionCorta = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DescripcionLarga = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CategoriaEdadId = table.Column<byte>(type: "tinyint", nullable: false),
                    TamanioId = table.Column<byte>(type: "tinyint", nullable: false),
                    TipoAnimalId = table.Column<byte>(type: "tinyint", nullable: false),
                    CategoriaGeneroId = table.Column<byte>(type: "tinyint", nullable: false),
                    Disponible = table.Column<bool>(type: "bit", nullable: false),
                    Aprobado = table.Column<bool>(type: "bit", nullable: false),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    Ciudad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CategoriaTipoAnimalId = table.Column<byte>(type: "tinyint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mascotas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Mascotas_CategoriasEdad_CategoriaEdadId",
                        column: x => x.CategoriaEdadId,
                        principalTable: "CategoriasEdad",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Mascotas_CategoriasGenero_CategoriaGeneroId",
                        column: x => x.CategoriaGeneroId,
                        principalTable: "CategoriasGenero",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Mascotas_CategoriasTipoAnimal_CategoriaTipoAnimalId",
                        column: x => x.CategoriaTipoAnimalId,
                        principalTable: "CategoriasTipoAnimal",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Mascotas_CategoriasTipoAnimal_TipoAnimalId",
                        column: x => x.TipoAnimalId,
                        principalTable: "CategoriasTipoAnimal",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Mascotas_Tamanio_TamanioId",
                        column: x => x.TamanioId,
                        principalTable: "Tamanio",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Mascotas_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MascotaPersonalidades",
                columns: table => new
                {
                    MascotaId = table.Column<int>(type: "int", nullable: false),
                    CategoriaPersonalidadId = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MascotaPersonalidades", x => new { x.MascotaId, x.CategoriaPersonalidadId });
                    table.ForeignKey(
                        name: "FK_MascotaPersonalidades_CategoriaPersonalidades_CategoriaPersonalidadId",
                        column: x => x.CategoriaPersonalidadId,
                        principalTable: "CategoriaPersonalidades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MascotaPersonalidades_Mascotas_MascotaId",
                        column: x => x.MascotaId,
                        principalTable: "Mascotas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "CategoriaPersonalidades",
                columns: new[] { "Id", "Nombre" },
                values: new object[,]
                {
                    { (byte)1, "Juguetón" },
                    { (byte)2, "Tranquilo" },
                    { (byte)3, "Protector" }
                });

            migrationBuilder.InsertData(
                table: "CategoriasEdad",
                columns: new[] { "Id", "Nombre" },
                values: new object[,]
                {
                    { (byte)1, "Cachorro" },
                    { (byte)2, "Joven" },
                    { (byte)3, "Adulto" }
                });

            migrationBuilder.InsertData(
                table: "CategoriasGenero",
                columns: new[] { "Id", "Nombre" },
                values: new object[,]
                {
                    { (byte)1, "Hembra" },
                    { (byte)2, "Macho" }
                });

            migrationBuilder.InsertData(
                table: "CategoriasTipoAnimal",
                columns: new[] { "Id", "Nombre" },
                values: new object[,]
                {
                    { (byte)1, "Perro" },
                    { (byte)2, "Gato" },
                    { (byte)3, "Conejo" },
                    { (byte)4, "Ave" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_MascotaPersonalidades_CategoriaPersonalidadId",
                table: "MascotaPersonalidades",
                column: "CategoriaPersonalidadId");

            migrationBuilder.CreateIndex(
                name: "IX_Mascotas_CategoriaEdadId",
                table: "Mascotas",
                column: "CategoriaEdadId");

            migrationBuilder.CreateIndex(
                name: "IX_Mascotas_CategoriaGeneroId",
                table: "Mascotas",
                column: "CategoriaGeneroId");

            migrationBuilder.CreateIndex(
                name: "IX_Mascotas_CategoriaTipoAnimalId",
                table: "Mascotas",
                column: "CategoriaTipoAnimalId");

            migrationBuilder.CreateIndex(
                name: "IX_Mascotas_TamanioId",
                table: "Mascotas",
                column: "TamanioId");

            migrationBuilder.CreateIndex(
                name: "IX_Mascotas_TipoAnimalId",
                table: "Mascotas",
                column: "TipoAnimalId");

            migrationBuilder.CreateIndex(
                name: "IX_Mascotas_UsuarioId",
                table: "Mascotas",
                column: "UsuarioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Imagenes");

            migrationBuilder.DropTable(
                name: "MascotaPersonalidades");

            migrationBuilder.DropTable(
                name: "CategoriaPersonalidades");

            migrationBuilder.DropTable(
                name: "Mascotas");

            migrationBuilder.DropTable(
                name: "CategoriasEdad");

            migrationBuilder.DropTable(
                name: "CategoriasGenero");

            migrationBuilder.DropTable(
                name: "CategoriasTipoAnimal");

            migrationBuilder.DropTable(
                name: "Tamanio");

            migrationBuilder.DropTable(
                name: "Usuarios");
        }
    }
}
