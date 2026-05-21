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
        modal: new bootstrap.Modal(document.getElementById('formModal'))
    };

    const config = {
        alunos: {
            endpoint: '/api/alunos',
            title: 'Alunos',
            columns: [
                { key: 'id', label: 'ID' },
                { key: 'matricula', label: 'Matrícula' },
                { key: 'nome', label: 'Nome do Aluno' },
                { key: 'nomeResponsavel', label: 'Responsável' }
            ],
            formFields: [
                { name: 'loginCpf', label: 'CPF de Login (Apenas números)', type: 'text', required: true, onlyCreate: true },
                { name: 'loginSenha', label: 'Senha de Acesso', type: 'password', required: true, onlyCreate: true },
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
                { key: 'id', label: 'ID' },
                { key: 'nome', label: 'Nome do Professor' },
                { key: 'especialidade', label: 'Especialidade' },
                { key: 'telefone', label: 'Telefone' }
            ],
            formFields: [
                { name: 'loginCpf', label: 'CPF de Login', type: 'text', required: true, onlyCreate: true },
                { name: 'loginSenha', label: 'Senha de Acesso', type: 'password', required: true, onlyCreate: true },
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
                { key: 'id', label: 'ID' },
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
                { key: 'id', label: 'ID' },
                { key: 'nomeTurma', label: 'Nome' },
                { key: 'anoEscolar', label: 'Ano Escolar' },
                { key: 'turno', label: 'Turno', format: v => String(v).toUpperCase().includes('VESPERTINO') || v == 1 ? 'Vespertino' : 'Matutino' }
            ],
            formFields: [
                { name: 'nomeTurma', label: 'Nome da Turma', type: 'text', required: true },
                { name: 'anoEscolar', label: 'Ano Escolar', type: 'text', required: true },
                { name: 'anoLetivo', label: 'Ano Letivo', type: 'number', required: true },
                { name: 'turno', label: 'Turno', type: 'select', required: true, options: [{ value: '0', label: 'Matutino' }, { value: '1', label: 'Vespertino' }] }
            ]
        }
    };

    let currentEntity = 'alunos';

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
        el.pageTitle.textContent = `Gerenciar ${conf.title}`;
        el.tableHead.innerHTML = `<tr>${conf.columns.map(c => `<th>${c.label}</th>`).join('')}<th class="text-end">Ações</th></tr>`;
        el.tableBody.innerHTML = '<tr><td colspan="100%" class="text-center">Carregando...</td></tr>';

        try {
            const data = await apiFetch(conf.endpoint);
            if (!data?.length) {
                el.tableBody.innerHTML = '<tr><td colspan="100%" class="text-center text-muted">Nenhum registro encontrado.</td></tr>';
                return;
            }

            el.tableBody.innerHTML = data.map(item => `
                <tr>
                    ${conf.columns.map(c => `<td>${c.format ? c.format(item[c.key]) : (item[c.key] ?? '-')}</td>`).join('')}
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
                    inputHtml = `<input type="${f.type}" class="form-control" name="${f.name}" ${f.required ? 'required' : ''} ${f.maxLength ? `maxlength="${f.maxLength}"` : ''}>`;
                }

                return `
                <div class="mb-3">
                    <label class="form-label">${f.label}</label>
                    ${inputHtml}
                </div>`;
            }).join('');
    }

    async function editItem(id) {
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

    async function deleteItem(id) {
        if (!confirm('Tem certeza que deseja excluir?')) return;
        try {
            await apiFetch(`${config[currentEntity].endpoint}/${id}`, { method: 'DELETE' });
            exibirAlerta('Registro excluído!', 'success');
            loadData();
        } catch (error) {
            exibirAlerta(`Falha: ${error.message}`, 'danger');
        }
    }

    el.tableBody.addEventListener('click', (e) => {
        const btn = e.target.closest('button');
        if (!btn) return;
        const id = btn.dataset.id;
        if (btn.classList.contains('btn-edit')) editItem(id);
        if (btn.classList.contains('btn-delete')) deleteItem(id);
    });

    document.getElementById('btnAddNew').addEventListener('click', () => {
        el.form.reset();
        el.idInput.value = '';
        el.modalTitle.textContent = `Adicionar Novo(a) ${config[currentEntity].title}`;
        buildForm(null);
        el.modal.show();
    });

    document.getElementById('btnSaveEntity').addEventListener('click', async () => {
        if (!el.form.checkValidity()) return el.form.reportValidity();

        const conf = config[currentEntity];
        const id = el.idInput.value;

        const rawData = Object.fromEntries(new FormData(el.form));

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
            alert(`Falha ao salvar: ${error.message}`);
        }
    });

    document.querySelectorAll('.nav-link[data-entity]').forEach(link => {
        link.addEventListener('click', (e) => {
            e.preventDefault();
            document.querySelectorAll('.nav-link').forEach(n => n.classList.remove('active'));
            e.currentTarget.classList.add('active');
            currentEntity = e.currentTarget.getAttribute('data-entity');
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

    loadData();
});