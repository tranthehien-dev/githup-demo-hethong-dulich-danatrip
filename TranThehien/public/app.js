const apiRoot = '/api/users';
let page = 1, perPage = 10, total = 0;
const tbody = document.querySelector('#userTable tbody');
const pageInfo = document.querySelector('#pageInfo');
const searchInput = document.getElementById('search');

async function load() {
  const q = encodeURIComponent(searchInput.value || '');
  const res = await fetch(`${apiRoot}?q=${q}&page=${page}&perPage=${perPage}`);
  const obj = await res.json();
  total = obj.total;
  renderTable(obj.data);
  pageInfo.textContent = `Trang ${page} — tổng ${total}`;
}

function renderTable(users) {
  tbody.innerHTML = '';
  users.forEach(u => {
    const tr = document.createElement('tr');
    tr.innerHTML = `
      <td><input type="checkbox" data-id="${u.id}"></td>
      <td>${u.name}</td>
      <td>${u.email}</td>
      <td><span class="${u.role==='Admin'?'role-admin':'role-user'}">${u.role}</span></td>
      <td><span class="${u.status==='Hoạt động'?'status-active':'status-locked'}">${u.status}</span></td>
      <td>${u.createdAt}</td>
      <td>
        <button data-action="edit" data-id="${u.id}">edit</button>
        <button data-action="del" data-id="${u.id}">delete</button>
      </td>
    `;
    tbody.appendChild(tr);
  });
}

document.getElementById('next').addEventListener('click', ()=>{ page++; load(); });
document.getElementById('prev').addEventListener('click', ()=>{ if(page>1){ page--; load(); } });
searchInput.addEventListener('input', ()=>{ page=1; load(); });

/* modal logic */
const modal = document.getElementById('modal');
const f_name = document.getElementById('f_name');
const f_email = document.getElementById('f_email');
const f_role = document.getElementById('f_role');
const f_status = document.getElementById('f_status');
const modalTitle = document.getElementById('modalTitle');
let editId = null;

document.getElementById('addBtn').addEventListener('click', ()=>{
  editId = null;
  modalTitle.textContent = 'Thêm Người Dùng';
  f_name.value=''; f_email.value=''; f_role.value='User'; f_status.value='Hoạt động';
  modal.classList.remove('hidden');
});
document.getElementById('cancelBtn').addEventListener('click', ()=> modal.classList.add('hidden'));

document.getElementById('saveBtn').addEventListener('click', async ()=>{
  const payload = { name: f_name.value, email: f_email.value, role: f_role.value, status: f_status.value };
  if(!payload.name || !payload.email){ alert('Nhập tên và email'); return; }
  if(editId){
    await fetch(`${apiRoot}/${editId}`, { method:'PUT', headers:{'Content-Type':'application/json'}, body: JSON.stringify(payload) });
  } else {
    await fetch(apiRoot, { method:'POST', headers:{'Content-Type':'application/json'}, body: JSON.stringify(payload) });
  }
  modal.classList.add('hidden');
  load();
});

/* table actions */
tbody.addEventListener('click', async (e)=>{
  const btn = e.target;
  const id = btn.dataset.id;
  const action = btn.dataset.action;
  if(action === 'del') {
    if(!confirm('Xóa user này?')) return;
    await fetch(`${apiRoot}/${id}`, { method: 'DELETE' });
    load();
  } else if(action === 'edit') {
    const res = await fetch(`${apiRoot}?q=&page=1&perPage=1000`);
    const data = await res.json();
    const user = data.data.find(u=>u.id===id);
    if(!user) return alert('Không tìm thấy user');
    editId = id;
    modalTitle.textContent = 'Sửa Người Dùng';
    f_name.value = user.name;
    f_email.value = user.email;
    f_role.value = user.role;
    f_status.value = user.status;
    modal.classList.remove('hidden');
  }
});

/* init */
load();
