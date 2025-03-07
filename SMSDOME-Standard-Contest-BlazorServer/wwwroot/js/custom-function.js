function saveAsFile(fileName, contentType, content) {
    const file = new File([content], fileName, { type: contentType });
    const exportUrl = URL.createObjectURL(file)

    var link = document.createElement('a');
    link.download = fileName;
    link.href = exportUrl
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);

    URL.revokeObjectURL(exportUrl);
}


function ShowToast() {
    var toastLiveExample = document.getElementById('liveToast')
    var toast = new bootstrap.Toast(toastLiveExample)
    toast.show()
}

window.bootstrapModal = {
    show: (modalId) => {
        var modal = new bootstrap.Modal(document.getElementById(modalId));
        modal.show();
    },
    hide: (modalId) => {
        var modalElement = document.getElementById(modalId);
        var modal = bootstrap.Modal.getInstance(modalElement);
        if (modal) modal.hide();
    }
};
