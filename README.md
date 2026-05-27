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

# Documentação Funcional

## Objetivo do Sistema

- Possibilitar à secretaria fazer todo o gerenciamento escolar, manipulando com facilidade as entidades escolares e ao mesmo tempo tornando flexível as relações entre as mesmas.
- Facilitar para os professores atribuírem notas e frequências para seus alunos.
- Permitir  aos alunos a visualização de seus boletins de forma fácil e simplificada.

## Regras de Negócio

- RF001 - O sistema utiliza o CPF e uma senha para autenticação. Quando criamos um novo aluno/professor é atribuído o seu respectivo Role, que os permite acessar a sua  tela dedicada no sistema.
- RF002 - Um aluno só pode estar matriculado em uma única turma, que é o comportamento natural em escolas. O sistema exige que o cadastro do aluno contenha obrigatoriamente os dados do seu responsável legal, destacando  a importância do acompanhamento escolar e de estar preparado para qualquer acidente/incidente.
- RF003 - O aluno só pode visualizar o seu próprio boletim, e o professor só pode atribuir as  notas/frequência para as  turmas e matérias que leciona.
- RF004 - Fluxo adequado de inserção de dados: Criar disciplinas, criar turma, criar  diário de classe, criar professor, criar aluno.

## Funcionalidades Principais

- Criação de diários de classe detalhados e flexíveis.
- Simplificação no processo de montar o boletim escolar do aluno.
- Agilidade em gerenciar todos os dados presentes no sistema.

