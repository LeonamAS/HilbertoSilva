document.addEventListener('DOMContentLoaded', () => {
    // Validação de Autenticação
    const token = localStorage.getItem('token');
    if (!token) {
        window.location.href = '/index.html';
        return;
    }

    // Elementos da UI
    const tableHead = document.getElementById('tableHead');
    const tableBody = document.getElementById('tableBody');
    const pageTitle = document.getElementById('pageTitle');
    const formFieldsContainer = document.getElementById('formFieldsContainer');
    const dynamicForm = document.getElementById('dynamicForm');
    const entityIdInput = document.getElementById('entityId');
    const modalTitle = document.getElementById('modalTitle');
    const alertFeedback = document.getElementById('alertFeedback');

    const formModal = new bootstrap.Modal(document.getElementById('formModal'));

    // ==========================================
    // CONFIGURAÇÃO DE ENTIDADES (Espelhando DTOs)
    // ==========================================
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
                // Campos de Usuário
                { name: 'loginCpf', label: 'CPF de Login do Aluno (Apenas números)', type: 'text', required: true, onlyCreate: true },
                { name: 'loginSenha', label: 'Senha de Acesso (Senha padrão: Aluno@123)', type: 'password', required: true, onlyCreate: true },

                // Campos de Aluno
                { name: 'turmaId', label: 'ID da Turma (Opcional)', type: 'number', required: false },
                { name: 'nome', label: 'Nome Completo', type: 'text', required: true },
                { name: 'dataNascimento', label: 'Data de Nascimento', type: 'date', required: true },
                { name: 'matricula', label: 'Matrícula', type: 'text', required: true },
                { name: 'nomeResponsavel', label: 'Nome do Responsável', type: 'text', required: true },
                { name: 'cpfResponsavel', label: 'CPF do Responsável (Apenas números)', type: 'text', required: true },
                { name: 'telefoneResponsavel', label: 'Telefone do Responsável', type: 'text', required: true }
            ]
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
                // Campos de Usuário
                { name: 'loginCpf', label: 'CPF de Login (Apenas números)', type: 'text', required: true, onlyCreate: true },
                { name: 'loginSenha', label: 'Senha de Acesso (Senha padrão: Professor@123)', type: 'password', required: true, onlyCreate: true },

                // Campos de Professor
                { name: 'nome', label: 'Nome do Professor', type: 'text', required: true },
                { name: 'telefone', label: 'Telefone', type: 'text', required: true },
                { name: 'especialidade', label: 'Especialidade', type: 'text', required: false }
            ]
        },
        turma: {
            endpoint: '/api/turma',
            title: 'Turmas',
            columns: [
                { key: 'id', label: 'ID' },
                { key: 'nomeTurma', label: 'Nome' },
                { key: 'anoEscolar', label: 'Ano Escolar' },
                { key: 'turno', label: 'Turno' }
            ],
            formFields: [
                { name: 'nomeTurma', label: 'Nome da Turma (ex: A, B...)', type: 'text', required: true },
                { name: 'anoEscolar', label: 'Ano Escolar (ex: 6º Ano)', type: 'text', required: true },
                { name: 'anoLetivo', label: 'Ano Letivo (ex: 2024)', type: 'number', required: true },
                {
                    name: 'turno',
                    label: 'Turno',
                    type: 'select',
                    required: true,
                    options: [
                        { value: '0', label: 'Matutino' },
                        { value: '1', label: 'Vespertino' }
                    ]
                }
            ]
        }
    };

    let currentEntity = 'alunos';

    // ==========================================
    // FUNÇÕES DE API
    // ==========================================
    async function apiFetch(url, options = {}) {
        const headers = {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${token}`,
            ...options.headers
        };

        const response = await fetch(url, { ...options, headers });

        if (response.status === 401 || response.status === 403) {
            localStorage.removeItem('token');
            window.location.href = '/index.html';
            throw new Error('Sessão expirada ou acesso negado.');
        }

        if (response.status === 204) return null;

        const data = await response.json().catch(() => ({}));

        if (!response.ok) {
            if (data.errors) {
                const primeirasMensagens = Object.values(data.errors).map(err => err.join(', ')).join(' | ');
                throw new Error(primeirasMensagens);
            }
            throw new Error(data.mensagem || `Erro HTTP: ${response.status}`);
        }

        return data;
    }

    // ==========================================
    // RENDERIZAÇÃO DA INTERFACE
    // ==========================================
    async function loadData() {
        const conf = config[currentEntity];
        pageTitle.textContent = `Gerenciar ${conf.title}`;

        let thHtml = '<tr>';
        conf.columns.forEach(col => thHtml += `<th>${col.label}</th>`);
        thHtml += '<th class="text-end">Ações</th></tr>';
        tableHead.innerHTML = thHtml;

        tableBody.innerHTML = '<tr><td colspan="100%" class="text-center">Carregando...</td></tr>';

        try {
            const data = await apiFetch(conf.endpoint);

            if (!data || data.length === 0) {
                tableBody.innerHTML = '<tr><td colspan="100%" class="text-center text-muted">Nenhum registro encontrado.</td></tr>';
                return;
            }

            tableBody.innerHTML = data.map(item => {
                let tr = '<tr>';
                conf.columns.forEach(col => {
                    let valor = item[col.key];

                    if (col.key === 'turno') {
                        if (valor === 0 || valor === 'MATUTINO' || String(valor).toUpperCase() === 'MATUTINO') valor = 'Matutino';
                        else if (valor === 1 || valor === 'VESPERTINO' || String(valor).toUpperCase() === 'VESPERTINO') valor = 'Vespertino';
                    }

                    tr += `<td>${valor !== null && valor !== undefined && valor !== '' ? valor : '-'}</td>`;
                });
                tr += `
                    <td class="text-end">
                        <button class="btn btn-sm btn-outline-primary me-2" onclick="editItem(${item.id})"><i class="fas fa-edit"></i></button>
                        <button class="btn btn-sm btn-outline-danger" onclick="deleteItem(${item.id})"><i class="fas fa-trash"></i></button>
                    </td>
                </tr>`;
                return tr;
            }).join('');

        } catch (error) {
            exibirAlerta(`Erro ao carregar dados: ${error.message}`, 'danger');
            tableBody.innerHTML = '<tr><td colspan="100%" class="text-center text-danger">Falha ao carregar os dados.</td></tr>';
        }
    }

    function buildForm(id = null) {
        const conf = config[currentEntity];
        formFieldsContainer.innerHTML = '';

        conf.formFields.forEach(field => {
            if (id && field.onlyCreate) return;

            let inputHtml = '';

            if (field.type === 'select') {
                const optionsHtml = field.options.map(opt =>
                    `<option value="${opt.value}">${opt.label}</option>`
                ).join('');

                inputHtml = `
                    <select class="form-select" name="${field.name}" ${field.required ? 'required' : ''}>
                        <option value="" disabled selected>Selecione...</option>
                        ${optionsHtml}
                    </select>`;
            } else {
                inputHtml = `
                    <input type="${field.type}" class="form-control" name="${field.name}" 
                        ${field.required ? 'required' : ''} 
                        ${field.maxLength ? `maxlength="${field.maxLength}"` : ''}>`;
            }

            formFieldsContainer.innerHTML += `
                <div class="mb-3">
                    <label class="form-label">${field.label}</label>
                    ${inputHtml}
                </div>
            `;
        });
    }

    // ==========================================
    // AÇÕES DE CRUD
    // ==========================================
    window.editItem = async (id) => {
        const conf = config[currentEntity];
        try {
            const item = await apiFetch(`${conf.endpoint}/${id}`);

            dynamicForm.reset();
            buildForm(id);

            entityIdInput.value = id;
            modalTitle.textContent = `Editar ${conf.title}`;

            conf.formFields.forEach(field => {
                const input = dynamicForm.elements[field.name];
                if (!input) return;

                let valor = item[field.name];

                if (field.type === 'date' && valor) {
                    input.value = valor.split('T')[0];
                }
                else if (field.name === 'turno') {
                    if (String(valor).toUpperCase() === 'MATUTINO') input.value = '0';
                    else if (String(valor).toUpperCase() === 'VESPERTINO') input.value = '1';
                    else input.value = (valor !== null && valor !== undefined) ? valor : '';
                }
                else {
                    input.value = (valor !== null && valor !== undefined) ? valor : '';
                }

                if (id && field.name === 'matricula') {
                    input.disabled = true;
                }
            });

            formModal.show();
        } catch (error) {
            exibirAlerta(`Erro ao buscar dados: ${error.message}`, 'danger');
        }
    };

    window.deleteItem = async (id) => {
        if (!confirm('Tem certeza que deseja excluir este registro? A ação não pode ser desfeita.')) return;

        try {
            await apiFetch(`${config[currentEntity].endpoint}/${id}`, { method: 'DELETE' });
            exibirAlerta('Registro excluído com sucesso!', 'success');
            loadData();
        } catch (error) {
            exibirAlerta(`Falha ao excluir: ${error.message}`, 'danger');
        }
    };

    // ==========================================
    // EVENT LISTENERS
    // ==========================================
    document.getElementById('btnAddNew').addEventListener('click', () => {
        dynamicForm.reset();
        entityIdInput.value = '';
        modalTitle.textContent = `Adicionar Novo(a) ${config[currentEntity].title}`;
        buildForm(null);
        formModal.show();
    });

    document.getElementById('btnSaveEntity').addEventListener('click', async () => {
        if (!dynamicForm.checkValidity()) {
            dynamicForm.reportValidity();
            return;
        }

        const conf = config[currentEntity];
        const id = entityIdInput.value;
        const method = id ? 'PUT' : 'POST';
        let url = id ? `${conf.endpoint}/${id}` : conf.endpoint;
        let payload = {};

        if (currentEntity === 'alunos' && !id) {
            url = `${conf.endpoint}/com-usuario`;

            payload = {
                usuario: {
                    cpf: dynamicForm.elements['loginCpf'].value,
                    senha: dynamicForm.elements['loginSenha'].value,
                    tipoUsuario: "Aluno"
                },
                aluno: {
                    turmaId: dynamicForm.elements['turmaId'].value ? Number(dynamicForm.elements['turmaId'].value) : null,
                    nome: dynamicForm.elements['nome'].value,
                    dataNascimento: dynamicForm.elements['dataNascimento'].value,
                    matricula: dynamicForm.elements['matricula'].value,
                    nomeResponsavel: dynamicForm.elements['nomeResponsavel'].value,
                    cpfResponsavel: dynamicForm.elements['cpfResponsavel'].value,
                    telefoneResponsavel: dynamicForm.elements['telefoneResponsavel'].value
                }
            };
        } else if (currentEntity === 'professor' && !id) {
            url = `${conf.endpoint}/com-usuario`;
            payload = {
                usuario: {
                    cpf: dynamicForm.elements['loginCpf'].value,
                    senha: dynamicForm.elements['loginSenha'].value,
                    tipoUsuario: "Professor"
                },
                professor: {
                    nome: dynamicForm.elements['nome'].value,
                    telefone: dynamicForm.elements['telefone'].value,
                    especialidade: dynamicForm.elements['especialidade'].value || null
                }
            };
        } else {
            conf.formFields.forEach(field => {
                const input = dynamicForm.elements[field.name];
                if (!input || (id && input.disabled)) return;

                let val = input.value;

                if (field.type === 'number' || (field.type === 'select' && !isNaN(val))) {
                    val = val !== '' ? Number(val) : null;
                }

                payload[field.name] = val;
            });
        }

        try {
            await apiFetch(url, {
                method: method,
                body: JSON.stringify(payload)
            });

            formModal.hide();
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

    document.getElementById('btnLogout').addEventListener('click', (e) => {
        e.preventDefault();
        localStorage.removeItem('token');
        window.location.href = '/index.html';
    });

    function exibirAlerta(mensagem, tipo) {
        alertFeedback.textContent = mensagem;
        alertFeedback.className = `alert alert-${tipo} mb-4`;
        setTimeout(() => alertFeedback.classList.add('d-none'), 5000);
    }

    loadData();
});