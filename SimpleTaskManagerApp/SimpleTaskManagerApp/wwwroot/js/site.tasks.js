// INIT FUNCTIONS

document.addEventListener("DOMContentLoaded", function () {
    setupCreateButton();
    setupCreateFormSubmit();

    //Init Edit functionality
    setupEditButton();
    setupEditFormSubmit();
});


// CREATE TASK HANDLERS

function setupCreateButton() {
    document.getElementById("load-create-form").addEventListener("click", function () {
        fetch("/Tasks/CreatePartial")
            .then(response => response.text())
            .then(html => {
                document.getElementById("modal-form-container").innerHTML = html;
                initFlatpickr(); // Flatpickr for the dates
            });
    });
}

function setupCreateFormSubmit() {
    $(document).on('submit', '#createTaskForm', function (e) {
        e.preventDefault();

        var form = $(this);
        var token = $('input[name="__RequestVerificationToken"]', form).val();
        $.ajax({
            type: 'POST',
            url: '/Tasks/CreatePartial',
            data: form.serialize(),
            headers: {
                'RequestVerificationToken': token
            },
            success: function () {
                $('#createTaskModal').modal('hide');
                showToast("Task added!");
                location.reload();
            },
            error: function (xhr, status, error) {
                console.error("Status:", status);
                console.error("Error:", error);
                console.error("Response:", xhr.responseText);
                showToast("Something went wrong!");
            }
        });
    });
}


// EDIT TASK HANDLERS

// Load Edit form into modal
function setupEditButton() {
    $(document).on('click', '.load-edit-form', function () {
        var taskId = $(this).data('task-id');
        fetch(`/Tasks/EditPartial?id=${taskId}`)
            .then(response => response.text())
            .then(html => {
                $('#edit-modal-form-container').html(html);
                $('#editTaskModal').modal('show');
                initFlatpickr(); // Flatpickr for the edit form
            });
    });
}

// Submit Edit form via AJAX
function setupEditFormSubmit() {
    $(document).on('submit', '#editTaskForm', function (e) {
        e.preventDefault();

        var form = $(this);
        var token = $('input[name="__RequestVerificationToken"]', form).val();
        $.ajax({
            type: 'POST',
            url: '/Tasks/EditPartial',
            data: form.serialize(),
            headers: {
                'RequestVerificationToken': token
            },
            success: function () {
                $('#editTaskModal').modal('hide');
                showToast("Task updated!");
                location.reload();
            },
            error: function (xhr, status, error) {
                console.error("Edit error:", error);
                console.error("Response:", xhr.responseText);
                showToast("Failed to update task.");
            }
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