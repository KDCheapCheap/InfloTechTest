// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// This initializes tooltips for elements with data-bs-toggle="tooltip"
document.querySelectorAll('[data-bs-toggle="tooltip"]').forEach((tooltipTriggerEl) => {
	new bootstrap.Tooltip(tooltipTriggerEl);
});

const deleteUserModalElement = document.getElementById("deleteUserModal");
if (deleteUserModalElement) {
	const deleteUserModal = bootstrap.Modal.getOrCreateInstance(deleteUserModalElement);
	const deleteUserName = document.getElementById("deleteUserName");
	let pendingDeleteForm;

	document.querySelectorAll("form[data-confirm-delete]").forEach((form) => {
		form.addEventListener("submit", (event) => {
			if (form.dataset.confirmed === "true") {
				delete form.dataset.confirmed;
				return;
			}

			event.preventDefault();
			pendingDeleteForm = form;
			deleteUserName.textContent = form.dataset.userName;
			deleteUserModal.show();
		});
	});

	document.getElementById("confirmDeleteUser").addEventListener("click", () => {
		if (!pendingDeleteForm) {
			return;
		}

		const form = pendingDeleteForm;
		pendingDeleteForm = null;
		form.dataset.confirmed = "true";
		form.requestSubmit();
	});

	deleteUserModalElement.addEventListener("hidden.bs.modal", () => {
		pendingDeleteForm = null;
	});
}
