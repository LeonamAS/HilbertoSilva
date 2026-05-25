const token = localStorage.getItem("token") || localStorage.getItem("meuToken");
const API_BASE_URL = 'https://localhost:7151';

let dadosBoletimParaImpressao = null;

if (!token) {
    alert("Acesso negado. Faça login primeiro.");
    window.location.href = "index.html";
}

document.getElementById('btnSair').addEventListener('click', () => {
    localStorage.removeItem("token");
    localStorage.removeItem("meuToken");
    window.location.href = API_BASE_URL;
});

async function carregarMeuBoletim() {
    const divConteudo = document.getElementById('conteudoBoletim');

    try {
        const response = await fetch(`${API_BASE_URL}/api/boletim/meu-boletim`, {
            method: 'GET',
            headers: {
                'Authorization': `Bearer ${token}`,
                'Content-Type': 'application/json'
            }
        });

        if (response.ok) {
            const boletim = await response.json();

            if (!boletim || (Array.isArray(boletim) && boletim.length === 0)) {
                document.getElementById('nomeUsuario').textContent = `Bem-vindo(a)!`;
                divConteudo.innerHTML = `<div class="alert alert-info mt-3"><i class="fas fa-info-circle me-2"></i>Nenhuma disciplina ou nota registrada para você ainda.</div>`;
                return;
            }

            const dadosAluno = Array.isArray(boletim) ? boletim[0] : boletim;
            const disciplinas = Array.isArray(boletim) ? boletim : boletim.disciplinas;

            dadosBoletimParaImpressao = {
                aluno: dadosAluno,
                disciplinas: disciplinas
            };

            const primeiroNome = (dadosAluno.nomeAluno || dadosAluno.nome).split(' ')[0];
            document.getElementById('nomeUsuario').textContent = `Bem-vindo(a), ${primeiroNome}!`;

            let htmlFinal = `
                <div class="d-flex justify-content-between align-items-center mb-3">
                    <h5 class="m-0 text-muted"><i class="fas fa-file-alt me-2"></i>Resumo Acadêmico</h5>
                    <button class="btn btn-outline-success btn-sm" onclick="imprimirMeuBoletim()">
                        <i class="fas fa-print me-2"></i> Imprimir Boletim
                    </button>
                </div>
                <div class="row mb-4 bg-light p-3 rounded mx-0">
                    <div class="col-md-6">
                        <p class="mb-1"><strong>Aluno(a):</strong> ${dadosAluno.nomeAluno || dadosAluno.nome}</p>
                        <p class="mb-0"><strong>Matrícula:</strong> ${dadosAluno.matricula}</p>
                    </div>
                    <div class="col-md-6 text-md-end mt-2 mt-md-0">
                        <p class="mb-1"><strong>Turma:</strong> ${dadosAluno.nomeTurma}</p>
                        <p class="mb-0"><strong>Ano Letivo:</strong> ${dadosAluno.anoLetivo || new Date().getFullYear()}</p>
                    </div>
                </div>
            `;

            if (disciplinas && disciplinas.length > 0) {
                htmlFinal += `
                <div class="table-responsive">
                    <table class="table table-hover table-custom mt-2 border">
                        <thead>
                            <tr>
                                <th>Disciplina</th>
                                <th class="text-center">Unidade 1</th>
                                <th class="text-center">Unidade 2</th>
                                <th class="text-center">Unidade 3</th>
                                <th class="text-center">Média Final</th>
                                <th class="text-center">Frequência</th>
                            </tr>
                        </thead>
                        <tbody>
                `;

                disciplinas.forEach(d => {
                    const formatarNota = (nota) => {
                        if (nota === null || nota === undefined) return '<span class="text-muted">-</span>';
                        const valor = Number(nota);
                        const cor = valor < 6 ? 'text-danger fw-bold' : '';
                        return `<span class="${cor}">${valor.toFixed(1)}</span>`;
                    };

                    const freqFormatada = (d.frequencia === null || d.frequencia === undefined)
                        ? '<span class="text-muted">-</span>'
                        : `<span class="${Number(d.frequencia) < 70 ? 'text-danger fw-bold' : ''}">${Number(d.frequencia).toFixed(0)}%</span>`;

                    htmlFinal += `
                        <tr>
                            <td class="fw-medium" data-label="Disciplina">${d.nomeDisciplina}</td>
                            <td class="text-center" data-label="Unidade 1">${formatarNota(d.notaU1)}</td>
                            <td class="text-center" data-label="Unidade 2">${formatarNota(d.notaU2)}</td>
                            <td class="text-center" data-label="Unidade 3">${formatarNota(d.notaU3)}</td>
                            <td class="text-center bg-light fw-bold" data-label="Média Final">${formatarNota(d.mediaFinal)}</td>
                            <td class="text-center" data-label="Frequência">${freqFormatada}</td>
                        </tr>
                    `;
                });

                htmlFinal += `
                        </tbody>
                    </table>
                </div>
                <div class="text-muted mt-3 small">
                    <i class="fas fa-info-circle me-1"></i> Média para aprovação: 6.0. Frequência mínima: 70%.
                </div>`;
            } else {
                htmlFinal += `<div class="alert alert-info mt-3"><i class="fas fa-info-circle me-2"></i>Nenhuma disciplina ou nota registrada para você ainda.</div>`;
            }

            divConteudo.innerHTML = htmlFinal;

        } else if (response.status === 404) {
            divConteudo.innerHTML = '<div class="alert alert-warning mt-3"><i class="fas fa-exclamation-triangle me-2"></i>Seu cadastro não foi encontrado. Contate a secretaria.</div>';
        } else if (response.status === 401 || response.status === 403) {
            alert("Sua sessão expirou ou você não tem permissão.");
            localStorage.removeItem("token");
            window.location.href = "index.html";
        } else {
            divConteudo.innerHTML = '<div class="alert alert-danger mt-3"><i class="fas fa-times-circle me-2"></i>Erro ao buscar os dados do seu boletim.</div>';
        }
    } catch (error) {
        console.error("Erro na API:", error);
        divConteudo.innerHTML = '<div class="alert alert-danger mt-3"><i class="fas fa-wifi me-2"></i>Erro de conexão com o servidor. Tente novamente mais tarde.</div>';
    }
}

document.addEventListener('DOMContentLoaded', () => {
    const formAlterarSenha = document.getElementById('formAlterarSenha');

    if (formAlterarSenha) {
        formAlterarSenha.addEventListener('submit', async (e) => {
            e.preventDefault();

            limparErros();
            let temErro = false;

            const senhaAtual = document.getElementById('senhaAtual').value;
            const novaSenha = document.getElementById('novaSenha').value;
            const confirmarSenha = document.getElementById('confirmarSenha').value;
            const alertSenha = document.getElementById('alertSenha');

            const btnSalvar = document.getElementById('btnSalvarNovaSenha') || document.getElementById('btnSalvarSenha');

            if (!senhaAtual) {
                mostrarErro('senhaAtual', 'erroSenhaAtual', 'A senha atual é obrigatória.');
                temErro = true;
            }

            if (!novaSenha) {
                mostrarErro('novaSenha', 'erroNovaSenha', 'A nova senha é obrigatória.');
                temErro = true;
            } else if (!validarSenhaForte(novaSenha)) {
                mostrarErro('novaSenha', 'erroNovaSenha', 'A senha não cumpre as regras exigidas.');
                temErro = true;
            } else if (novaSenha === senhaAtual) {
                mostrarErro('novaSenha', 'erroNovaSenha', 'A nova senha deve ser diferente da atual.');
                temErro = true;
            }

            if (!confirmarSenha) {
                mostrarErro('confirmarSenha', 'erroConfirmarSenha', 'Confirme a nova senha.');
                temErro = true;
            } else if (novaSenha !== confirmarSenha) {
                mostrarErro('confirmarSenha', 'erroConfirmarSenha', 'As novas senhas não coincidem!');
                temErro = true;
            }

            if (temErro) return;

            try {
                if (btnSalvar) {
                    btnSalvar.disabled = true;
                    btnSalvar.innerHTML = '<span class="spinner-border spinner-border-sm me-2"></span>Salvando...';
                }

                const response = await fetch(`${API_BASE_URL}/api/auth/alterar-senha`, {
                    method: 'PUT',
                    headers: {
                        'Content-Type': 'application/json',
                        'Authorization': `Bearer ${token}`
                    },
                    body: JSON.stringify({
                        senhaAtual: senhaAtual,
                        novaSenha: novaSenha
                    })
                });

                if (response.status === 204) {
                    if (alertSenha) mostrarAlerta(alertSenha, 'Senha alterada com sucesso!', 'success');
                    formAlterarSenha.reset();

                    setTimeout(() => {
                        const modalElement = document.getElementById('modalAlterarSenha') || document.getElementById('formModal');
                        const modalInstance = bootstrap.Modal.getInstance(modalElement);
                        if (modalInstance) modalInstance.hide();
                        if (alertSenha) alertSenha.classList.add('d-none');
                    }, 2000);

                } else {
                    const data = await response.json();

                    if (data.mensagem && data.mensagem.toLowerCase().includes('atual')) {
                        mostrarErro('senhaAtual', 'erroSenhaAtual', data.mensagem);
                    } else if (alertSenha) {
                        mostrarAlerta(alertSenha, data.mensagem || 'Erro ao alterar a senha.', 'danger');
                    } else {
                        mostrarErro('novaSenha', 'erroNovaSenha', data.mensagem || 'Erro ao processar.');
                    }
                }

            } catch (error) {
                console.error('Erro na requisição:', error);
                if (alertSenha) mostrarAlerta(alertSenha, 'Erro de conexão com o servidor.', 'danger');
            } finally {
                if (btnSalvar) {
                    btnSalvar.disabled = false;
                    btnSalvar.innerText = 'Salvar Nova Senha';
                }
            }
        });
    }

    function mostrarAlerta(elemento, mensagem, tipo) {
        elemento.className = `alert alert-${tipo} small p-2 text-center mt-2`;
        elemento.textContent = mensagem;
        elemento.classList.remove('d-none');
    }
});

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

function limparErros() {
    document.querySelectorAll('.text-danger.small').forEach(el => {
        el.classList.add('d-none');
        el.textContent = '';
    });
    document.querySelectorAll('.is-invalid').forEach(el => {
        el.classList.remove('is-invalid');
    });
}

function mostrarErro(inputId, erroId, mensagem) {
    const input = document.getElementById(inputId);
    const erroEl = document.getElementById(erroId);

    if (input) input.classList.add('is-invalid');
    if (erroEl) {
        erroEl.textContent = mensagem;
        erroEl.classList.remove('d-none');
    }
}

window.imprimirMeuBoletim = function () {
    if (!dadosBoletimParaImpressao || !dadosBoletimParaImpressao.disciplinas || dadosBoletimParaImpressao.disciplinas.length === 0) {
        alert('Não há dados para imprimir neste boletim.');
        return;
    }

    const info = dadosBoletimParaImpressao.aluno;
    const disciplinas = dadosBoletimParaImpressao.disciplinas;
    const dataAtual = new Date().toLocaleDateString('pt-BR');

    const linhasTabela = disciplinas.map(d => {
        const corU1 = Number(d.notaU1) < 6 && d.notaU1 !== null ? 'text-danger' : '';
        const corU2 = Number(d.notaU2) < 6 && d.notaU2 !== null ? 'text-danger' : '';
        const corU3 = Number(d.notaU3) < 6 && d.notaU3 !== null ? 'text-danger' : '';
        const corMedia = Number(d.mediaFinal) < 6 && d.mediaFinal !== null ? 'text-danger fw-bold' : '';
        const corFreq = Number(d.frequencia) < 70 && d.frequencia !== null ? 'text-danger fw-bold' : '';

        const formatarNotaImp = (n) => n === null || n === undefined ? '-' : Number(n).toFixed(1);
        const formatarFreqImp = (f) => f === null || f === undefined ? '-' : `${Number(f).toFixed(0)}%`;

        return `
            <tr>
                <td>${d.nomeDisciplina}</td>
                <td class="text-center ${corU1}">${formatarNotaImp(d.notaU1)}</td>
                <td class="text-center ${corU2}">${formatarNotaImp(d.notaU2)}</td>
                <td class="text-center ${corU3}">${formatarNotaImp(d.notaU3)}</td>
                <td class="text-center fw-bold bg-light ${corMedia}">${formatarNotaImp(d.mediaFinal)}</td>
                <td class="text-center ${corFreq}">${formatarFreqImp(d.frequencia)}</td>
            </tr>
        `;
    }).join('');

    const htmlImpressao = `
        <!DOCTYPE html>
        <html lang="pt-BR">
        <head>
            <meta charset="UTF-8">
            <title>Boletim - ${info.nomeAluno || info.nome}</title>
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
                .bg-light { background-color: #f8f9fa !important; }
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
                <p><strong>Aluno(a):</strong> ${info.nomeAluno || info.nome}</p>
                <p><strong>Matrícula:</strong> ${info.matricula || 'N/A'}</p>
                <p><strong>Ano Escolar:</strong> ${info.anoEscolar || 'N/A'}</p>
                <p><strong>Turma:</strong> ${info.nomeTurma || 'N/A'}</p>
                <p><strong>Ano Letivo:</strong> ${info.anoLetivo || new Date().getFullYear()}</p>
            </div>

            <table>
                <thead>
                    <tr>
                        <th>Disciplina</th>
                        <th class="text-center">Unidade 1</th>
                        <th class="text-center">Unidade 2</th>
                        <th class="text-center">Unidade 3</th>
                        <th class="text-center">Média Final</th>
                        <th class="text-center">Frequência</th>
                    </tr>
                </thead>
                <tbody>
                    ${linhasTabela}
                </tbody>
            </table>

            <div class="footer">
                <p>Este documento é um resumo de notas e frequência. Em caso de divergência, procure a secretaria da escola.</p>
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
};
carregarMeuBoletim();