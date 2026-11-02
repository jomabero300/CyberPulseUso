window.downloadFileFromBytes = function (byteArray, contentType, fileName) {
    const blob = new Blob([new Uint8Array(byteArray)], { type: contentType });
    const url = URL.createObjectURL(blob);

    // Crear un enlace temporal para forzar la descarga
    const anchorElement = document.createElement('a');
    anchorElement.href = url;
    anchorElement.download = fileName ?? 'archivo.xlsx';
    document.body.appendChild(anchorElement);
    anchorElement.click();
    document.body.removeChild(anchorElement);

    // Liberar memoria
    setTimeout(() => URL.revokeObjectURL(url), 10000);
};