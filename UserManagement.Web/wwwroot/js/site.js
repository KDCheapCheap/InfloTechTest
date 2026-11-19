// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// This initializes tooltips for elements with data-bs-toggle="tooltip"
document.querySelectorAll('[data-bs-toggle="tooltip"]').forEach((tooltipTriggerEl) => {
	new bootstrap.Tooltip(tooltipTriggerEl);
});
