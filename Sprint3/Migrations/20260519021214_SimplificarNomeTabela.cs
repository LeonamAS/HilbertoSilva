using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HilbertoSilva.Migrations
{
    /// <inheritdoc />
    public partial class SimplificarNomeTabela : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Boletins_TurmasDisciplinas_fk_turma_disciplina",
                table: "Boletins");

            migrationBuilder.DropTable(
                name: "TurmasDisciplinas");

            migrationBuilder.CreateTable(
                name: "DiarioClasse",
                columns: table => new
                {
                    turma_disciplina_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    fk_turma = table.Column<int>(type: "int", nullable: false),
                    fk_disciplina = table.Column<int>(type: "int", nullable: false),
                    fk_professor = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiarioClasse", x => x.turma_disciplina_id);
                    table.ForeignKey(
                        name: "FK_DiarioClasse_Disciplinas_fk_disciplina",
                        column: x => x.fk_disciplina,
                        principalTable: "Disciplinas",
                        principalColumn: "disciplina_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DiarioClasse_Professores_fk_professor",
                        column: x => x.fk_professor,
                        principalTable: "Professores",
                        principalColumn: "professor_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DiarioClasse_Turmas_fk_turma",
                        column: x => x.fk_turma,
                        principalTable: "Turmas",
                        principalColumn: "turma_id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_DiarioClasse_fk_disciplina",
                table: "DiarioClasse",
                column: "fk_disciplina");

            migrationBuilder.CreateIndex(
                name: "IX_DiarioClasse_fk_professor",
                table: "DiarioClasse",
                column: "fk_professor");

            migrationBuilder.CreateIndex(
                name: "IX_DiarioClasse_fk_turma",
                table: "DiarioClasse",
                column: "fk_turma");

            migrationBuilder.AddForeignKey(
                name: "FK_Boletins_DiarioClasse_fk_turma_disciplina",
                table: "Boletins",
                column: "fk_turma_disciplina",
                principalTable: "DiarioClasse",
                principalColumn: "turma_disciplina_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Boletins_DiarioClasse_fk_turma_disciplina",
                table: "Boletins");

            migrationBuilder.DropTable(
                name: "DiarioClasse");

            migrationBuilder.CreateTable(
                name: "TurmasDisciplinas",
                columns: table => new
                {
                    turma_disciplina_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    fk_disciplina = table.Column<int>(type: "int", nullable: false),
                    fk_professor = table.Column<int>(type: "int", nullable: false),
                    fk_turma = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TurmasDisciplinas", x => x.turma_disciplina_id);
                    table.ForeignKey(
                        name: "FK_TurmasDisciplinas_Disciplinas_fk_disciplina",
                        column: x => x.fk_disciplina,
                        principalTable: "Disciplinas",
                        principalColumn: "disciplina_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TurmasDisciplinas_Professores_fk_professor",
                        column: x => x.fk_professor,
                        principalTable: "Professores",
                        principalColumn: "professor_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TurmasDisciplinas_Turmas_fk_turma",
                        column: x => x.fk_turma,
                        principalTable: "Turmas",
                        principalColumn: "turma_id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_TurmasDisciplinas_fk_disciplina",
                table: "TurmasDisciplinas",
                column: "fk_disciplina");

            migrationBuilder.CreateIndex(
                name: "IX_TurmasDisciplinas_fk_professor",
                table: "TurmasDisciplinas",
                column: "fk_professor");

            migrationBuilder.CreateIndex(
                name: "IX_TurmasDisciplinas_fk_turma",
                table: "TurmasDisciplinas",
                column: "fk_turma");

            migrationBuilder.AddForeignKey(
                name: "FK_Boletins_TurmasDisciplinas_fk_turma_disciplina",
                table: "Boletins",
                column: "fk_turma_disciplina",
                principalTable: "TurmasDisciplinas",
                principalColumn: "turma_disciplina_id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
