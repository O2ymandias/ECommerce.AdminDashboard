const uploadImageInput = document.querySelector('#uploadImageInp');
const previewImage = document.querySelector('#previewImage');

uploadImageInput.addEventListener('change', function (event) {
	const file = event.target.files[0];
	if (file) {
		const reader = new FileReader();
		reader.onload = e => previewImage.src = e.target.result;
		reader.readAsDataURL(file);
	}
});