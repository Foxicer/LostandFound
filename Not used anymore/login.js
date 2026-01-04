document.addEventListener('DOMContentLoaded', () => {
  const form = document.getElementById('loginForm');
  const emailEl = document.getElementById('email');
  const passEl = document.getElementById('password');
  const errEl = document.getElementById('loginError') || document.createElement('div');

  function showError(msg){ errEl.textContent = msg; }

  form.addEventListener('submit', async (e) => {
    e.preventDefault();
    const email = (emailEl.value || '').trim().toLowerCase();
    const password = passEl.value || '';
    if (!email || !password) { showError('Enter email and password'); return; }

    // 1) check localStorage users
    try {
      const localUsers = JSON.parse(localStorage.getItem('users') || '[]');
      const foundLocal = (localUsers || []).find(u => (u.email||'').toLowerCase() === email && (u.password||'') === password);
      if (foundLocal) {
        const store = { name: foundLocal.name || foundLocal.username || email, email: foundLocal.email, gradeSection: foundLocal.grade_section || '' };
        localStorage.setItem('currentUser', JSON.stringify(store));
        localStorage.setItem('currentUserEmail', store.email || '');
        window.location.href = 'dashboard.html';
        return;
      }
    } catch (ex) { /* ignore parse errors */ }

    // 2) fallback: fetch users.json
    try {
      const res = await fetch('users.json', { cache: 'no-store' });
      if (!res.ok) { showError('Could not load users.json'); return; }
      const users = await res.json();
      const found = (users || []).find(u => (u.email||'').toLowerCase() === email && (u.password||'') === password);
      if (!found) { showError('Invalid email or password'); return; }
      const store = { name: found.name || found.username || email, email: found.email, gradeSection: found.grade_section || '' };
      localStorage.setItem('currentUser', JSON.stringify(store));
      localStorage.setItem('currentUserEmail', store.email || '');
      window.location.href = 'dashboard.html';
    } catch (ex) {
      showError('Login failed: ' + (ex.message || ex));
    }
  });
});

// Use users.json in project root (LostandFound/users.json)
const usersJsonPath = 'users.json';