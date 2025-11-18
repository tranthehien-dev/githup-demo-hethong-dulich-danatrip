// Dữ liệu mẫu
const tours = [
  {
    id: 1,
    name: "Khám phá Bà Nà Hills",
    desc: "Trải nghiệm Cầu Vàng và Làng Pháp thu nhỏ",
    price: 1500000,
    duration: "1 ngày",
    status: "Hoạt động",
    type: "1 ngày",
  },
  {
    id: 2,
    name: "Tour Cù Lao Chàm",
    desc: "Lặn ngắm san hô và thưởng thức hải sản tươi sống",
    price: 850000,
    duration: "1 ngày",
    status: "Hoạt động",
    type: "1 ngày",
  },
  {
    id: 3,
    name: "Ngũ Hành Sơn - Hội An",
    desc: "Tham quan làng đá Non Nước và phố cổ Hội An",
    price: 600000,
    duration: "Nửa ngày",
    status: "Đã Ẩn",
    type: "Nửa ngày",
  },
  {
    id: 4,
    name: "Đà Nẵng - Huế Di Sản",
    desc: "Hành trình khám phá Cố đô Huế tráng lệ, cổ kính",
    price: 2200000,
    duration: "2 ngày 1 đêm",
    status: "Hoạt động",
    type: "2 ngày 1 đêm",
  },
];

let currentTours = [...tours];

// Phân trang
const rowsPerPage = 4;
let currentPage = 1;

// DOM Elements
const tableBody = document.getElementById("tableBody");
const searchInput = document.getElementById("searchInput");
const filterStatus = document.getElementById("filterStatus");
const filterType = document.getElementById("filterType");
const filterDuration = document.getElementById("filterDuration");
const btnAddTour = document.getElementById("btnAddTour");
const modalOverlay = document.getElementById("modalOverlay");
const modalTitle = document.getElementById("modalTitle");
const tourForm = document.getElementById("tourForm");
const btnCancel = document.getElementById("btnCancel");
const prevPageBtn = document.getElementById("prevPage");
const nextPageBtn = document.getElementById("nextPage");
const infoCount = document.getElementById("infoCount");

// Các input form
const tourNameInput = document.getElementById("tourName");
const tourDescInput = document.getElementById("tourDesc");
const tourPriceInput = document.getElementById("tourPrice");
const tourDurationInput = document.getElementById("tourDuration");
const tourStatusInput = document.getElementById("tourStatus");

// Trạng thái modal: 'add' hoặc 'edit'
let modalMode = "add";
let editTourId = null;

// Định dạng giá tiền Việt nam
function formatPrice(v) {
  return v.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ".") + "đ";
}

// Lọc dữ liệu theo tìm kiếm và filter
function filterTours(tours) {
  const searchText = searchInput.value.trim().toLowerCase();
  const statusVal = filterStatus.value;
  const typeVal = filterType.value;
  const durationVal = filterDuration.value;

  return tours.filter((t) => {
    const matchSearch = t.name.toLowerCase().includes(searchText);
    const matchStatus = statusVal === "" || t.status === statusVal;
    const matchType = typeVal === "" || t.type === typeVal;
    const matchDuration = durationVal === "" || t.duration === durationVal;
    return matchSearch && matchStatus && matchType && matchDuration;
  });
}

// Render bảng dữ liệu tour
function renderTable() {
  let filtered = filterTours(currentTours);
  const total = filtered.length;
  const start = (currentPage - 1) * rowsPerPage;
  const end = start + rowsPerPage;
  const pageTours = filtered.slice(start, end);

  tableBody.innerHTML = "";

  if (pageTours.length === 0) {
    tableBody.innerHTML = `<tr><td colspan="6" style="text-align:center; padding: 20px; color: #888;">Không tìm thấy tour phù hợp</td></tr>`;
  } else {
    for (const t of pageTours) {
      const row = document.createElement("tr");

      // Tên tour
      const tdName = document.createElement("td");
      tdName.textContent = t.name;
      row.appendChild(tdName);

      // Mô tả
      const tdDesc = document.createElement("td");
      tdDesc.textContent = t.desc;
      tdDesc.style.color = "#6c757d";
      row.appendChild(tdDesc);

      // Giá
      const tdPrice = document.createElement("td");
      tdPrice.textContent = formatPrice(t.price);
      tdPrice.style.textAlign = "right";
      row.appendChild(tdPrice);

      // Thời lượng
      const tdDuration = document.createElement("td");
      tdDuration.textContent = t.duration;
      row.appendChild(tdDuration);

      // Trạng thái
      const tdStatus = document.createElement("td");
      const spanStatus = document.createElement("span");
      spanStatus.classList.add("badge");
      if (t.status === "Hoạt động") {
        spanStatus.classList.add("active");
      } else {
        spanStatus.classList.add("hidden");
      }
      spanStatus.textContent = t.status;
      tdStatus.appendChild(spanStatus);
      row.appendChild(tdStatus);

      // Hành động
      const tdAction = document.createElement("td");
      // Edit button
      const btnEdit = document.createElement("button");
      btnEdit.className = "btn-edit";
      btnEdit.textContent = "EDIT";
      btnEdit.addEventListener("click", () => openEditModal(t.id));
      tdAction.appendChild(btnEdit);
      // Delete button
      const btnDel = document.createElement("button");
      btnDel.className = "btn-delete";
      btnDel.textContent = "DELETE";
      btnDel.addEventListener("click", () => deleteTour(t.id));
      tdAction.appendChild(btnDel);

      row.appendChild(tdAction);

      tableBody.appendChild(row);
    }
  }

  // Cập nhật phân trang
  infoCount.textContent = `Hiển thị ${total === 0 ? 0 : start + 1}-${Math.min(
    end,
    total
  )} của ${total}`;
  prevPageBtn.disabled = currentPage === 1;
  nextPageBtn.disabled = end >= total;
}

// Mở modal thêm tour mới
function openAddModal() {
  modalMode = "add";
  modalTitle.textContent = "Thêm Tour Mới";
  tourForm.reset();
  modalOverlay.classList.add("show");
  editTourId = null;
  modalOverlay.setAttribute("aria-hidden", "false");
}

// Mở modal chỉnh sửa tour
function openEditModal(id) {
  modalMode = "edit";
  modalTitle.textContent = "Chỉnh sửa Tour";
  const t = currentTours.find((x) => x.id === id);
  if (!t) return;

  tourNameInput.value = t.name;
  tourDescInput.value = t.desc;
  tourPriceInput.value = t.price;
  tourDurationInput.value = t.duration;
  tourStatusInput.value = t.status;

  modalOverlay.classList.add("show");
  editTourId = id;
  modalOverlay.setAttribute("aria-hidden", "false");
}

// Thêm tour mới
function addTour(data) {
  const newId =
    currentTours.length > 0
      ? Math.max(...currentTours.map((t) => t.id)) + 1
      : 1;
  currentTours.push({
    id: newId,
    name: data.name,
    desc: data.desc,
    price: data.price,
    duration: data.duration,
    status: data.status,
    type: data.duration, // giữ type = duration
  });
  currentPage = 1;
  renderTable();
}

// Cập nhật tour
function updateTour(id, data) {
  const idx = currentTours.findIndex((t) => t.id === id);
  if (idx < 0) return;
  currentTours[idx] = {
    id,
    name: data.name,
    desc: data.desc,
    price: data.price,
    duration: data.duration,
    status: data.status,
    type: data.duration,
  };
  renderTable();
}

// Xoá tour
function deleteTour(id) {
  if (confirm("Bạn có chắc chắn muốn xoá tour này?")) {
    const idx = currentTours.findIndex((t) => t.id === id);
    if (idx >= 0) {
      currentTours.splice(idx, 1);
      // Nếu xoá tour cuối trang, chuyển trang nếu cần
      const filtered = filterTours(currentTours);
      const totalPages = Math.ceil(filtered.length / rowsPerPage);
      if (currentPage > totalPages) {
        currentPage = totalPages > 0 ? totalPages : 1;
      }
      renderTable();
    }
  }
}

// Đóng modal
function closeModal() {
  modalOverlay.classList.remove("show");
  modalOverlay.setAttribute("aria-hidden", "true");
}

// Xử lý form gửi
tourForm.addEventListener("submit", (e) => {
  e.preventDefault();
  const data = {
    name: tourNameInput.value.trim(),
    desc: tourDescInput.value.trim(),
    price: Number(tourPriceInput.value),
    duration: tourDurationInput.value,
    status: tourStatusInput.value,
  };
  if (modalMode === "add") {
    addTour(data);
  } else if (modalMode === "edit") {
    updateTour(editTourId, data);
  }
  closeModal();
});

// Sự kiện cho nút mở modal thêm tour
btnAddTour.addEventListener("click", openAddModal);
// Hủy modal
btnCancel.addEventListener("click", closeModal);
// Đóng modal khi click ngoài vùng modal
modalOverlay.addEventListener("click", (e) => {
  if (e.target === modalOverlay) closeModal();
});

// Sự kiện lọc và tìm kiếm
searchInput.addEventListener("input", () => {
  currentPage = 1;
  renderTable();
});
filterStatus.addEventListener("change", () => {
  currentPage = 1;
  renderTable();
});
filterType.addEventListener("change", () => {
  currentPage = 1;
  renderTable();
});
filterDuration.addEventListener("change", () => {
  currentPage = 1;
  renderTable();
});

// Phân trang
prevPageBtn.addEventListener("click", () => {
  if (currentPage > 1) {
    currentPage--;
    renderTable();
  }
});
nextPageBtn.addEventListener("click", () => {
  const filtered = filterTours(currentTours);
  const totalPages = Math.ceil(filtered.length / rowsPerPage);
  if (currentPage < totalPages) {
    currentPage++;
    renderTable();
  }
});

// Khởi tạo lần đầu
renderTable();
