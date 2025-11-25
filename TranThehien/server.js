const express = require('express');
const bodyParser = require('body-parser');
const cors = require('cors');
const { Low, JSONFile } = require('lowdb');
const path = require('path');

const app = express();
app.use(cors());
app.use(bodyParser.json());

// DB (lowdb)
const file = path.join(__dirname, 'users.json');
const adapter = new JSONFile(file);
const db = new Low(adapter);

async function initDB() {
  await db.read();
  db.data = db.data || { users: [] };
  await db.write();
}
initDB();

// Serve static frontend
app.use('/', express.static(path.join(__dirname, 'public')));

// API: get users (with simple search & pagination)
app.get('/api/users', async (req, res) => {
  await db.read();
  let users = db.data.users || [];

  const q = (req.query.q || '').toLowerCase();
  if (q) {
    users = users.filter(u => (u.name + u.email).toLowerCase().includes(q));
  }

  // pagination
  const page = parseInt(req.query.page || '1');
  const perPage = parseInt(req.query.perPage || '10');
  const total = users.length;
  const start = (page - 1) * perPage;
  const paged = users.slice(start, start + perPage);

  res.json({ data: paged, total });
});

// API: add user
app.post('/api/users', async (req, res) => {
  await db.read();
  const { name, email, role = 'User', status = 'Hoạt động' } = req.body;
  const id = Date.now().toString();
  const user = { id, name, email, role, status, createdAt: new Date().toISOString().slice(0,10) };
  db.data.users.unshift(user);
  await db.write();
  res.json({ success: true, user });
});

// API: update user
app.put('/api/users/:id', async (req, res) => {
  await db.read();
  const id = req.params.id;
  const idx = db.data.users.findIndex(u => u.id === id);
  if (idx === -1) return res.status(404).json({ success:false, message: 'Not found' });
  db.data.users[idx] = { ...db.data.users[idx], ...req.body };
  await db.write();
  res.json({ success:true, user: db.data.users[idx] });
});

// API: delete user
app.delete('/api/users/:id', async (req, res) => {
  await db.read();
  const id = req.params.id;
  db.data.users = db.data.users.filter(u => u.id !== id);
  await db.write();
  res.json({ success: true });
});

const PORT = process.env.PORT || 3000;
app.listen(PORT, () => console.log(`Server running: http://localhost:${PORT}`));
