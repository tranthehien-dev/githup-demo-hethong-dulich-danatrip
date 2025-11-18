document.querySelectorAll(".dropdown").forEach((dropdown) => {
  const btn = dropdown.querySelector(".filter-btn");
  const menu = dropdown.querySelector(".dropdown-menu");

  btn.addEventListener("click", () => {
    menu.classList.toggle("show");
  });

  menu.querySelectorAll("a").forEach((item) => {
    item.addEventListener("click", (e) => {
      e.preventDefault();
      btn.textContent =
        btn.textContent.split(":")[0] + ": " + item.textContent + " ▼";
      menu.classList.remove("show");
    });
  });
});

window.addEventListener("click", (e) => {
  document.querySelectorAll(".dropdown-menu.show").forEach((menu) => {
    if (!menu.parentElement.contains(e.target)) {
      menu.classList.remove("show");
    }
  });
});

// --- Modal Add / Edit ---
const modal = document.getElementById("food-modal");
const modalTitle = document.getElementById("modal-title");
const form = document.getElementById("food-form");

const deleteModal = document.getElementById("delete-modal");
let currentRow = null;

// Thêm mới
document.querySelector(".btn-add").addEventListener("click", () => {
  modalTitle.textContent = "Thêm Mới Món Ăn";
  form.reset();
  modal.style.display = "block";
  currentRow = null;
});

// Edit
document.querySelectorAll(".edit").forEach((btn) => {
  btn.addEventListener("click", (e) => {
    e.preventDefault();
    const row = e.target.closest("tr");
    currentRow = row;
    modalTitle.textContent = "Chỉnh Sửa Món Ăn";

    document.getElementById("food-name").value = row.cells[1].textContent;
    document.getElementById("food-desc").value = row.cells[2].textContent;
    const status = row.cells[3].textContent.includes("Đang")
      ? "active"
      : "inactive";
    document.getElementById("food-status").value = status;
    document.getElementById("food-img").value = row.querySelector("img").src;

    modal.style.display = "block";
  });
});

// Delete
document.querySelectorAll(".delete").forEach((btn) => {
  btn.addEventListener("click", (e) => {
    e.preventDefault();
    currentRow = e.target.closest("tr");
    deleteModal.style.display = "block";
  });
});

// Close modal
document.querySelectorAll(".close").forEach((btn) => {
  btn.addEventListener("click", () => {
    modal.style.display = "none";
    deleteModal.style.display = "none";
  });
});

// Submit form
form.addEventListener("submit", (e) => {
  e.preventDefault();
  const name = document.getElementById("food-name").value;
  const desc = document.getElementById("food-desc").value;
  const status = document.getElementById("food-status").value;
  const img = document.getElementById("food-img").value;

  if (currentRow) {
    // Edit row
    currentRow.cells[1].textContent = name;
    currentRow.cells[2].textContent = desc;
    currentRow.cells[3].innerHTML =
      status === "active"
        ? '<span class="status status-active">Đang hoạt động</span>'
        : '<span class="status status-inactive">Tạm ẩn</span>';
    currentRow.querySelector("img").src = img;
  } else {
    // Add new row
    const tbody = document.querySelector(".food-table tbody");
    const tr = document.createElement("tr");
    tr.innerHTML = `
      <td><img class="food-img" src="${img}" alt="${name}"></td>
      <td>${name}</td>
      <td>${desc}</td>
      <td>${
        status === "active"
          ? '<span class="status status-active">Đang hoạt động</span>'
          : '<span class="status status-inactive">Tạm ẩn</span>'
      }</td>
      <td><a href="#" class="edit">edit</a><a href="#" class="delete">delete</a></td>
    `;
    tbody.appendChild(tr);
    attachRowEvents(tr);
  }

  modal.style.display = "none";
});

// Confirm delete
document.getElementById("confirm-delete").addEventListener("click", () => {
  if (currentRow) currentRow.remove();
  deleteModal.style.display = "none";
});

// Hủy xóa (nếu cần, thêm nút hủy trong modal xóa)
document.querySelector(".cancel").addEventListener("click", () => {
  deleteModal.style.display = "none";
});

function attachRowEvents(row) {
  row.querySelector(".edit").addEventListener("click", (e) => {
    e.preventDefault();
    currentRow = row;
    modalTitle.textContent = "Chỉnh Sửa Món Ăn";
    document.getElementById("food-name").value = row.cells[1].textContent;
    document.getElementById("food-desc").value = row.cells[2].textContent;
    const status = row.cells[3].textContent.includes("Đang")
      ? "active"
      : "inactive";
    document.getElementById("food-status").value = status;
    document.getElementById("food-img").value = row.querySelector("img").src;
    modal.style.display = "block";
  });

  row.querySelector(".delete").addEventListener("click", (e) => {
    e.preventDefault();
    currentRow = row;
    deleteModal.style.display = "block";
  });
}

// Close modal when click outside
window.addEventListener("click", (e) => {
  if (e.target == modal) modal.style.display = "none";
  if (e.target == deleteModal) deleteModal.style.display = "none";
});
