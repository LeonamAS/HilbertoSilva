using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HilbertoSilva.Migrations
{
    /// <inheritdoc />
    public partial class Inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Disciplinas",
                columns: table => new
                {
                    disciplina_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    nome = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    carga_horaria = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Disciplinas", x => x.disciplina_id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Turmas",
                columns: table => new
                {
                    turma_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    nome_turma = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ano_escolar = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ano_letivo = table.Column<int>(type: "int", nullable: false),
                    turno = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Turmas", x => x.turma_id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    usuario_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    cpf = table.Column<string>(type: "varchar(11)", maxLength: 11, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    senha = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    tipo_usuario = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    data_cadastro = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.usuario_id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Alunos",
                columns: table => new
                {
                    aluno_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    fk_usuario = table.Column<int>(type: "int", nullable: false),
                    fk_turma = table.Column<int>(type: "int", nullable: true),
                    nome = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    data_nascimento = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    matricula = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    nome_responsavel = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    cpf_responsavel = table.Column<string>(type: "varchar(11)", maxLength: 11, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    telefone_responsavel = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alunos", x => x.aluno_id);
                    table.ForeignKey(
                        name: "FK_Alunos_Turmas_fk_turma",
                        column: x => x.fk_turma,
                        principalTable: "Turmas",
                        principalColumn: "turma_id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Alunos_Usuarios_fk_usuario",
                        column: x => x.fk_usuario,
                        principalTable: "Usuarios",
                        principalColumn: "usuario_id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Professores",
                columns: table => new
                {
                    professor_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    fk_usuario = table.Column<int>(type: "int", nullable: false),
                    nome = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    telefone = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    especialidade = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Professores", x => x.professor_id);
                    table.ForeignKey(
                        name: "FK_Professores_Usuarios_fk_usuario",
                        column: x => x.fk_usuario,
                        principalTable: "Usuarios",
                        principalColumn: "usuario_id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TurmasDisciplinas",
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

            migrationBuilder.CreateTable(
                name: "Boletins",
                columns: table => new
                {
                    boletim_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    fk_aluno = table.Column<int>(type: "int", nullable: false),
                    fk_turma_disciplina = table.Column<int>(type: "int", nullable: false),
                    nota_u1 = table.Column<decimal>(type: "decimal(4,2)", precision: 4, scale: 2, nullable: false),
                    nota_u2 = table.Column<decimal>(type: "decimal(4,2)", precision: 4, scale: 2, nullable: false),
                    nota_u3 = table.Column<decimal>(type: "decimal(4,2)", precision: 4, scale: 2, nullable: false),
                    frequencia = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Boletins", x => x.boletim_id);
                    table.ForeignKey(
                        name: "FK_Boletins_Alunos_fk_aluno",
                        column: x => x.fk_aluno,
                        principalTable: "Alunos",
                        principalColumn: "aluno_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Boletins_TurmasDisciplinas_fk_turma_disciplina",
                        column: x => x.fk_turma_disciplina,
                        principalTable: "TurmasDisciplinas",
                        principalColumn: "turma_disciplina_id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Alunos_fk_turma",
                table: "Alunos",
                column: "fk_turma");

            migrationBuilder.CreateIndex(
                name: "IX_Alunos_fk_usuario",
                table: "Alunos",
                column: "fk_usuario",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Alunos_matricula",
                table: "Alunos",
                column: "matricula",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "aluno_materia_unico",
                table: "Boletins",
                columns: new[] { "fk_aluno", "fk_turma_disciplina" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Boletins_fk_turma_disciplina",
                table: "Boletins",
                column: "fk_turma_disciplina");

            migrationBuilder.CreateIndex(
                name: "IX_Professores_fk_usuario",
                table: "Professores",
                column: "fk_usuario",
                unique: true);

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

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_cpf",
                table: "Usuarios",
                column: "cpf",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Boletins");

            migrationBuilder.DropTable(
                name: "Alunos");

            migrationBuilder.DropTable(
                name: "TurmasDisciplinas");

            migrationBuilder.DropTable(
                name: "Disciplinas");

            migrationBuilder.DropTable(
                name: "Professores");

            migrationBuilder.DropTable(
                name: "Turmas");

            migrationBuilder.DropTable(
                name: "Usuarios");
        }
    }
}
