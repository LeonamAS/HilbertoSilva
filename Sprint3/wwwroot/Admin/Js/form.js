import { state } from './state.js';
import { config } from './config.js';
import { apiFetch } from './api.js';
import { exibirAlerta, exibirErroFormulario } from './utils.js';
import { loadData } from './table.js';

export async function carregarDropdownsDinamicos() {
    const conf = config[state.currentEntity];

    const turmaField = conf.formFields.find(f => f.name === 'turmaId');
    if (turmaField) {
        try {
            const turmas = await apiFetch('/api/turma');
            turmaField.options = turmas.map(t => {
                const turnoStr = String(t.turno).toUpperCase().includes('VESPERTINO') || t.turno == 1 ? 'Vespertino' : 'Matutino';
                return {
                    value: t.id,
                    label: `${t.nomeTurma} - ${t.anoEscolar} (${turnoStr})`
                };
            });
        } catch (error) {
            console.error("Falha ao carregar turmas para o formulário:", error);
        }
    }

    const diarioField = conf.formFields.find(f => f.name === 'turmaDisciplinaId');
    if (diarioField) {
        try {
            let diarios = await apiFetch('/api/diarioclasse');

            if (state.currentEntity === 'boletim' && state.activeAlunoTurmaId) {
                if (state.activeAlunoTurmaId === 'null' || state.activeAlunoTurmaId === 'undefined') {
                    diarioField.options = [];
                    exibirErroFormulario("Este aluno não está matriculado em nenhuma turma. Edite o aluno e adicione uma turma primeiro.");
                    return;
                } else {
                    diarios = diarios.filter(d => String(d.turmaId) === String(state.activeAlunoTurmaId));
                }
            }

            diarioField.options = diarios.map(d => ({
                value: d.id,
                label: `${d.nomeDisciplina} (Prof. ${d.nomeProfessor})`
            }));
        } catch (error) {
            console.error("Erro ao carregar diários", error);
        }
    }
}

export function buildForm(id = null) {
    const conf = config[state.currentEntity];

    state.el.fieldsContainer.innerHTML = conf.formFields
        .filter(f => !(id && f.onlyCreate))
        .map(f => {
            let inputHtml = '';

            if (f.type === 'select') {
                inputHtml = `
                    <select class="form-select" name="${f.name}" ${f.required ? 'required' : ''}>
                        <option value="" disabled selected>Selecione...</option>
                        ${f.options.map(o => `<option value="${o.value}">${o.label}</option>`).join('')}
                    </select>`;
            }
            else if (f.type === 'password') {
                inputHtml = `
                    <div class="input-group">
                        <input type="password" class="form-control" name="${f.name}" id="input-${f.name}" ${f.required ? 'required' : ''}>
                        <button class="btn btn-outline-secondary toggle-password" type="button" data-target="input-${f.name}">
                            <i class="fas fa-eye"></i>
                        </button>
                    </div>`;
            }
            else {
                inputHtml = `<input type="${f.type}" class="form-control" name="${f.name}" ${f.required ? 'required' : ''} ${f.maxLength ? `maxlength="${f.maxLength}"` : ''} ${f.step ? `step="${f.step}"` : ''} ${f.disabled ? 'disabled' : ''} ${f.placeholder ? `placeholder="${f.placeholder}"` : ''}>`;
            }

            return `
            <div class="mb-3">
                <label class="form-label">${f.label}</label>
                ${inputHtml}
            </div>`;
        }).join('');
}

export function setupFormEvents() {
    document.getElementById('btnAddNew').addEventListener('click', async () => {
        const erroAntigo = document.getElementById('formErrorFeedback');
        if (erroAntigo) erroAntigo.classList.add('d-none');

        state.el.form.reset();
        state.el.idInput.value = '';
        state.el.modalTitle.textContent = `Adicionar Novo(a) ${config[state.currentEntity].title}`;

        await carregarDropdownsDinamicos();
        buildForm(null);

        if (state.currentEntity === 'boletim' && state.activeAlunoFilter) {
            const fieldAluno = state.el.form.elements['alunoNomeDisplay'];
            if (fieldAluno) {
                fieldAluno.value = state.activeAlunoNome;
            }
        }
        else if (state.currentEntity === 'diarioclasse' && state.activeTurmaFilter) {
            const fieldTurma = state.el.form.elements['turmaId'];
            if (fieldTurma) {
                fieldTurma.value = state.activeTurmaFilter;
                fieldTurma.readOnly = true;
            }
        }

        state.el.modal.show();
    });

    document.getElementById('btnSaveEntity').addEventListener('click', async () => {
        if (!state.el.form.checkValidity()) return state.el.form.reportValidity();

        const conf = config[state.currentEntity];
        const id = state.el.idInput.value;
        const rawData = Object.fromEntries(new FormData(state.el.form));

        if (rawData.loginCpf) rawData.loginCpf = rawData.loginCpf.replace(/\D/g, '');
        if (rawData.cpfResponsavel) rawData.cpfResponsavel = rawData.cpfResponsavel.replace(/\D/g, '');
        if (rawData.telefoneResponsavel) {
            rawData.telefoneResponsavel = rawData.telefoneResponsavel.replace(/\D/g, '');
        }

        if (state.currentEntity === 'alunos') {
            if (!rawData.telefoneResponsavel || rawData.telefoneResponsavel.length !== 11) {
                exibirErroFormulario("O telefone do responsável é obrigatório e deve conter exatamente 11 números (DDD + 9 dígitos).");
                return;
            }
        }

        const isCpfValido = (cpf) => {
            if (!cpf) return false;
            return cpf.replace(/\D/g, '').length === 11;
        };

        if (state.currentEntity === 'alunos' || state.currentEntity === 'professor') {
            if (!id && rawData.loginCpf && !isCpfValido(rawData.loginCpf)) {
                exibirErroFormulario("O CPF de Login está incorreto. Ele deve conter exatamente 11 números.");
                return;
            }

            if (state.currentEntity === 'alunos' && rawData.cpfResponsavel && !isCpfValido(rawData.cpfResponsavel)) {
                exibirErroFormulario("O CPF do Responsável está incorreto. Ele deve conter exatamente 11 números.");
                return;
            }
        }

        conf.formFields.forEach(f => {
            if (f.type === 'number' || f.type === 'select') {
                if (rawData[f.name] === "") {
                    rawData[f.name] = null;
                }
                else if (rawData[f.name] !== undefined && rawData[f.name] !== null) {
                    rawData[f.name] = Number(rawData[f.name]);
                }
            }
        });

        const payload = conf.buildPayload ? conf.buildPayload(rawData, id) : rawData;
        const url = !id && (state.currentEntity === 'alunos' || state.currentEntity === 'professor')
            ? `${conf.endpoint}/com-usuario`
            : (id ? `${conf.endpoint}/${id}` : conf.endpoint);

        try {
            await apiFetch(url, { method: id ? 'PUT' : 'POST', body: JSON.stringify(payload) });
            state.el.modal.hide();
            exibirAlerta('Salvo com sucesso!', 'success');
            loadData();
        } catch (error) {
            exibirErroFormulario(error.message);
        }
    });
}