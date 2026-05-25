1. Descrição do Projeto
        	Portal de Gestão Escolar para uma escola do Ensino Fundamental II, onde pode-se fazer todo o controle de professores, alunos, classes e disciplinas. Há também uma parte destinada ao professor que possibilita a inserção de notas e frequências para seus alunos. Possui também um dashboard para o aluno visualizar o seu boletim.
 
2. Tecnologias Utilizadas
● 	Linguagem: C#, JavaScript, HTML, CSS.
● 	Framework: ASP.NET
● 	Banco de Dados: MySQL
● 	Segurança: JWT
● 	Documentação de API: Swagger UI
 
3. Instruções de Execução
Para rodar o projeto localmente, siga os passos abaixo:

Abra o appsettings.json e configure a ConnectionString para acessar o banco de dados local. 
Deve ficar assim:
“Server=localhost;Database=em_hilberto_silva;Uid=root;Pwd={SUA_SENHA_AQUI};”.
Abra o Console do Gerenciador de Pacotes e execute o Update-Database.

Rode a aplicação.

Obs.: Se desejar visualizar como ficará a aplicação populada com os dados, copie o texto do arquivo Dados de Teste.txt e rode no MySql Workbench 8. Os alunos que possuem o boletim preenchido são os que a matricula termina com XX1.
	O seu administrador será CPF/Usuário = Admin, e Senha = 123456.
 
4. Endpoints da API
Abaixo estão os principais endpoints disponíveis no sistema:
 
Login:
POST: /api/Auth/login - Realiza o login de um usuário e retorna um Token JWT.

Criar usuários (Administrador):
POST: /api/Alunos/com-usuario - Cria um novo aluno vinculado a um usuário de acesso.
POST: /api/Professor/com-usuario - Cria um novo professor vinculado a um usuário de acesso.

Funcionalidade para o Aluno:
GET: /api/Boletim/meu-boletim - Obtém os boletins do aluno autenticado com base no seu CPF.

Funcionalidades para o Professor:
GET: /api/DiarioClasse/minhas-turmas - Obtém as turmas e disciplinas vinculadas ao professor autenticado.
GET: /api/DiarioClasse/turma/{turmaId}/disciplina/{disciplinaId} - Obtém os alunos e suas respectivas notas filtrados por Turma e Disciplina para o Diário.
PUT: /api/DiarioClasse/lancar-notas - Lança ou atualiza as notas e a frequência de um aluno específico no diário.
