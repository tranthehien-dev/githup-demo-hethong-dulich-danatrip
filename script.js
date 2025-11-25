/* ----------- LƯU TRẠNG THÁI BAN ĐẦU ----------- */
let originalData = {};

function storeOriginalData() {
    originalData = {
        name: document.getElementById("name").value,
        phone: document.getElementById("phone").value,
        email: document.getElementById("email").value,
        address: document.getElementById("address").value,
        sidebarName: document.getElementById("sidebarName").textContent,
        sidebarEmail: document.getElementById("sidebarEmail").textContent
    };
}
storeOriginalData();

/* ----------- LƯU THAY ĐỔI ----------- */
function saveChanges() {
    const name = document.getElementById("name").value.trim();
    const email = document.getElementById("email").value.trim();

    // cập nhật sidebar
    document.getElementById("sidebarName").textContent = name || "---";
    document.getElementById("sidebarEmail").textContent = email || "---";

    openModal();
    storeOriginalData(); // lưu snapshot mới
}

/* ----------- HỦY THAY ĐỔI ----------- */
function cancelChanges() {

    // Khôi phục form
    document.getElementById("name").value = originalData.name;
    document.getElementById("phone").value = originalData.phone;
    document.getElementById("email").value = originalData.email;
    document.getElementById("address").value = originalData.address;

    // Khôi phục sidebar
    document.getElementById("sidebarName").textContent = originalData.sidebarName;
    document.getElementById("sidebarEmail").textContent = originalData.sidebarEmail;
}

/* ----------- MODAL ----------- */
function openModal() {
    document.getElementById("modal").style.display = "flex";
}
function closeModal() {
    document.getElementById("modal").style.display = "none";
}
function closeOnBg(e) {
    if (e.target.id === "modal") closeModal();
}

/* ----------- TABS ----------- */
const tabs = document.querySelectorAll("#tabs span");
const content = document.getElementById("tab-content");

tabs.forEach(tab => {
    tab.addEventListener("click", () => {

        tabs.forEach(t => t.classList.remove("active"));
        tab.classList.add("active");

        const type = tab.getAttribute("data-tab");

        switch(type) {
            case "upcoming":
                content.innerHTML = `
                    <p><strong>Chưa có chuyến đi nào</strong></p>
                    <p>Bạn chưa đặt tour nào sắp tới.</p>
                    <button class="save-btn">Khám phá các tour</button>
                `;
                break;

            case "done":
                content.innerHTML = `
                    <p><strong>Chưa có chuyến đi nào</strong></p>
                    <p>Bạn chưa có tour đã hoàn thành.</p>
                `;
                break;

            case "canceled":
                content.innerHTML = `
                    <p><strong>Chưa có chuyến đi nào</strong></p>
                    <p>Bạn chưa hủy tour nào.</p>
                `;
                break;
        }
    });
});
