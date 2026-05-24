// js/main.js
import { state, initElements } from './state.js';
import { setupMasks } from './utils.js';
import { loadData, setupTableEvents } from './table.js';
import { setupFormEvents } from './form.js';

document.addEventListener('DOMContentLoaded', () => {
    const token = localStorage.getItem('token');
    if (!token) {
        window.location.href = '/index.html';
        return;
    }

    initElements();
    setupMasks();
    setupTableEvents();
    setupFormEvents();

    document.querySelectorAll('.nav-link[data-entity]').forEach(link => {
        link.addEventListener('click', (e) => {
            e.preventDefault();

            document.querySelectorAll('.nav-link').forEach(n => n.classList.remove('active'));
            e.currentTarget.classList.add('active');

            state.currentEntity = e.currentTarget.getAttribute('data-entity');

            state.activeAlunoFilter = null;
            state.activeAlunoNome = '';
            state.activeAlunoTurmaId = null;
            state.activeTurmaFilter = null;
            state.activeTurmaNome = '';

            loadData();
        });
    });

    const btnLogout = document.getElementById('btnLogout');
    if (btnLogout) {
        btnLogout.addEventListener('click', () => {
            localStorage.removeItem('token');
            window.location.href = '/index.html';
        });
    }

    loadData();
});