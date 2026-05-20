document.addEventListener('DOMContentLoaded', () => {
    const authForm = document.getElementById('authForm');
    const inputCpf = document.getElementById('inputCpf');
    const inputSenha = document.getElementById('inputSenha');
    const alertBox = document.getElementById('alertBox');
    const btnAction = document.getElementById('btnAction');

    const API_BASE_URL = '/api/auth';

    inputCpf.addEventListener('input', (e) => {
        let value = e.target.value;

        if (/[a-zA-Z]/.test(value)) {
            e.target.value = value.substring(0, 14);
            return;
        }

        value = value.replace(/\D/g, '');
        if (value.length > 11) value = value.slice(0, 11);

        value = value.replace(/(\d{3})(\d)/, '$1.$2');
        value = value.replace(/(\d{3})(\d)/, '$1.$2');
        value = value.replace(/(\d{3})(\d{1,2})$/, '$1-$2');

        e.target.value = value;
    });

    authForm.addEventListener('submit', async (e) => {
        e.preventDefault();
        ocultarErro();

        let credencial = inputCpf.value.trim();
        const senha = inputSenha.value;

        if (credencial.toLowerCase() !== 'admin') {
            credencial = credencial.replace(/\D/g, '');
        } else {
            credencial = "Admin";
        }

        if (!credencial || !senha) {
            exibirErro('Por favor, preencha o CPF (ou usuário) e a Senha.');
            return;
        }

        try {
            setLoading(true);

            const response = await fetch(`${API_BASE_URL}/login`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({ cpf: credencial, senha: senha })
            });

            let data = {};
            const contentType = response.headers.get("content-type");
            if (contentType && contentType.indexOf("application/json") !== -1) {
                data = await response.json();
            }

            if (!response.ok) {
                throw new Error(data.mensagem || 'Ocorreu um erro ao processar o login.');
            }

            localStorage.setItem('token', data.token);
            window.location.href = 'Admin/dashboard.html';

        } catch (error) {
            exibirErro(error.message);
        } finally {
            setLoading(false);
        }
    });

    // --- Funções Utilitárias ---
    function exibirErro(mensagem) {
        alertBox.textContent = mensagem || 'Erro interno no servidor.';
        alertBox.classList.remove('d-none');
    }

    function ocultarErro() {
        alertBox.classList.add('d-none');
        alertBox.textContent = '';
    }

    function setLoading(isLoading) {
        if (isLoading) {
            btnAction.disabled = true;
            btnAction.dataset.originalText = btnAction.textContent;
            btnAction.innerHTML = `<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Processando...`;
        } else {
            btnAction.disabled = false;
            btnAction.textContent = btnAction.dataset.originalText || 'FAZER LOGIN';
        }
    }
});