# Desafio Sprint Final - Implantação e Documentação de Sistemas

## 1. Descrição do Projeto:
Sistema **Portal de Gestão Escolar** desenvolvido para uma escola do Ensino Fundamental II. A aplicação permite o controle completo da instituição, gerenciando de forma eficiente **professores, disciplinas, alunos e seus boletins, classes e seus diários**.

O sistema conta com painéis específicos para diferentes usuários:
* **Área do Professor:** Possibilita o gerenciamento do diário de classe, com inserção de notas e controle de frequências dos alunos.
* **Área do Aluno:** Oferece um dashboard interativo para que o aluno possa visualizar e imprimir o seu boletim escolar de forma rápida e prática.

---

## 2. Tecnologias Utilizadas:
* **Linguagem:** C#, JavaScript, HTML, CSS.
* **Framework:** ASP.NET Core Web API com Entity Framework Core
* **Banco de Dados:** MySQL
* **Segurança:** JWT para Autenticação
* **Documentação de API:** Swagger UI

---

## 3. Instruções de Execução:
Para rodar o projeto localmente, siga os passos abaixo:

**1.** Abra o appsettings.json e configure a string de conexão no `appsettings.json` para acessar o banco de dados local. 
Deve ficar assim:
“Server=localhost;Database=em_hilberto_silva;Uid=root;Pwd={SUA_SENHA_AQUI};”.
**2.** Abra o Console do Gerenciador de Pacotes e execute o `Update-Database`.

**3.** Execute o projeto (F5).

**Obs.:** Se desejar visualizar como ficará a aplicação populada com os dados, copie o texto do arquivo Dados de Teste.txt e rode no MySql Workbench 8. Os alunos que possuem o boletim preenchido são os que a matricula termina com XX1.
* **O seu administrador será CPF/Usuário = Admin, e Senha = 123456.**

---

## 4. Endpoints da API
Abaixo estão os principais endpoints disponíveis no sistema:
 
**Login**:
- POST: /api/Auth/login - Realiza o login de um usuário e retorna um Token JWT.

**Criar usuários (Administrador):**
- POST: /api/Alunos/com-usuario - Cria um novo aluno vinculado a um usuário de acesso.
- POST: /api/Professor/com-usuario - Cria um novo professor vinculado a um usuário de acesso.

**Funcionalidade para o Aluno:**
- GET: /api/Boletim/meu-boletim - Obtém os boletins do aluno autenticado com base no seu CPF.

**Funcionalidades para o Professor:**
- GET: /api/DiarioClasse/minhas-turmas - Obtém as turmas e disciplinas vinculadas ao professor autenticado.
- GET: /api/DiarioClasse/turma/{turmaId}/disciplina/{disciplinaId} - Obtém os alunos e suas respectivas notas filtrados por Turma e Disciplina para o Diário.
- PUT: /api/DiarioClasse/lancar-notas - Lança ou atualiza as notas e a frequência de um aluno específico no diário.
