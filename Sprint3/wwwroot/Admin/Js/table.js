import { state } from './state.js';
import { config } from './config.js';
import { apiFetch } from './api.js';
import { exibirAlerta } from './utils.js';
import { buildForm, carregarDropdownsDinamicos } from './form.js';

export async function loadData() {
    const conf = config[state.currentEntity];

    const oldBackBtnAluno = document.getElementById('btnVoltarAlunos');
    if (oldBackBtnAluno) oldBackBtnAluno.remove();
    const oldBackBtnTurma = document.getElementById('btnVoltarTurmas');
    if (oldBackBtnTurma) oldBackBtnTurma.remove();

    if (state.currentEntity === 'boletim' && state.activeAlunoFilter) {
        state.el.pageTitle.innerHTML = `
                <button class="btn btn-sm btn-outline-secondary me-3" id="btnVoltarAlunos">
                    <i class="fas fa-arrow-left"></i> Voltar
                </button> 
                Boletim: <span class="text-muted ms-1">${state.activeAlunoNome}</span>
                <button class="btn btn-sm btn-outline-primary ms-3" id="btnPrintBoletim">
                    <i class="fas fa-print"></i> Imprimir
                </button>
            `;

        document.getElementById('btnVoltarAlunos').addEventListener('click', () => {
            state.currentEntity = 'alunos';
            state.activeAlunoFilter = null;
            state.activeAlunoNome = '';
            state.activeAlunoTurmaId = null;
            loadData();
        });

        document.getElementById('btnPrintBoletim').addEventListener('click', imprimirBoletim);
    }
    else if (state.currentEntity === 'diarioclasse' && state.activeTurmaFilter) {
        state.el.pageTitle.innerHTML = `<button class="btn btn-sm btn-outline-secondary me-3" id="btnVoltarTurmas"><i class="fas fa-arrow-left"></i> Voltar</button> Diário de Classe: <span class="text-muted ms-1">${state.activeTurmaNome}</span>`;

        document.getElementById('btnVoltarTurmas').addEventListener('click', () => {
            state.currentEntity = 'turma';
            state.activeTurmaFilter = null;
            state.activeTurmaNome = '';
            loadData();
        });
    }
    else {
        state.el.pageTitle.textContent = `Gerenciar ${conf.title}`;
    }

    state.el.tableHead.innerHTML = `<tr>${conf.columns.map(c => `<th>${c.label}</th>`).join('')}<th class="text-end">Ações</th></tr>`;
    state.el.tableBody.innerHTML = '<tr><td colspan="100%" class="text-center">Carregando...</td></tr>';

    try {
        let data = await apiFetch(conf.endpoint);

        if (state.currentEntity === 'boletim' && state.activeAlunoFilter) {
            data = data.filter(b => b.alunoId == state.activeAlunoFilter);
        } else if (state.currentEntity === 'diarioclasse' && state.activeTurmaFilter) {
            data = data.filter(d => d.turmaId == state.activeTurmaFilter);
        }

        state.currentTableData = data;

        if (!data?.length) {
            state.el.tableBody.innerHTML = '<tr><td colspan="100%" class="text-center text-muted">Nenhum registro encontrado.</td></tr>';
            return;
        }

        state.el.tableBody.innerHTML = data.map(item => `
            <tr>
                ${conf.columns.map(c => `<td data-label="${c.label}">${c.format ? c.format(item[c.key], item) : (item[c.key] ?? '-')}</td>`).join('')}
                <td class="text-end">
                    <button class="btn btn-sm btn-outline-primary me-2 btn-edit" data-id="${item.id}"><i class="fas fa-edit"></i></button>
                    <button class="btn btn-sm btn-outline-danger btn-delete" data-id="${item.id}"><i class="fas fa-trash"></i></button>
                </td>
            </tr>`).join('');
    } catch (error) {
        exibirAlerta(`Erro: ${error.message}`, 'danger');
        state.el.tableBody.innerHTML = '<tr><td colspan="100%" class="text-center text-danger">Falha ao carregar dados.</td></tr>';
    }
}

export function setupTableEvents() {
    state.el.tableBody.addEventListener('click', async (e) => {

        const btnBoletim = e.target.closest('.lnk-ver-boletim');
        if (btnBoletim) {
            e.preventDefault();
            state.currentEntity = 'boletim';
            state.activeAlunoFilter = btnBoletim.getAttribute('data-id');
            state.activeAlunoNome = btnBoletim.getAttribute('data-nome');
            state.activeAlunoTurmaId = btnBoletim.getAttribute('data-turma');
            loadData();
            return;
        }

        const btnDiario = e.target.closest('.lnk-ver-diario');
        if (btnDiario) {
            e.preventDefault();
            state.currentEntity = 'diarioclasse';
            state.activeTurmaFilter = btnDiario.getAttribute('data-id');
            state.activeTurmaNome = btnDiario.getAttribute('data-nome');
            loadData();
            return;
        }

        const btnEdit = e.target.closest('.btn-edit');
        if (btnEdit) {
            const id = btnEdit.getAttribute('data-id');
            const item = state.currentTableData.find(x => x.id == id);
            if (!item) return;

            const erroAntigo = document.getElementById('formErrorFeedback');
            if (erroAntigo) erroAntigo.classList.add('d-none');

            state.el.modalTitle.textContent = `Editar ${config[state.currentEntity].title}`;
            state.el.idInput.value = item.id;

            await carregarDropdownsDinamicos();
            buildForm(item.id);

            for (const key in item) {
                const field = state.el.form.elements[key];
                if (field) {
                    if (field.type === 'date' && item[key]) {
                        field.value = item[key].split('T')[0];
                    } else {
                        field.value = item[key];
                    }
                }
            }
            state.el.modal.show();
            return;
        }

        const btnDelete = e.target.closest('.btn-delete');
        if (btnDelete) {
            state.itemToDeleteId = btnDelete.getAttribute('data-id');
            state.el.deleteModal.show();
            return;
        }
    });

    document.getElementById('btnConfirmDelete').addEventListener('click', async () => {
        if (!state.itemToDeleteId) return;
        try {
            await apiFetch(`${config[state.currentEntity].endpoint}/${state.itemToDeleteId}`, { method: 'DELETE' });
            state.el.deleteModal.hide();
            exibirAlerta('Registro excluído com sucesso!', 'success');
            loadData();
        } catch (error) {
            state.el.deleteModal.hide();
            exibirAlerta(`Falha ao excluir: ${error.message}`, 'danger');
        } finally {
            state.itemToDeleteId = null;
        }
    });
}

function imprimirBoletim() {
    if (!currentTableData || currentTableData.length === 0) {
        exibirAlerta('Não há dados para imprimir neste boletim.', 'warning');
        return;
    }

    const info = currentTableData[0];
    const dataAtual = new Date().toLocaleDateString('pt-BR');

    const linhasTabela = currentTableData.map(b => {
        const corU1 = Number(b.notaU1) < 6 ? 'text-danger' : '';
        const corU2 = Number(b.notaU2) < 6 ? 'text-danger' : '';
        const corU3 = Number(b.notaU3) < 6 ? 'text-danger' : '';
        const corMedia = Number(b.mediaFinal) < 6 ? 'text-danger' : '';
        const corFreq = Number(b.frequencia) < 70 ? 'text-danger fw-bold' : '';

        return `
            <tr>
                <td>${b.nomeDisciplina}</td>
                <td class="text-center ${corU1}">${Number(b.notaU1).toFixed(1)}</td>
                <td class="text-center ${corU2}">${Number(b.notaU2).toFixed(1)}</td>
                <td class="text-center ${corU3}">${Number(b.notaU3).toFixed(1)}</td>
                <td class="text-center fw-bold ${corMedia}">${Number(b.mediaFinal).toFixed(1)}</td>
                <td class="text-center ${corFreq}">${Number(b.frequencia).toFixed(0)}%</td>
            </tr>
            `;
    }).join('');

    const htmlImpressao = `
            <!DOCTYPE html>
            <html lang="pt-BR">
            <head>
                <meta charset="UTF-8">
                <title>Boletim - ${info.nomeAluno}</title>
                <style>
                    body { font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; padding: 20px; color: #333; }
                    .header { text-align: center; border-bottom: 2px solid #54a790; padding-bottom: 15px; margin-bottom: 25px; }
                    .header h1 { margin: 0; color: #2E4D68; font-size: 24px; text-transform: uppercase; }
                    .header p { margin: 5px 0 0 0; color: #666; font-size: 14px; }
                    .aluno-info { margin-bottom: 30px; padding: 15px; background-color: #f8f9fa; border: 1px solid #ddd; border-radius: 8px; }
                    .aluno-info p { margin: 5px 0; font-size: 14px; }
                    table { width: 100%; border-collapse: collapse; margin-bottom: 30px; }
                    th { background-color: #54a790; color: white; padding: 10px; text-align: left; }
                    td { border: 1px solid #ddd; padding: 10px; }
                    th.text-center, td.text-center { text-align: center; }
                    .fw-bold { font-weight: bold; }
                    .text-danger { color: #dc3545 !important; }
                    .footer { text-align: center; margin-top: 50px; font-size: 12px; color: #999; border-top: 1px solid #eee; padding-top: 10px; }
                    @media print {
                        body { padding: 0; }
                        .aluno-info { background-color: transparent; }
                        th { background-color: #54a790 !important; color: white !important; -webkit-print-color-adjust: exact; }
                    }
                </style>
            </head>
            <body>
                <div class="header">
                    <h1>Escola Municipal Hilberto Silva</h1>
                    <h3>Boletim Escolar</h3>
                    <p>Documento emitido em ${dataAtual}</p>
                </div>
                
                <div class="aluno-info">
                    <p><strong>Aluno(a):</strong> ${info.nomeAluno}</p>
                    <p><strong>Matrícula:</strong> ${info.matricula || 'N/A'}</p>
                    <p><strong>Turma:</strong> ${info.nomeTurma || 'N/A'}</p>
                </div>

                <table>
                    <thead>
                        <tr>
                            <th>Disciplina</th>
                            <th class="text-center">Nota U1</th>
                            <th class="text-center">Nota U2</th>
                            <th class="text-center">Nota U3</th>
                            <th class="text-center">Média Final</th>
                            <th class="text-center">Frequência</th>
                        </tr>
                    </thead>
                    <tbody>
                        ${linhasTabela}
                    </tbody>
                </table>

                <div class="footer">
                    <p>Este documento é um resumo de notas e frequência. Em caso de divergência, procure a secretaria.</p>
                </div>
            </body>
            </html>
        `;

    const janelaImpressao = window.open('', '_blank');
    janelaImpressao.document.write(htmlImpressao);
    janelaImpressao.document.close();

    setTimeout(() => {
        janelaImpressao.print();
        janelaImpressao.close();
    }, 250);
}