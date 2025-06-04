
document.addEventListener("DOMContentLoaded", function () {
    setupCreateButton();
    setupCreateFormSubmit();
    setupEditButton();
    setupEditFormSubmit();
    setupDeleteSubmit();
    setupDetailsModal();
    loadEditModalAdmin();
    setupCancelEdit();
    setupAdminCreateButton();
});

// CREATE TASK HANDLERS

function setupCreateButton() {
    const btn = document.getElementById("load-create-form");
    if (!btn) return;

    btn.addEventListener("click", function () {
        fetch("/Tasks/CreatePartial")
            .then(response => response.text())
            .then(html => {
                const container = document.getElementById("modal-form-container");
                if (container) {
                    container.innerHTML = html;
                    initFlatpickr();
                }
            });
    });
}

function setupCreateFormSubmit() {
    document.addEventListener('submit', function (e) {
        if (e.target && e.target.id === 'createTaskForm') {
            e.preventDefault();
            const form = e.target;
            const token = form.querySelector('input[name="__RequestVerificationToken"]').value;

            fetch('/Tasks/CreatePartial', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded',
                    'RequestVerificationToken': token
                },
                body: new URLSearchParams(new FormData(form))
            })
                .then(response => {
                    if (!response.ok) throw new Error("Request failed");
                    const modal = bootstrap.Modal.getInstance(document.getElementById('createTaskModal'));
                    if (modal) modal.hide();
                    showToast("Task added!");
                    location.reload();
                })
                .catch(err => {
                    console.error("Create error:", err);
                    showToast("Something went wrong!");
                });
        }
    });
}

// EDIT TASK HANDLERS

function setupEditButton() {
    document.addEventListener('click', function (e) {
        if (e.target && e.target.classList.contains('load-edit-form')) {
            const taskId = e.target.dataset.taskId;
            fetch(`/Tasks/EditPartial?id=${taskId}`)
                .then(response => response.text())
                .then(html => {
                    const container = document.getElementById('edit-modal-form-container');
                    if (container) {
                        container.innerHTML = html;
                        new bootstrap.Modal(document.getElementById('editTaskModal')).show();
                        initFlatpickr();
                    }
                });
        }
    });
}

function setupEditFormSubmit() {
    document.addEventListener('submit', function (e) {
        if (e.target && e.target.id === 'editTaskForm') {
            e.preventDefault();
            const form = e.target;
            const token = form.querySelector('input[name="__RequestVerificationToken"]').value;

            const formData = new FormData(form);

            const dueDateInput = form.querySelector('input[name="DueDate"]');
            if (dueDateInput && dueDateInput.value) {
                const localDate = new Date(dueDateInput.value);
                formData.set("DueDate", localDate.toISOString());
            }

            fetch('/Tasks/EditPartial', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded',
                    'RequestVerificationToken': token
                },
                body: new URLSearchParams(formData)
            })
                .then(response => {
                    if (!response.ok) throw new Error("Edit failed");
                    const modal = bootstrap.Modal.getInstance(document.getElementById('editTaskModal'));
                    if (modal) modal.hide();
                    showToast("Task updated!");
                    location.reload();
                })
                .catch(err => {
                    console.error("Edit error:", err);
                    showToast("Failed to update task.");
                });
        }
    });
}

// DELETE TASK HANDLERS

function setupDeleteSubmit() {
    let currentDeleteTaskId = null;

    document.addEventListener('click', function (e) {
        if (e.target && e.target.classList.contains('load-delete-modal')) {
            currentDeleteTaskId = e.target.dataset.taskId;
        }
    });

    const deleteBtn = document.getElementById('confirmDeleteBtn');
    if (deleteBtn) {
        deleteBtn.addEventListener('click', function () {
            if (!currentDeleteTaskId) return;

            const tokenInput = document.querySelector('input[name="__RequestVerificationToken"]');
            if (!tokenInput) {
                showToast("Token not found.");
                return;
            }

            fetch(`/Tasks/Delete/${currentDeleteTaskId}`, {
                method: 'POST',
                headers: {
                    'RequestVerificationToken': tokenInput.value
                }
            })
                .then(response => {
                    if (!response.ok) {
                        throw new Error("Delete failed");
                    }
                    const modal = bootstrap.Modal.getInstance(document.getElementById('deleteConfirmModal'));
                    if (modal) modal.hide();
                    showToast("Task deleted!");
                    location.reload();
                })
                .catch(() => {
                    showToast("Failed to delete task.");
                });
        });
    }
}

// DETAILS TASK HANDLER

function setupDetailsModal() {
    document.addEventListener('click', function (e) {
        if (e.target && e.target.classList.contains('load-details-modal')) {
            const taskId = e.target.dataset.taskId;

            fetch(`/Tasks/DetailsPartial?id=${taskId}`)
                .then(response => response.text())
                .then(html => {
                    const container = document.getElementById('details-modal-form-container');
                    if (container) {
                        container.innerHTML = html;
                        const modalEl = document.getElementById('detailsTaskModal');
                        const modal = bootstrap.Modal.getOrCreateInstance(modalEl);
                        modalEl.addEventListener('hidden.bs.modal', function () {
                            const backdrops = document.querySelectorAll('.modal-backdrop');
                            backdrops.forEach(b => b.remove());
                        });


                        modal.show();
                    }
                })
                .catch(() => {
                    showToast("Failed to load task details.");
                });
        }
    });
}

// EDIT TASK HANDLER ADMIN

function loadEditModalAdmin() {
    console.log("loadEditModalAdmin initialized");

    document.addEventListener('click', async function (e) {
        const button = e.target.closest('.load-admin-edit-form');
        if (!button) {
            return;
        }
        console.log("Edit admin button clicked");

        const taskId = button.getAttribute("data-task-id");
        const editContainerId = "edit-container-" + taskId;

        console.log(`Fetching EditPartial for ID: ${taskId}`);

        const response = await fetch(`/Administrator/Tasks/EditPartial?id=${taskId}`);
        const editContainer = document.getElementById(editContainerId);

        if (!response.ok) {
            console.error("Failed to fetch partial:", response.status);
            if (editContainer) {
                editContainer.innerHTML = `<div class="text-danger">Failed to load form.</div>`;
            }
            return;
        }

        const partialHtml = await response.text();
        if (editContainer) {
            editContainer.innerHTML = partialHtml;

            initFlatpickr();
        }
    });
}

//Remove EditPartial from Administrator/Tasks
function setupCancelEdit() {
    document.addEventListener("click", function (e) {
        if (e.target && e.target.classList.contains("cancel-edit")) {
            const formContainer = e.target.closest("form")?.parentElement;
            if (formContainer) {
                formContainer.style.display = "none";
            }
        }
    });
}

//Admin Create
function setupAdminCreateButton() {
    const button = document.getElementById("load-admin-create-form");
    const container = document.getElementById("admin-create-form-container");

    if (!button || !container) {
        return
    };

    button.addEventListener("click", function () {

        //If there is form, hide it
        if (container.innerHTML.trim() !== "") {
            container.innerHTML = "";
            button.textContent = '+ New Task'
            return;
        }

        //If not, load the form
        fetch("/Tasks/CreatePartial")
            .then(res => res.text())
            .then(html => {
                container.innerHTML = html;
                initFlatpickr();
                button.textContent = 'Cancel';
            })
            .catch(err => {
                console.error(err);
                showToast("Failed to load create form.");
            });
    });
}



// FLATPICKR INIT

function initFlatpickr() {
    flatpickr(".datepicker", {
        enableTime: true,
        dateFormat: "Y-m-d H:i",
        time_24hr: true
    });
}

// TOAST

function showToast(message) {
    const toast = document.createElement('div');
    toast.className = 'toast align-items-center text-bg-primary border-0 show position-fixed bottom-0 end-0 m-4';
    toast.style.zIndex = 10000;
    toast.innerHTML = `
        <div class="d-flex">
            <div class="toast-body">${message}</div>
            <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast"></button>
        </div>`;
    document.body.appendChild(toast);
    setTimeout(() => toast.remove(), 3000);
}
