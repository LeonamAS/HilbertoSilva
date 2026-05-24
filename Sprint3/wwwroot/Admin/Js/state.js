export const state = {
    currentEntity: 'alunos',
    activeAlunoFilter: null,
    activeAlunoNome: '',
    activeAlunoTurmaId: null,
    activeTurmaFilter: null,
    activeTurmaNome: '',
    itemToDeleteId: null,
    currentTableData: [],
    el: {}
};

export function initElements() {
    state.el = {
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
}