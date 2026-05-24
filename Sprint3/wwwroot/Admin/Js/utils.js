import { state } from './state.js';

export function exibirAlerta(msg, tipo) {
    state.el.alert.textContent = msg;
    state.el.alert.className = `alert alert-${tipo} mb-4`;
    state.el.alert.classList.remove('d-none');
    setTimeout(() => state.el.alert.classList.add('d-none'), 5000);
}

export function exibirErroFormulario(msg) {
    let errorDiv = document.getElementById('formErrorFeedback');
    if (!errorDiv) {
        errorDiv = document.createElement('div');
        errorDiv.id = 'formErrorFeedback';
        state.el.fieldsContainer.parentNode.insertBefore(errorDiv, state.el.fieldsContainer);
    }
    errorDiv.className = 'alert alert-danger mb-3';
    errorDiv.innerHTML = `<i class="fas fa-exclamation-triangle me-2"></i> ${msg}`;
    errorDiv.classList.remove('d-none');
    setTimeout(() => { if (errorDiv) errorDiv.classList.add('d-none'); }, 6000);
}

export function setupMasks() {
    document.addEventListener('input', (e) => {
        const camposSemNumeros = ['nome', 'nomeResponsavel', 'nomeProfessor', 'alunoNomeDisplay', 'especialidade'];

        if (camposSemNumeros.includes(e.target.name)) {
            e.target.value = e.target.value.replace(/\d/g, '');
        }

        if (e.target.name === 'loginCpf' || e.target.name === 'cpfResponsavel') {
            let value = e.target.value.replace(/\D/g, '');

            if (value.length > 11) value = value.slice(0, 11);

            if (value.length > 9) {
                value = value.replace(/(\d{3})(\d{3})(\d{3})(\d{1,2})/, "$1.$2.$3-$4");
            } else if (value.length > 6) {
                value = value.replace(/(\d{3})(\d{3})(\d{1,3})/, "$1.$2.$3");
            } else if (value.length > 3) {
                value = value.replace(/(\d{3})(\d{1,3})/, "$1.$2");
            }

            e.target.value = value;
        }
        if (e.target.name === 'telefoneResponsavel' || e.target.name === 'telefone') {
            let value = e.target.value.replace(/\D/g, '');

            if (value.length > 11) value = value.slice(0, 11);

            if (value.length > 6) {
                value = value.replace(/^(\d{2})(\d{5})(\d{1,4}).*/, "($1) $2-$3");
            } else if (value.length > 2) {
                value = value.replace(/^(\d{2})(\d{1,5}).*/, "($1) $2");
            } else if (value.length > 0) {
                value = value.replace(/^(\d{1,2}).*/, "($1");
            }

            e.target.value = value;
        }
    });
}