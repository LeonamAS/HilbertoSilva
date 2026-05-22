document.addEventListener('DOMContentLoaded', () => {
    const token = localStorage.getItem('token');
    if (!token) return window.location.href = '/index.html';

    const el = {
        tableHead: document.getElementById('tableHead'),
        tableBody: document.getElementById('tableBody'),
        pageTitle: document.getElementById('pageTitle'),
        fieldsContainer: document.getElementById('formFieldsContainer'),
        form: document.getElementById('dynamicForm'),
        idInput: document.getElementById('entityId'),
        modalTitle: document.getElementById('modalTitle'),
        alert: document.getElementById('alertFeedback'),
        modal: new bootstrap.Modal(document.getElementById('formModal')),
        deleteModal: new bootstrap.Modal(document.getElementById('deleteConfirmModal'))
    };

    const config = {
        alunos: {
            endpoint: '/api/alunos',
            title: 'Alunos',
            columns: [
                { key: 'matricula', label: 'Matrícula' },
                {
                    key: 'nome',
                    label: 'Nome do Aluno',
                    format: (v, item) => `<a href="#" class="lnk-ver-boletim text-decoration-none fw-bold text-primary" data-id="${item.id}" data-nome="${item.nome}"><i class="fas fa-graduation-cap me-1"></i> ${v}</a>`
                },
                {
                    key: 'cpf',
                    label: 'CPF',
                    format: v => v ? v.replace(/(\d{3})(\d{3})(\d{3})(\d{2})/, "$1.$2.$3-$4") : '-'
                },
                { key: 'nomeResponsavel', label: 'Responsável' },
                {
                    key: 'telefoneResponsavel',
                    label: 'Tel. Responsável',
                    format: v => {
                        if (!v) return '-';
                        const num = v.replace(/\D/g, '');
                        if (num.length === 11) {
                            return num.replace(/(\d{2})(\d{5})(\d{4})/, "($1) $2-$3");
                        } else if (num.length === 10) {
                            return num.replace(/(\d{2})(\d{4})(\d{4})/, "($1) $2-$3");
                        }
                        return v;
                    }
                }
            ],
            formFields: [
                { name: 'loginCpf', label: 'CPF de Login (Apenas números)', type: 'text', required: true, onlyCreate: true },
                { name: 'loginSenha', label: 'Senha de Acesso (Senha Padrão: Aluno@123)', type: 'password', required: true, onlyCreate: true },
                { name: 'turmaId', label: 'ID da Turma (Opcional)', type: 'number' },
                { name: 'nome', label: 'Nome Completo', type: 'text', required: true },
                { name: 'dataNascimento', label: 'Data de Nascimento', type: 'date', required: true },
                { name: 'matricula', label: 'Matrícula', type: 'text', required: true },
                { name: 'nomeResponsavel', label: 'Nome do Responsável', type: 'text', required: true },
                { name: 'cpfResponsavel', label: 'CPF do Responsável', type: 'text', required: true },
                { name: 'telefoneResponsavel', label: 'Telefone do Responsável', type: 'text', required: true }
            ],
            buildPayload: (data, id) => id ? data : {
                usuario: { cpf: data.loginCpf, senha: data.loginSenha, tipoUsuario: "Aluno" },
                aluno: { ...data, turmaId: data.turmaId ? Number(data.turmaId) : null }
            }
        },
        professor: {
            endpoint: '/api/professor',
            title: 'Professores',
            columns: [
                { key: 'nome', label: 'Nome do Professor' },
                {
                    key: 'cpf',
                    label: 'CPF',
                    format: v => v ? v.replace(/(\d{3})(\d{3})(\d{3})(\d{2})/, "$1.$2.$3-$4") : '-'
                },
                { key: 'especialidade', label: 'Especialidade' },
                { key: 'telefone', label: 'Telefone' }
            ],
            formFields: [
                { name: 'loginCpf', label: 'CPF de Login', type: 'text', required: true, onlyCreate: true },
                { name: 'loginSenha', label: 'Senha de Acesso (Senha Padrão: Professor@123)', type: 'password', required: true, onlyCreate: true },
                { name: 'nome', label: 'Nome do Professor', type: 'text', required: true },
                { name: 'telefone', label: 'Telefone', type: 'text', required: true },
                { name: 'especialidade', label: 'Especialidade', type: 'text' }
            ],
            buildPayload: (data, id) => id ? data : {
                usuario: { cpf: data.loginCpf, senha: data.loginSenha, tipoUsuario: "Professor" },
                professor: data
            }
        },
        disciplina: {
            endpoint: '/api/disciplina',
            title: 'Disciplinas',
            columns: [
                { key: 'nome', label: 'Nome da Disciplina' },
                { key: 'cargaHoraria', label: 'Carga Horária (Horas)', format: v => `${v}h` }
            ],
            formFields: [
                { name: 'nome', label: 'Nome da Disciplina (ex: Matemática)', type: 'text', required: true, maxLength: 50 },
                { name: 'cargaHoraria', label: 'Carga Horária (Horas)', type: 'number', required: true }
            ]
        },
        turma: {
            endpoint: '/api/turma',
            title: 'Turmas',
            columns: [
                { key: 'nomeTurma', label: 'Nome', format: (v, item) => `<a href="#" class="lnk-ver-diario text-decoration-none fw-bold text-primary" data-id="${item.id}" data-nome="${item.nomeTurma}"><i class="fas fa-clipboard-list me-1"></i> ${v}</a>` },
                { key: 'anoEscolar', label: 'Ano Escolar' },
                { key: 'turno', label: 'Turno', format: v => String(v).toUpperCase().includes('VESPERTINO') || v == 1 ? 'Vespertino' : 'Matutino' }
            ],
            formFields: [
                { name: 'nomeTurma', label: 'Nome da Turma', type: 'text', required: true },
                { name: 'anoEscolar', label: 'Ano Escolar', type: 'text', required: true },
                { name: 'anoLetivo', label: 'Ano Letivo', type: 'number', required: true },
                { name: 'turno', label: 'Turno', type: 'select', required: true, options: [{ value: '0', label: 'Matutino' }, { value: '1', label: 'Vespertino' }] }
            ]
        },
        diarioclasse: {
            endpoint: '/api/diarioclasse',
            title: 'Diários de Classe',
            columns: [
                { key: 'id', label: 'ID' },
                { key: 'nomeDisciplina', label: 'Disciplina' },
                { key: 'nomeProfessor', label: 'Professor Responsável' }
            ],
            formFields: [
                { name: 'turmaId', label: 'ID da Turma', type: 'number', required: true, onlyCreate: true },
                { name: 'disciplinaId', label: 'ID da Disciplina', type: 'number', required: true, onlyCreate: true },
                { name: 'professorId', label: 'ID do Professor Responsável', type: 'number', required: true }
            ]
        },
        boletim: {
            endpoint: '/api/boletim',
            title: 'Boletins',
            columns: [
                { key: 'nomeDisciplina', label: 'Disciplina' },
                { key: 'notaU1', label: '1ª Unidade', format: v => Number(v) < 6 ? `<span class="text-danger fw-bold">${Number(v).toFixed(1)}</span>` : Number(v).toFixed(1) },
                { key: 'notaU2', label: '2ª Unidade', format: v => Number(v) < 6 ? `<span class="text-danger fw-bold">${Number(v).toFixed(1)}</span>` : Number(v).toFixed(1) },
                { key: 'notaU3', label: '3ª Unidade', format: v => Number(v) < 6 ? `<span class="text-danger fw-bold">${Number(v).toFixed(1)}</span>` : Number(v).toFixed(1) },
                { key: 'mediaFinal', label: 'Média Final', format: v => Number(v) < 6 ? `<strong class="text-danger">${Number(v).toFixed(1)}</strong>` : `<strong>${Number(v).toFixed(1)}</strong>` },
                { key: 'frequencia', label: 'Frequência', format: v => Number(v) < 70 ? `<span class="text-danger fw-bold">${Number(v).toFixed(0)}%</span>` : `${Number(v).toFixed(0)}%` }
            ],
            formFields: [
                { name: 'alunoId', label: 'ID do Aluno', type: 'number', required: true, onlyCreate: true },
                { name: 'turmaDisciplinaId', label: 'ID do Diário (Turma/Disciplina)', type: 'number', required: true, onlyCreate: true },
                { name: 'notaU1', label: 'Nota - Unidade 1', type: 'number', required: true, step: '0.1' },
                { name: 'notaU2', label: 'Nota - Unidade 2', type: 'number', required: true, step: '0.1' },
                { name: 'notaU3', label: 'Nota - Unidade 3', type: 'number', required: true, step: '0.1' },
                { name: 'frequencia', label: 'Percentual de Frequência (0 a 100)', type: 'number', required: true }
            ]
        }
    };

    let currentEntity = 'alunos';

    let activeAlunoFilter = null;
    let activeAlunoNome = '';
    let activeTurmaFilter = null;
    let activeTurmaNome = '';
    let itemToDeleteId = null;
    let currentTableData = [];

    async function apiFetch(url, options = {}) {
        const response = await fetch(url, {
            ...options,
            headers: { 'Content-Type': 'application/json', 'Authorization': `Bearer ${token}`, ...options.headers }
        });

        if ([401, 403].includes(response.status)) {
            localStorage.removeItem('token');
            window.location.href = '/index.html';
            throw new Error('Sessão expirada.');
        }
        if (response.status === 204) return null;

        const data = await response.json().catch(() => ({}));
        if (!response.ok) {
            throw new Error(data.errors ? Object.values(data.errors).map(e => e.join(', ')).join(' | ') : data.mensagem || `Erro: ${response.status}`);
        }
        return data;
    }

    function exibirAlerta(msg, tipo) {
        el.alert.textContent = msg;
        el.alert.className = `alert alert-${tipo} mb-4`;
        setTimeout(() => el.alert.classList.add('d-none'), 5000);
    }

    async function loadData() {
        const conf = config[currentEntity];

        const oldBackBtnAluno = document.getElementById('btnVoltarAlunos');
        if (oldBackBtnAluno) oldBackBtnAluno.remove();
        const oldBackBtnTurma = document.getElementById('btnVoltarTurmas');
        if (oldBackBtnTurma) oldBackBtnTurma.remove();

        if (currentEntity === 'boletim' && activeAlunoFilter) {
            el.pageTitle.innerHTML = `
                <button class="btn btn-sm btn-outline-secondary me-3" id="btnVoltarAlunos">
                    <i class="fas fa-arrow-left"></i> Voltar
                </button> 
                Boletim: <span class="text-muted ms-1">${activeAlunoNome}</span>
                <button class="btn btn-sm btn-outline-primary ms-3" id="btnPrintBoletim">
                    <i class="fas fa-print"></i> Imprimir
                </button>
            `;

            document.getElementById('btnVoltarAlunos').addEventListener('click', () => {
                currentEntity = 'alunos';
                activeAlunoFilter = null;
                activeAlunoNome = '';
                loadData();
            });

            document.getElementById('btnPrintBoletim').addEventListener('click', imprimirBoletim);
        }
        else if (currentEntity === 'diarioclasse' && activeTurmaFilter) {
            el.pageTitle.innerHTML = `<button class="btn btn-sm btn-outline-secondary me-3" id="btnVoltarTurmas"><i class="fas fa-arrow-left"></i> Voltar</button> Diário de Classe: <span class="text-muted ms-1">${activeTurmaNome}</span>`;
            document.getElementById('btnVoltarTurmas').addEventListener('click', () => {
                currentEntity = 'turma';
                activeTurmaFilter = null;
                activeTurmaNome = '';
                loadData();
            });
        }
        else {
            el.pageTitle.textContent = `Gerenciar ${conf.title}`;
        }

        el.tableHead.innerHTML = `<tr>${conf.columns.map(c => `<th>${c.label}</th>`).join('')}<th class="text-end">Ações</th></tr>`;
        el.tableBody.innerHTML = '<tr><td colspan="100%" class="text-center">Carregando...</td></tr>';

        try {
            let data = await apiFetch(conf.endpoint);

            if (currentEntity === 'boletim' && activeAlunoFilter) {
                data = data.filter(b => b.alunoId == activeAlunoFilter);
            }
            else if (currentEntity === 'diarioclasse' && activeTurmaFilter) {
                data = data.filter(d => d.turmaId == activeTurmaFilter);
            }
            currentTableData = data;

            if (!data?.length) {
                el.tableBody.innerHTML = '<tr><td colspan="100%" class="text-center text-muted">Nenhum registro encontrado.</td></tr>';
                return;
            }

            el.tableBody.innerHTML = data.map(item => `
                <tr>
                    ${conf.columns.map(c => `<td data-label="${c.label}">${c.format ? c.format(item[c.key], item) : (item[c.key] ?? '-')}</td>`).join('')}
                    <td class="text-end">
                        <button class="btn btn-sm btn-outline-primary me-2 btn-edit" data-id="${item.id}"><i class="fas fa-edit"></i></button>
                        <button class="btn btn-sm btn-outline-danger btn-delete" data-id="${item.id}"><i class="fas fa-trash"></i></button>
                    </td>
                </tr>`).join('');
        } catch (error) {
            exibirAlerta(`Erro: ${error.message}`, 'danger');
            el.tableBody.innerHTML = '<tr><td colspan="100%" class="text-center text-danger">Falha ao carregar dados.</td></tr>';
        }
    }

    function buildForm(id = null) {
        el.fieldsContainer.innerHTML = config[currentEntity].formFields
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
                    inputHtml = `<input type="${f.type}" class="form-control" name="${f.name}" ${f.required ? 'required' : ''} ${f.maxLength ? `maxlength="${f.maxLength}"` : ''} ${f.step ? `step="${f.step}"` : ''}>`;
                }

                return `
                <div class="mb-3">
                    <label class="form-label">${f.label}</label>
                    ${inputHtml}
                </div>`;
            }).join('');
    }

    async function editItem(id) {
        const erroAntigo = document.getElementById('formErrorFeedback');
        if (erroAntigo) erroAntigo.classList.add('d-none');

        const conf = config[currentEntity];
        try {
            const item = await apiFetch(`${conf.endpoint}/${id}`);
            el.form.reset();
            buildForm(id);
            el.idInput.value = id;
            el.modalTitle.textContent = `Editar ${conf.title}`;

            conf.formFields.forEach(f => {
                const input = el.form.elements[f.name];
                if (!input) return;

                let val = item[f.name];
                if (f.type === 'date' && val) val = val.split('T')[0];
                if (f.name === 'turno') val = String(val).toUpperCase().includes('VESPERTINO') || val == 1 ? '1' : '0';

                input.value = val ?? '';
                if (f.name === 'matricula') input.disabled = true;
            });
            el.modal.show();
        } catch (error) {
            exibirAlerta(`Erro ao buscar: ${error.message}`, 'danger');
        }
    }

    function deleteItem(id) {
        itemToDeleteId = id;
        el.deleteModal.show();
    }

    el.tableBody.addEventListener('click', (e) => {
        const lnkBoletim = e.target.closest('.lnk-ver-boletim');
        if (lnkBoletim) {
            e.preventDefault();
            activeAlunoFilter = lnkBoletim.dataset.id;
            activeAlunoNome = lnkBoletim.dataset.nome;
            currentEntity = 'boletim';
            document.querySelectorAll('.nav-link').forEach(n => n.classList.remove('active'));
            loadData();
            return;
        }

        const lnkDiario = e.target.closest('.lnk-ver-diario');
        if (lnkDiario) {
            e.preventDefault();
            activeTurmaFilter = lnkDiario.dataset.id;
            activeTurmaNome = lnkDiario.dataset.nome;
            currentEntity = 'diarioclasse';
            document.querySelectorAll('.nav-link').forEach(n => n.classList.remove('active'));
            loadData();
            return;
        }

        const btn = e.target.closest('button');
        if (!btn) return;
        const id = btn.dataset.id;
        if (btn.classList.contains('btn-edit')) editItem(id);
        if (btn.classList.contains('btn-delete')) deleteItem(id);
    });

    document.getElementById('btnAddNew').addEventListener('click', () => {
        const erroAntigo = document.getElementById('formErrorFeedback');
        if (erroAntigo) erroAntigo.classList.add('d-none');

        el.form.reset();
        el.idInput.value = '';
        el.modalTitle.textContent = `Adicionar Novo(a) ${config[currentEntity].title}`;
        buildForm(null);

        if (currentEntity === 'boletim' && activeAlunoFilter) {
            const fieldAluno = el.form.elements['alunoId'];
            if (fieldAluno) {
                fieldAluno.value = activeAlunoFilter;
                fieldAluno.readOnly = true;
            }
        }
        else if (currentEntity === 'diarioclasse' && activeTurmaFilter) {
            const fieldTurma = el.form.elements['turmaId'];
            if (fieldTurma) {
                fieldTurma.value = activeTurmaFilter;
                fieldTurma.readOnly = true;
            }
        }

        el.modal.show();
    });

    document.getElementById('btnSaveEntity').addEventListener('click', async () => {
        if (!el.form.checkValidity()) return el.form.reportValidity();

        const conf = config[currentEntity];
        const id = el.idInput.value;

        const rawData = Object.fromEntries(new FormData(el.form));

        const isCpfValido = (cpf) => {
            if (!cpf) return false;
            return cpf.replace(/\D/g, '').length === 11;
        };

        if (currentEntity === 'alunos' || currentEntity === 'professor') {
            if (!id && rawData.loginCpf && !isCpfValido(rawData.loginCpf)) {
                exibirErroFormulario("O CPF de Login está incorreto. Ele deve conter exatamente 11 números.");
                return;
            }

            if (currentEntity === 'alunos' && rawData.cpfResponsavel && !isCpfValido(rawData.cpfResponsavel)) {
                exibirErroFormulario("O CPF do Responsável está incorreto. Ele deve conter exatamente 11 números.");
                return;
            }
        }

        conf.formFields.forEach(f => {
            if ((f.type === 'number' || f.type === 'select') && rawData[f.name]) {
                rawData[f.name] = Number(rawData[f.name]);
            }
        });

        const payload = conf.buildPayload ? conf.buildPayload(rawData, id) : rawData;
        const url = !id && (currentEntity === 'alunos' || currentEntity === 'professor') ? `${conf.endpoint}/com-usuario` : (id ? `${conf.endpoint}/${id}` : conf.endpoint);

        try {
            await apiFetch(url, { method: id ? 'PUT' : 'POST', body: JSON.stringify(payload) });
            el.modal.hide();
            exibirAlerta('Salvo com sucesso!', 'success');
            loadData();
        } catch (error) {
            exibirErroFormulario(error.message);
        }
    });

    document.querySelectorAll('.nav-link[data-entity]').forEach(link => {
        link.addEventListener('click', (e) => {
            e.preventDefault();
            document.querySelectorAll('.nav-link').forEach(n => n.classList.remove('active'));
            e.currentTarget.classList.add('active');
            currentEntity = e.currentTarget.getAttribute('data-entity');

            activeAlunoFilter = null;
            activeAlunoNome = '';
            activeTurmaFilter = null;
            activeTurmaNome = '';

            loadData();
        });
    });

    document.getElementById('btnLogout').addEventListener('click', () => {
        localStorage.removeItem('token');
        window.location.href = '/index.html';
    });

    document.addEventListener('click', (e) => {
        const toggleBtn = e.target.closest('.toggle-password');
        if (!toggleBtn) return;

        const targetInput = document.getElementById(toggleBtn.dataset.target);
        const icon = toggleBtn.querySelector('i');

        if (targetInput.type === 'password') {
            targetInput.type = 'text';
            icon.classList.replace('fa-eye', 'fa-eye-slash');
        } else {
            targetInput.type = 'password';
            icon.classList.replace('fa-eye-slash', 'fa-eye');
        }
    });

    document.getElementById('btnConfirmDelete').addEventListener('click', async () => {
        if (!itemToDeleteId) return;

        try {
            await apiFetch(`${config[currentEntity].endpoint}/${itemToDeleteId}`, { method: 'DELETE' });
            el.deleteModal.hide();
            exibirAlerta('Registro excluído com sucesso!', 'success');
            loadData();
        } catch (error) {
            el.deleteModal.hide();
            exibirAlerta(`Falha ao excluir: ${error.message}`, 'danger');
        } finally {
            itemToDeleteId = null;
        }
    });

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

    function exibirErroFormulario(msg) {
        let errorDiv = document.getElementById('formErrorFeedback');

        if (!errorDiv) {
            errorDiv = document.createElement('div');
            errorDiv.id = 'formErrorFeedback';
            el.fieldsContainer.parentNode.insertBefore(errorDiv, el.fieldsContainer);
        }

        errorDiv.className = 'alert alert-danger mb-3';
        errorDiv.innerHTML = `<i class="fas fa-exclamation-triangle me-2"></i> ${msg}`;
        errorDiv.classList.remove('d-none');

        setTimeout(() => {
            if (errorDiv) errorDiv.classList.add('d-none');
        }, 6000);
    }

    loadData();
});