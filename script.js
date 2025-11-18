let locations = [
  {
    id: 1,
    name: "Cầu Vàng",
    desc: "Cây cầu nổi tiếng với thiết kế bàn tay khổng lồ ôm lấy dãy núi, điểm check-in đình đám của Đà Nẵng.",
    status: "Hoạt động",
  },
  {
    id: 2,
    name: "Bà Nà Hills",
    desc: "Khu du lịch trên đỉnh núi với khí hậu 4 mùa, kiến trúc Pháp cổ kính và nhiều khu vui chơi, ẩm thực phong phú.",
    status: "Hoạt động",
  },
  {
    id: 3,
    name: "Bãi biển Mỹ Khê",
    desc: "Một trong những bãi biển quyến rũ nhất hành tinh với cát trắng mịn, sóng nhẹ và dịch vụ du lịch hoàn chỉnh.",
    status: "Tạm ẩn",
  },
  {
    id: 4,
    name: "Ngũ Hành Sơn",
    desc: "Quần thể 5 ngọn núi đá vôi và cẩm thạch chứa nhiều hang động và chùa linh thiêng, thu hút du khách.",
    status: "Chờ duyệt",
  },
  // Thêm đủ dữ liệu trang 2,3,4 nếu muốn
  { id: 5, name: "Chợ Hàn", desc: "Nơi đặc sản và quà lưu niệm.", status: "Hoạt động" },
  { id: 6, name: "Sông Hàn", desc: "Dòng sông thơ mộng của Đà Nẵng.", status: "Hoạt động" },
  { id: 7, name: "Asia Park", desc: "Công viên giải trí đa dạng.", status: "Tạm ẩn" },
  { id: 8, name: "Bảo tàng Chăm", desc: "Bảo tàng văn hóa lịch sử.", status: "Chờ duyệt" },
  { id: 9, name: "Cầu Rồng", desc: "Cây cầu biểu tượng hình rồng.", status: "Hoạt động" },
  { id: 10, name: "Ngã ba Huế", desc: "Khu mua sắm nổi tiếng của giới trẻ.", status: "Hoạt động" },
];

const locationTbody = document.getElementById("locationTbody");
const modal = document.getElementById("modal");
const confirmDeleteModal = document.getElementById("confirmDeleteModal");
const descModal = document.getElementById("descModal");
const descModalContent = document.getElementById("descModalContent");
const modalTitle = document.getElementById("modalTitle");
const locationForm = document.getElementById("locationForm");
const addNewBtn = document.getElementById("addNewBtn");
const closeModalBtn = document.getElementById("closeModal");
const cancelBtn = document.getElementById("cancelBtn");
const searchInput = document.getElementById("searchInput");
const filterBtn = document.getElementById("filterBtn");
const confirmDeleteBtn = document.getElementById("confirmDeleteBtn");
const cancelDeleteBtn = document.getElementById("cancelDeleteBtn");
const closeDescModalBtn = document.getElementById("closeDescModal");
const paginationEl = document.querySelector(".pagination");
const selectAllCheckbox = document.getElementById("selectAll");

let editId = null;
let deleteId = null;
const pageSize = 4;
let currentPage = 1;

function statusClass(status) {
  switch (status) {
    case "Hoạt động":
      return "active";
    case "Tạm ẩn":
      return "hidden";
    case "Chờ duyệt":
      return "pending";
    default:
      return "";
  }
}
function statusText(status) {
  return status;
}

function renderLocations(filter = "", page = 1) {
  let filtered = locations.filter((l) =>
    l.name.toLowerCase().includes(filter.toLowerCase())
  );
  const totalPages = Math.ceil(filtered.length / pageSize);
  currentPage = Math.min(Math.max(1, page), totalPages || 1);

  const start = (currentPage - 1) * pageSize;
  const end = start + pageSize;
  const pageList = filtered.slice(start, end);

  locationTbody.innerHTML = "";
  if (pageList.length === 0) {
    locationTbody.innerHTML =
      '<tr><td colspan="5" style="text-align:center; padding: 1rem;">Không tìm thấy địa điểm nào.</td></tr>';
    renderPagination(totalPages);
    return;
  }

  for (let loc of pageList) {
    const tr = document.createElement("tr");
    tr.innerHTML = `
      <td style="text-align:center;"><input type="checkbox" /></td>
      <td>${loc.name}</td>
      <td class="desc-cell" title="${loc.desc}">${loc.desc.length > 35 ? loc.desc.slice(0, 35) + "..." : loc.desc}</td>
      <td><span class="status ${statusClass(loc.status)}">${statusText(loc.status)}</span></td>
      <td class="action-buttons">
        <span class="edit" data-id="${loc.id}">edit</span>
        <span class="delete" data-id="${loc.id}">delete</span>
      </td>
    `;
    locationTbody.appendChild(tr);
  }

  // Bấm vào ô mô tả mở modal mô tả đầy đủ
  document.querySelectorAll(".desc-cell").forEach((td) => {
    td.style.cursor = "pointer";
    td.onclick = () => {
      descModalContent.textContent = td.getAttribute("title");
      showModal(descModal);
    };
  });

  // Gắn sự kiện edit và delete
  document.querySelectorAll(".edit").forEach((btn) => {
    btn.onclick = (e) => {
      openEditModal(Number(e.target.dataset.id));
    };
  });
  document.querySelectorAll(".delete").forEach((btn) => {
    btn.onclick = (e) => {
      openDeleteModal(Number(e.target.dataset.id));
    };
  });

  renderPagination(totalPages);
  updateSelectAllCheckbox();
}

function renderPagination(totalPages) {
  paginationEl.innerHTML = "";
  if (totalPages <= 1) return;

  const createBtn = (label, page, active = false) => {
    let btn = document.createElement("button");
    btn.textContent = label;
    btn.className = "page-btn";
    if (active) btn.classList.add("active");
    btn.onclick = () => {
      if (page !== currentPage) renderLocations(searchInput.value, page);
    };
    return btn;
  };

  if (currentPage > 1) paginationEl.appendChild(createBtn("«", currentPage -1));

  paginationEl.appendChild(createBtn(1, 1, currentPage === 1));

  if (totalPages > 6 && currentPage > 4) {
    const ellipsis = document.createElement("span");
    ellipsis.textContent = "...";
    ellipsis.className = "page-ellipsis";
    paginationEl.appendChild(ellipsis);
  }

  let from = Math.max(2, currentPage -1);
  let to = Math.min(totalPages -1, currentPage +1);
  for (let i = from; i <= to; i++) {
    paginationEl.appendChild(createBtn(i, i, currentPage === i));
  }

  if (totalPages > 6 && currentPage < totalPages -3) {
    const ellipsis = document.createElement("span");
    ellipsis.textContent = "...";
    ellipsis.className = "page-ellipsis";
    paginationEl.appendChild(ellipsis);
  }

  if (totalPages > 1) paginationEl.appendChild(createBtn(totalPages, totalPages, currentPage === totalPages));

  if (currentPage < totalPages) paginationEl.appendChild(createBtn("»", currentPage +1));
}

function openEditModal(id) {
  const loc = locations.find((l) => l.id === id);
  if (!loc) return;
  editId = id;
  modalTitle.textContent = "Sửa địa điểm";
  locationForm.locationName.value = loc.name;
  locationForm.locationDesc.value = loc.desc;
  locationForm.locationStatus.value = loc.status;
  showModal(modal);
}

function openAddModal() {
  editId = null;
  modalTitle.textContent = "Thêm địa điểm mới";
  locationForm.reset();
  showModal(modal);
}
function showModal(modalEl) {
  modalEl.classList.remove("hidden");
}
function hideModal(modalEl) {
  modalEl.classList.add("hidden");
}
function openDeleteModal(id) {
  deleteId = id;
  showModal(confirmDeleteModal);
}
locationForm.addEventListener("submit", (e) => {
  e.preventDefault();
  const name = locationForm.locationName.value.trim();
  const desc = locationForm.locationDesc.value.trim();
  const status = locationForm.locationStatus.value;
  if (editId) {
    const idx = locations.findIndex((l) => l.id === editId);
    if (idx > -1) {
      locations[idx] = { id: editId, name, desc, status };
    }
  } else {
    const newId = locations.length ? locations[locations.length - 1].id + 1 : 1;
    locations.push({ id: newId, name, desc, status });
  }
  renderLocations(searchInput.value, currentPage);
  hideModal(modal);
});
closeModalBtn.onclick = () => hideModal(modal);
cancelBtn.onclick = () => hideModal(modal);
confirmDeleteBtn.onclick = () => {
  if (deleteId !== null) {
    locations = locations.filter((l) => l.id !== deleteId);
    renderLocations(searchInput.value, currentPage);
    deleteId = null;
    hideModal(confirmDeleteModal);
  }
};
cancelDeleteBtn.onclick = () => {
  deleteId = null;
  hideModal(confirmDeleteModal);
};
addNewBtn.onclick = () => openAddModal();
filterBtn.onclick = () => renderLocations(searchInput.value, 1);
searchInput.addEventListener("keyup", (e) => {
  if (e.key === "Enter") renderLocations(searchInput.value, 1);
});

selectAllCheckbox.addEventListener("change", (e) => {
  const checked = e.target.checked;
  locationTbody.querySelectorAll("input[type='checkbox']").forEach((cb) => (cb.checked = checked));
});
function updateSelectAllCheckbox() {
  const checkboxes = locationTbody.querySelectorAll("input[type='checkbox']");
  if (checkboxes.length === 0) {
    selectAllCheckbox.checked = false;
    selectAllCheckbox.indeterminate = false;
    return;
  }
  const allChecked = Array.from(checkboxes).every((cb) => cb.checked);
  const someChecked = Array.from(checkboxes).some((cb) => cb.checked);

  selectAllCheckbox.checked = allChecked;
  selectAllCheckbox.indeterminate = !allChecked && someChecked;
}

// Đóng modal mô tả chi tiết
document.getElementById("closeDescModal").onclick = () => hideModal(descModal);

document.addEventListener("DOMContentLoaded", () => {
  renderLocations();
});