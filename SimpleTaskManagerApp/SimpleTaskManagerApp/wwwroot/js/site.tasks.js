// INIT FUNCTIONS

document.addEventListener("DOMContentLoaded", function () {
    setupCreateButton();
    setupCreateFormSubmit();
    setupEditButton();
    setupEditFormSubmit();
    setupDeleteSubmit();
    setupDetailsModal();
});

// CREATE TASK HANDLERS

function setupCreateButton() {
    document.getElementById("load-create-form").addEventListener("click", function () {
        fetch("/Tasks/CreatePartial")
            .then(response => response.text())
            .then(html => {
                document.getElementById("modal-form-container").innerHTML = html;
                initFlatpickr();
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
                    bootstrap.Modal.getInstance(document.getElementById('createTaskModal')).hide();
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
                    document.getElementById('edit-modal-form-container').innerHTML = html;
                    new bootstrap.Modal(document.getElementById('editTaskModal')).show();
                    initFlatpickr();
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

            fetch('/Tasks/EditPartial', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded',
                    'RequestVerificationToken': token
                },
                body: new URLSearchParams(new FormData(form))
            })
                .then(response => {
                    if (!response.ok) throw new Error("Edit failed");
                    bootstrap.Modal.getInstance(document.getElementById('editTaskModal')).hide();
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

    document.getElementById('confirmDeleteBtn').addEventListener('click', function () {
        if (!currentDeleteTaskId) return;
        const token = document.querySelector('input[name="__RequestVerificationToken"]').value;

        fetch(`/Tasks/Delete/${currentDeleteTaskId}`, {
            method: 'POST',
            headers: {
                'RequestVerificationToken': token
            }
        })
            .then(response => {
                if (!response.ok) {
                    throw new Error("Delete failed")
                };
                bootstrap.Modal.getInstance(document.getElementById('deleteConfirmModal')).hide();
                showToast("Task deleted!");
                location.reload();
            })
            .catch(() => {
                showToast("Failed to delete task.");
            });
    });

}

//DETAILS TASK HANDLER
function setupDetailsModal() {
    document.addEventListener('click', function (e) {
        if (e.target && e.target.classList.contains('load-details-modal')) {
            const taskId = e.target.dataset.taskId;

            fetch(`/Tasks/DetailsPartial?id=${taskId}`)
                .then(response => response.text())
                .then(html => {
                    document.getElementById('details-modal-form-container').innerHTML = html;
                    const modal = new bootstrap.Modal(document.getElementById('detailsTaskModal'));
                    modal.show();
                })
                .catch(() => {
                    showToast("Failed to load task details.");
                });
        }
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
