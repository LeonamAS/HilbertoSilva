const token = localStorage.getItem("token");
const API_BASE_URL = 'https://localhost:7151';

let turmasDisciplinasCache = [];

if (!token) {
    alert("Acesso negado. Faça login primeiro.");
    window.location.href = "index.html";
}

document.getElementById('btnSair').addEventListener('click', () => {
    localStorage.removeItem("token");
    window.location.href = API_BASE_URL;
});

let modalNotasInstance = null;

document.addEventListener('DOMContentLoaded', async () => {
    modalNotasInstance = new bootstrap.Modal(document.getElementById('modalLancarNotas'));

    await carregarMinhasTurmas();

    document.getElementById('selectTurma').addEventListener('change', (e) => {
        const turmaId = parseInt(e.target.value);
        const selectDisciplina = document.getElementById('selectDisciplina');

        if (turmaId) {
            selectDisciplina.disabled = false;
            const disciplinasDaTurma = turmasDisciplinasCache.filter(td => td.turmaId === turmaId);

            selectDisciplina.innerHTML = '<option value="">Selecione a disciplina...</option>';
            disciplinasDaTurma.forEach(d => {
                selectDisciplina.innerHTML += `<option value="${d.disciplinaId}">${d.nomeDisciplina}</option>`;
            });
        } else {
            selectDisciplina.disabled = true;
            selectDisciplina.innerHTML = '<option value="">Selecione a disciplina...</option>';
        }
    });

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
            const response = await fetch(`${API_BASE_URL}/api/DiarioClasse/turma/${turmaId}/disciplina/${disciplinaId}`, {
                method: 'GET',
                headers: {
                    'Authorization': `Bearer ${token}`,
                    'Content-Type': 'application/json'
                }
            });

            if (!response.ok) throw new Error("Falha ao buscar alunos.");

            const alunos = await response.json();
            renderizarTabelaAlunos(alunos);

        } catch (error) {
            console.error(error);
            tbody.innerHTML = `<tr><td colspan="7" class="text-center text-danger py-4">Erro ao carregar alunos do servidor.</td></tr>`;
        }
    });

    document.getElementById('formLancarNotas').addEventListener('submit', async (e) => {
        e.preventDefault();

        const btnSalvar = document.getElementById('btnSalvarNotas');
        const alertModal = document.getElementById('alertModal');

        const alunoId = document.getElementById('modalIdAluno').value;
        const turmaId = document.getElementById('selectTurma').value;
        const disciplinaId = document.getElementById('selectDisciplina').value;

        const vU1 = document.getElementById('notaU1').value;
        const vU2 = document.getElementById('notaU2').value;
        const vU3 = document.getElementById('notaU3').value;
        const vFreq = document.getElementById('frequencia').value;

        const payload = {
            alunoId: parseInt(alunoId),
            turmaId: parseInt(turmaId),
            disciplinaId: parseInt(disciplinaId),
            notaU1: vU1 !== "" ? parseFloat(vU1.replace(',', '.')) : null,
            notaU2: vU2 !== "" ? parseFloat(vU2.replace(',', '.')) : null,
            notaU3: vU3 !== "" ? parseFloat(vU3.replace(',', '.')) : null,
            frequencia: vFreq !== "" ? parseFloat(vFreq.replace(',', '.')) : null
        };

        try {
            btnSalvar.disabled = true;
            btnSalvar.innerHTML = '<span class="spinner-border spinner-border-sm me-2"></span>Salvando...';

            const response = await fetch(`${API_BASE_URL}/api/DiarioClasse/lancar-notas`, {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token}`
                },
                body: JSON.stringify(payload)
            });

            if (!response.ok) throw new Error("Erro da API ao salvar.");

            mostrarAlerta(alertModal, "Notas salvas com sucesso!", "success");

            setTimeout(() => {
                modalNotasInstance.hide();
                alertModal.classList.add('d-none');
                document.getElementById('btnBuscarAlunos').click();
            }, 1500);

        } catch (error) {
            console.error(error);
            mostrarAlerta(alertModal, "Erro ao salvar as notas. Verifique os dados e tente novamente.", "danger");
        } finally {
            btnSalvar.disabled = false;
            btnSalvar.innerText = "Salvar Lançamento";
        }
    });
});

async function carregarMinhasTurmas() {
    try {
        const response = await fetch(`${API_BASE_URL}/api/DiarioClasse/minhas-turmas`, {
            method: 'GET',
            headers: { 'Authorization': `Bearer ${token}` }
        });

        if (!response.ok) throw new Error("Erro ao buscar turmas.");

        turmasDisciplinasCache = await response.json();

        const mapTurmasUnicas = new Map();
        const turmasUnicas = [];

        for (const item of turmasDisciplinasCache) {
            if (!mapTurmasUnicas.has(item.turmaId)) {
                mapTurmasUnicas.set(item.turmaId, true);
                turmasUnicas.push({ id: item.turmaId, nome: item.nomeTurma });
            }
        }

        const selectTurma = document.getElementById('selectTurma');
        selectTurma.innerHTML = '<option value="">Selecione a turma...</option>';

        turmasUnicas.forEach(t => {
            selectTurma.innerHTML += `<option value="${t.id}">${t.nome}</option>`;
        });

    } catch (error) {
        console.error(error);
        alert("Erro de comunicação com o servidor ao carregar suas turmas.");
    }
}

function renderizarTabelaAlunos(alunos) {
    const tbody = document.getElementById('corpoTabelaAlunos');
    tbody.innerHTML = '';

    if (alunos.length === 0) {
        tbody.innerHTML = `<tr><td colspan="7" class="text-center py-4 text-muted">Nenhum aluno encontrado nesta turma.</td></tr>`;
        return;
    }

    alunos.forEach(aluno => {
        const corU1 = (aluno.notaU1 !== null && Number(aluno.notaU1) < 6) ? 'text-danger fw-bold' : '';
        const corU2 = (aluno.notaU2 !== null && Number(aluno.notaU2) < 6) ? 'text-danger fw-bold' : '';
        const corU3 = (aluno.notaU3 !== null && Number(aluno.notaU3) < 6) ? 'text-danger fw-bold' : '';
        const corFreq = (aluno.frequencia !== null && Number(aluno.frequencia) < 70) ? 'text-danger fw-bold' : '';

        const formatarNota = (n, cor) => n === null || n === undefined ? '<span class="text-muted">-</span>' : `<span class="${cor}">${Number(n).toFixed(1)}</span>`;
        const formatarFreq = (f, cor) => f === null || f === undefined ? '<span class="text-muted">-</span>' : `<span class="${cor}">${Number(f).toFixed(0)}%</span>`;

        const tr = document.createElement('tr');
        tr.innerHTML = `
            <td data-label="Matrícula">${aluno.matricula}</td>
            <td data-label="Nome do Aluno" class="fw-medium">${aluno.nomeAluno}</td>
            <td data-label="Unidade 1" class="text-center">${formatarNota(aluno.notaU1, corU1)}</td>
            <td data-label="Unidade 2" class="text-center">${formatarNota(aluno.notaU2, corU2)}</td>
            <td data-label="Unidade 3" class="text-center">${formatarNota(aluno.notaU3, corU3)}</td>
            <td data-label="Frequência (%)" class="text-center">${formatarFreq(aluno.frequencia, corFreq)}</td>
            <td data-label="Ações" class="text-center">
                <button class="btn btn-sm btn-outline-primary" onclick="abrirModalNotas(${aluno.alunoId}, '${aluno.nomeAluno}', ${aluno.notaU1}, ${aluno.notaU2}, ${aluno.notaU3}, ${aluno.frequencia})">
                    <i class="fas fa-edit"></i> Lançar
                </button>
            </td>
        `;
        tbody.appendChild(tr);
    });
}

window.abrirModalNotas = function (alunoId, nomeAluno, notaU1, notaU2, notaU3, frequencia) {
    document.getElementById('modalIdAluno').value = alunoId;
    document.getElementById('modalNomeAluno').textContent = nomeAluno;

    document.getElementById('notaU1').value = notaU1 !== null ? notaU1 : "";
    document.getElementById('notaU2').value = notaU2 !== null ? notaU2 : "";
    document.getElementById('notaU3').value = notaU3 !== null ? notaU3 : "";
    document.getElementById('frequencia').value = frequencia !== null ? frequencia : "";

    document.getElementById('alertModal').classList.add('d-none');
    modalNotasInstance.show();
};

function mostrarAlerta(elemento, mensagem, tipo) {
    elemento.className = `alert alert-${tipo} small p-2 text-center mt-3 mb-0`;
    elemento.textContent = mensagem;
    elemento.classList.remove('d-none');
}

document.querySelectorAll('.toggle-password').forEach(button => {
    button.addEventListener('click', function () {
        const targetId = this.getAttribute('data-target');
        const input = document.getElementById(targetId);
        const icon = this.querySelector('i');

        if (input.type === 'password') {
            input.type = 'text';
            icon.classList.remove('fa-eye');
            icon.classList.add('fa-eye-slash');
        } else {
            input.type = 'password';
            icon.classList.remove('fa-eye-slash');
            icon.classList.add('fa-eye');
        }
    });
});

function validarSenhaForte(senha) {
    const regex = /^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*#?&])[A-Za-z\d@$!%*#?&]{8,}$/;
    return regex.test(senha);
}

function limparErrosSenha() {
    document.querySelectorAll('#formAlterarSenha .text-danger.small').forEach(el => {
        el.classList.add('d-none');
        el.textContent = '';
    });
    document.querySelectorAll('#formAlterarSenha .is-invalid').forEach(el => {
        el.classList.remove('is-invalid');
    });
}

function mostrarErroSenha(inputId, erroId, mensagem) {
    const input = document.getElementById(inputId);
    const erroEl = document.getElementById(erroId);

    if (input) input.classList.add('is-invalid');
    if (erroEl) {
        erroEl.textContent = mensagem;
        erroEl.classList.remove('d-none');
    }
}

document.getElementById('formAlterarSenha').addEventListener('submit', async function (e) {
    e.preventDefault();
    limparErrosSenha();

    const senhaAtual = document.getElementById('senhaAtual').value;
    const novaSenha = document.getElementById('novaSenha').value;
    const confirmarSenha = document.getElementById('confirmarSenha').value;

    let formValido = true;

    if (!senhaAtual) {
        mostrarErroSenha('senhaAtual', 'erroSenhaAtual', 'A senha atual é obrigatória.');
        formValido = false;
    }

    if (!validarSenhaForte(novaSenha)) {
        mostrarErroSenha('novaSenha', 'erroNovaSenha', 'A nova senha não atende aos requisitos mínimos.');
        formValido = false;
    }

    if (novaSenha !== confirmarSenha) {
        mostrarErroSenha('confirmarSenha', 'erroConfirmarSenha', 'As senhas digitadas não coincidem.');
        formValido = false;
    }

    if (!formValido) return;

    console.log("Formulário validado com sucesso! Pronto para chamar a API.");
});