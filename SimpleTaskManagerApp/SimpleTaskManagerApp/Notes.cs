/*
Create modal 101

1. ADD MODAL BUTTON

<button class="btn btn-success mb-3" data-bs-toggle="modal" data-bs-target="#createTaskModal">
    + Create Task
</button>

2. ADD MODAL IN THE HTML FILE END


<div class="modal fade" id="createTaskModal" tabindex="-1" aria-labelledby="createTaskModalLabel" aria-hidden="true">
	<div class="modal-dialog">
		<div class="modal-content">
			<div class="modal-header">
				<h5 class="modal-title" id="createTaskModalLabel">Create Task</h5>
				<button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
			</div>
			<div class="modal-body">
				<div id="modal-form-container">
					
				Place to load AJAX

				</div>
			</div>
		</div>
	</div>
</div>


3. CREATE PARTIAL VIEW

Go to Views/Tasks/_CreatePartial.cshtml and create new view

Add the form from Create.cshtml

@using SimpleTaskManagerApp.ViewModels.AppTask
@model AppTaskCreateViewModel

<form asp-action="Create" method="post">
	@Html.AntiForgeryToken()
	<div class="form-floating mb-3">
		<input asp-for="Title" class="form-control" />
		<label asp-for="Title"></label>
		<span asp-validation-for="Title" class="text-danger"></span>
	</div>

	<div class="form-floating mb-3">
		<textarea asp-for="Description" class="form-control"></textarea>
		<label asp-for="Description"></label>
		<span asp-validation-for="Description" class="text-danger"></span>
	</div>

	<div class="form-floating mb-3">
		@* Add JS Flatpickr to input*@
		<input asp-for="DueDate" class="form-control datepicker" />
		<label asp-for="DueDate"></label>
		<span asp-validation-for="DueDate" class="text-danger"></span>
	</div>

	<div class="form-floating mb-3">
		<select asp-for="StatusId" class="form-control" asp-items="@(new SelectList(Model.Statuses, "Id", "Name"))"></select>
		<label asp-for="StatusId"></label>
		<span asp-validation-for="StatusId" class="text-danger"></span>
	</div>

	<button type="submit" class="btn btn-primary">Create</button>
</form>

4.ADD SECTION SCRIPTS IN INDEX.CSHTML

@section Scripts {
	<partial name="_ValidationScriptsPartial" />

	<script src="https://cdn.jsdelivr.net/npm/flatpickr"></script>
	<script>
		// Load form in modal
		document.getElementById("load-create-form").addEventListener("click", function () {
			fetch("/Tasks/CreatePartial")
				.then(response => response.text())
				.then(html => {
					document.getElementById("modal-form-container").innerHTML = html;

					// flatpickr initalization inside the modal
					flatpickr(".datepicker", {
						enableTime: true,
						dateFormat: "Y-m-d H:i",
						time_24hr: true
					});
				});
		});


		// AJAX submit of the form
		$(document).on('submit', '#createTaskForm', function (e) {
			e.preventDefault();

			var form = $(this);
			$.ajax({
				type: 'POST',
				url: '/Tasks/Create',
				data: form.serialize(),
				success: function () {
					$('#createTaskModal').modal('hide');
					showToast("Task added!");
					location.reload();
				},
				error: function () {
					showToast("Something went wrong!");
				}
			});
		});


		// Toast function
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
	</script>


}


5. CREATE CreatePartial in TaskController
[HttpGet]
public async Task<IActionResult> CreatePartial()
{
	var model = new AppTaskCreateViewModel
	{
		Statuses = await _appTaskService.GetAllStatusesAsync()
	};

	return PartialView("_CreatePartial", model);
}

*/