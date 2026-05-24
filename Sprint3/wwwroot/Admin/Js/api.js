export async function apiFetch(url, options = {}) {
    const token = localStorage.getItem('token');

    if (!token && !url.includes('login')) {
        window.location.href = '/index.html';
        return;
    }

    const response = await fetch(url, {
        ...options,
        headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${token}`,
            ...options.headers
        }
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