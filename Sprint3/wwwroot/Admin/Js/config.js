import { state } from './state.js';

export const config = {
    alunos: {
        endpoint: '/api/alunos',
        title: 'Alunos',
        columns: [
            { key: 'matricula', label: 'Matrícula' },
            {
                key: 'nome',
                label: 'Nome do Aluno',
                format: (v, item) => `<a href="#" class="lnk-ver-boletim text-decoration-none fw-bold text-primary" 
                    data-id="${item.id}" data-nome="${item.nome}" data-turma="${item.turmaId}"><i class="fas fa-graduation-cap me-1"></i> ${v}</a>`
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
                    if (num.length === 11) return num.replace(/(\d{2})(\d{5})(\d{4})/, "($1) $2-$3");
                    if (num.length === 10) return num.replace(/(\d{2})(\d{4})(\d{4})/, "($1) $2-$3");
                    return v;
                }
            }
        ],
        formFields: [
            { name: 'matricula', label: 'Matrícula', type: 'text', disabled: true, placeholder: 'Gerada automaticamente' },
            { name: 'loginCpf', label: 'CPF de Login (Apenas números)', type: 'text', required: true, onlyCreate: true },
            { name: 'loginSenha', label: 'Senha de Acesso (Senha Padrão: Aluno@123)', type: 'password', required: true, onlyCreate: true },
            { name: 'turmaId', label: 'Turma', type: 'select', options: [], required: true },
            { name: 'nome', label: 'Nome Completo', type: 'text', required: true },
            { name: 'dataNascimento', label: 'Data de Nascimento', type: 'date', required: true },
            { name: 'nomeResponsavel', label: 'Nome do Responsável', type: 'text', required: true },
            { name: 'cpfResponsavel', label: 'CPF do Responsável', type: 'text', required: true },
            { name: 'telefoneResponsavel', label: 'Telefone do Responsável', type: 'text', required: true }
        ],
        buildPayload: (data, id) => {
            const turmaIdTratado = Number(data.turmaId);
            if (id) {
                return { ...data, turmaId: turmaIdTratado };
            } else {
                return {
                    usuario: { cpf: data.loginCpf, senha: data.loginSenha, tipoUsuario: "Aluno" },
                    aluno: { ...data, turmaId: turmaIdTratado }
                };
            }
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
            { name: 'alunoNomeDisplay', label: 'Aluno', type: 'text', disabled: true, onlyCreate: true },
            { name: 'turmaDisciplinaId', label: 'Diário de Classe (Disciplina)', type: 'select', required: true, options: [], onlyCreate: true },
            { name: 'notaU1', label: 'Nota - Unidade 1', type: 'number', required: true, step: '0.1' },
            { name: 'notaU2', label: 'Nota - Unidade 2', type: 'number', required: true, step: '0.1' },
            { name: 'notaU3', label: 'Nota - Unidade 3', type: 'number', required: true, step: '0.1' },
            { name: 'frequencia', label: 'Percentual de Frequência (0 a 100)', type: 'number', required: true }
        ],
        buildPayload: (data, id) => {
            if (!id) {
                data.alunoId = Number(state.activeAlunoFilter);
            }
            delete data.alunoNomeDisplay;
            return data;
        }
    }
};