const token = localStorage.getItem("token");
const API_BASE_URL = 'https://localhost:7151';

if (!token) {
    alert("Acesso negado. Faça login primeiro.");
    window.location.href = "index.html";
}

document.getElementById('btnSair').addEventListener('click', () => {
    localStorage.removeItem("token");
    window.location.href = "index.html";
});

let modalNotasInstance = null;

document.addEventListener('DOMContentLoaded', () => {
    modalNotasInstance = new bootstrap.Modal(document.getElementById('modalLancarNotas'));

    // 1. AQUI VOCÊ DEVE CARREGAR AS TURMAS DO PROFESSOR (Exemplo estático abaixo)
    // fetch(`${API_BASE_URL}/api/professor/turmas`, ... )
    const selectTurma = document.getElementById('selectTurma');
    selectTurma.innerHTML = `
        <option value="">Selecione...</option>
        <option value="1">1º Ano A</option>
        <option value="2">2º Ano B</option>
    `;

    // 2. AQUI VOCÊ CARREGA AS DISCIPLINAS QUANDO A TURMA MUDA
    selectTurma.addEventListener('change', (e) => {
        const selectDisciplina = document.getElementById('selectDisciplina');
        if (e.target.value) {
            selectDisciplina.disabled = false;
            selectDisciplina.innerHTML = `
                <option value="">Selecione...</option>
                <option value="101">Matemática</option>
                <option value="102">Português</option>
            `;
        } else {
            selectDisciplina.disabled = true;
            selectDisciplina.innerHTML = '<option value="">Selecione a disciplina...</option>';
        }
    });

    // 3. BUSCAR ALUNOS DA TURMA E DISCIPLINA SELECIONADAS
    document.getElementById('btnBuscarAlunos').addEventListener('click', async () => {
        const turmaId = document.getElementById('selectTurma').value;
        const disciplinaId = document.getElementById('selectDisciplina').value;

        if (!turmaId || !disciplinaId) {
            alert("Selecione a turma e a disciplina primeiro.");
            return;
        }

        const tbody = document.getElementById('corpoTabelaAlunos');
        tbody.innerHTML = `<tr><td colspan="7" class="text-center py-4"><div class="spinner-border text-success spinner-border-sm"></div> Carregando...</td></tr>`;

        try {
            // Substitua pela sua rota C# correta para buscar os alunos do diário
            // const response = await fetch(`${API_BASE_URL}/api/diario/turma/${turmaId}/disciplina/${disciplinaId}`, { headers: ... });

            // DADOS FALSOS PARA TESTE VISUAL (Apague quando conectar na API)
            const mockAlunos = [
                { idAluno: 1, matricula: "2024001", nome: "João Silva", notaU1: 7.5, notaU2: null, notaU3: null, frequencia: 85 },
                { idAluno: 2, matricula: "2024002", nome: "Maria Oliveira", notaU1: 9.0, notaU2: 8.5, notaU3: null, frequencia: 95 }
            ];

            renderizarTabelaAlunos(mockAlunos);

        } catch (error) {
            console.error(error);
            tbody.innerHTML = `<tr><td colspan="7" class="text-center text-danger py-4">Erro ao carregar alunos.</td></tr>`;
        }
    });

    // 4. SALVAR NOTAS DO MODAL NO BANCO DE DADOS
    document.getElementById('formLancarNotas').addEventListener('submit', async (e) => {
        e.preventDefault();

        const btnSalvar = document.getElementById('btnSalvarNotas');
        const alertModal = document.getElementById('alertModal');

        const idAluno = document.getElementById('modalIdAluno').value;
        const turmaId = document.getElementById('selectTurma').value;
        const disciplinaId = document.getElementById('selectDisciplina').value;

        // Pega valores. Se vazio, envia nulo para não sobrescrever com zero incorretamente.
        const vU1 = document.getElementById('notaU1').value;
        const vU2 = document.getElementById('notaU2').value;
        const vU3 = document.getElementById('notaU3').value;
        const vFreq = document.getElementById('frequencia').value;

        const payload = {
            idAluno: parseInt(idAluno),
            idTurma: parseInt(turmaId),
            idDisciplina: parseInt(disciplinaId),
            notaU1: vU1 !== "" ? parseFloat(vU1) : null,
            notaU2: vU2 !== "" ? parseFloat(vU2) : null,
            notaU3: vU3 !== "" ? parseFloat(vU3) : null,
            frequencia: vFreq !== "" ? parseInt(vFreq) : null
        };

        try {
            btnSalvar.disabled = true;
            btnSalvar.innerHTML = '<span class="spinner-border spinner-border-sm me-2"></span>Salvando...';

            // CHAME SUA API AQUI (Exemplo de PUT ou POST)
            /*
            const response = await fetch(`${API_BASE_URL}/api/diario/lancar-notas`, {
                method: 'PUT',
                headers: { 'Content-Type': 'application/json', 'Authorization': `Bearer ${token}` },
                body: JSON.stringify(payload)
            });
            */

            // Simulação de sucesso
            await new Promise(r => setTimeout(r, 1000)); // Simula tempo de rede

            mostrarAlerta(alertModal, "Notas salvas com sucesso!", "success");

            setTimeout(() => {
                modalNotasInstance.hide();
                alertModal.classList.add('d-none');
                document.getElementById('btnBuscarAlunos').click(); // Recarrega a tabela
            }, 1500);

        } catch (error) {
            mostrarAlerta(alertModal, "Erro ao salvar as notas.", "danger");
        } finally {
            btnSalvar.disabled = false;
            btnSalvar.innerText = "Salvar Lançamento";
        }
    });
});

// ================= FUNÇÕES AUXILIARES =================

function renderizarTabelaAlunos(alunos) {
    const tbody = document.getElementById('corpoTabelaAlunos');
    tbody.innerHTML = '';

    if (alunos.length === 0) {
        tbody.innerHTML = `<tr><td colspan="7" class="text-center py-4">Nenhum aluno encontrado nesta turma.</td></tr>`;
        return;
    }

    alunos.forEach(aluno => {
        const formatarNota = (n) => n === null || n === undefined ? '<span class="text-muted">-</span>' : Number(n).toFixed(1);
        const formatarFreq = (f) => f === null || f === undefined ? '<span class="text-muted">-</span>' : `${f}%`;

        // Guardamos os dados do aluno no atributo data do botão para preencher o modal depois
        const tr = document.createElement('tr');
        tr.innerHTML = `
            <td>${aluno.matricula}</td>
            <td class="fw-medium">${aluno.nome}</td>
            <td class="text-center">${formatarNota(aluno.notaU1)}</td>
            <td class="text-center">${formatarNota(aluno.notaU2)}</td>
            <td class="text-center">${formatarNota(aluno.notaU3)}</td>
            <td class="text-center">${formatarFreq(aluno.frequencia)}</td>
            <td class="text-center">
                <button class="btn btn-sm btn-outline-primary" onclick="abrirModalNotas(${aluno.idAluno}, '${aluno.nome}', ${aluno.notaU1}, ${aluno.notaU2}, ${aluno.notaU3}, ${aluno.frequencia})">
                    <i class="fas fa-edit"></i> Lançar
                </button>
            </td>
        `;
        tbody.appendChild(tr);
    });
}

// Função chamada pelo botão "Lançar" na tabela
window.abrirModalNotas = function (idAluno, nome, notaU1, notaU2, notaU3, frequencia) {
    // Preenche os campos do modal com os dados atuais do aluno
    document.getElementById('modalIdAluno').value = idAluno;
    document.getElementById('modalNomeAluno').textContent = nome;

    document.getElementById('notaU1').value = notaU1 !== null ? notaU1 : "";
    document.getElementById('notaU2').value = notaU2 !== null ? notaU2 : "";
    document.getElementById('notaU3').value = notaU3 !== null ? notaU3 : "";
    document.getElementById('frequencia').value = frequencia !== null ? frequencia : "";

    document.getElementById('alertModal').classList.add('d-none');

    // Abre o modal
    modalNotasInstance.show();
};

function mostrarAlerta(elemento, mensagem, tipo) {
    elemento.className = `alert alert-${tipo} small p-2 text-center mt-3 mb-0`;
    elemento.textContent = mensagem;
    elemento.classList.remove('d-none');
}