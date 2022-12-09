function changeLabelTextContent(myFile) {
    var file = myFile.files[0];

    var fileLabel = document.getElementsByClassName('custom-file-label');

    fileLabel[0].textContent = file.name;
}