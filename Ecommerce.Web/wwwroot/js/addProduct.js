
tinymce.init({
    selector: 'textarea',
    menubar: false,
    plugins: 'lists link image preview code',
    toolbar: 'undo redo | bold italic underline | bullist numlist | alignleft aligncenter alignright | preview code',
    height: 250
});

function previewImage(event) {
    const input = event.target;
    const preview = document.getElementById('imagePreview');
    const file = input.files[0];

    if (file) {
        const allowedExtensions = ['image/jpeg', 'image/png', 'image/jpg'];
        const maxSize = 2 * 1024 * 1024;

        if (!allowedExtensions.includes(file.type)) { 
            toastr.error("Only JPG, PNG, files are allowed.");
            input.value = "";
            preview.classList.add('d-none');
            return;
        }

        if (file.size > maxSize) {
            toastr.error("File size must be less than 2MB!");
            input.value = ""; 
            preview.classList.add('d-none');
            return;
        }

        const reader = new FileReader();
        reader.onload = function (e) {
            preview.src = e.target.result;
            preview.classList.remove('d-none');
        };
        reader.readAsDataURL(file);
    }
}
