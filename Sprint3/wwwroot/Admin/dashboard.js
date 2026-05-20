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
    // 🛠️ CONFIGURAÇÃO DE ENTIDADES (Espelhando DTOs)
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
                { name: 'usuarioId', label: 'ID de Usuário (Login)', type: 'number', required: true },
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
                { name: 'usuarioId', label: 'ID de Usuário (Login)', type: 'number', required: true },
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
                { name: 'nomeTurma', label: 'Nome da Turma', type: 'text', required: true },
                { name: 'anoEscolar', label: 'Ano Escolar (ex: Ensino Fundamental)', type: 'text', required: true },
                { name: 'anoLetivo', label: 'Ano Letivo (ex: 2024)', type: 'number', required: true },
                { name: 'turno', label: 'Turno (ex: Matutino)', type: 'text', required: true }
            ]
        }
    };

    let currentEntity = 'alunos';

    // ==========================================
    // 📡 FUNÇÕES DE API
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
            // Tenta extrair mensagens de validação padrão do .NET (ValidationProblemDetails)
            if (data.errors) {
                const primeirasMensagens = Object.values(data.errors).map(err => err.join(', ')).join(' | ');
                throw new Error(primeirasMensagens);
            }
            throw new Error(data.mensagem || `Erro HTTP: ${response.status}`);
        }

        return data;
    }

    // ==========================================
    // 🖥️ RENDERIZAÇÃO DA INTERFACE
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
                    tr += `<td>${item[col.key] || '-'}</td>`;
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

    function buildForm() {
        const conf = config[currentEntity];
        formFieldsContainer.innerHTML = '';

        conf.formFields.forEach(field => {
            // Nota de UI: Desabilita os campos de criação (UsuarioId, Matricula) na hora de atualizar, 
            // já que a sua "UpdateDto" não exige ou aceita eles
            formFieldsContainer.innerHTML += `
                <div class="mb-3">
                    <label class="form-label">${field.label}</label>
                    <input type="${field.type}" class="form-control" name="${field.name}" 
                        ${field.required ? 'required' : ''} 
                        ${field.maxLength ? `maxlength="${field.maxLength}"` : ''}>
                </div>
            `;
        });
    }

    // ==========================================
    // ⚙️ AÇÕES DE CRUD
    // ==========================================
    window.editItem = async (id) => {
        const conf = config[currentEntity];
        try {
            const item = await apiFetch(`${conf.endpoint}/${id}`);

            dynamicForm.reset();
            buildForm();

            entityIdInput.value = id;
            modalTitle.textContent = `Editar ${conf.title}`;

            conf.formFields.forEach(field => {
                const input = dynamicForm.elements[field.name];
                if (!input) return;

                // Formata DataNascimento (Ex: de "2024-05-20T00:00:00" para "2024-05-20" p/ o HTML5)
                if (field.type === 'date' && item[field.name]) {
                    input.value = item[field.name].split('T')[0];
                } else {
                    input.value = item[field.name] || '';
                }

                // Opcional: Se for modo Edição, campos exclusivos de "CreateDto" ficam desabilitados (read-only)
                if (id && (field.name === 'usuarioId' || field.name === 'matricula')) {
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
    // 🖱️ EVENT LISTENERS
    // ==========================================
    document.getElementById('btnAddNew').addEventListener('click', () => {
        dynamicForm.reset();
        entityIdInput.value = '';
        modalTitle.textContent = `Adicionar Novo(a) ${config[currentEntity].title}`;
        buildForm();
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
        const url = id ? `${conf.endpoint}/${id}` : conf.endpoint;

        const payload = {};
        conf.formFields.forEach(field => {
            const input = dynamicForm.elements[field.name];

            // Ignora campos que foram desabilitados na edição (como UsuarioId no UpdateAlunoDto)
            if (id && input.disabled) return;

            let val = input.value;

            // Converte os tipos para corresponder aos DTOs (C# não aceita string em int)
            if (field.type === 'number') {
                val = val ? Number(val) : null;
            }

            payload[field.name] = val;
        });

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

    // Navegação Sidebar
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