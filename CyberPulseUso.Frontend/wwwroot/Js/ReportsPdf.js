window.displayPdf = function (pdfBytes) {
    // Convertir el arreglo de bytes a Blob
    const blob = new Blob([new Uint8Array(pdfBytes)], { type: 'application/pdf' });
    const url = URL.createObjectURL(blob);
    window.open(url, '_blank');

    // Liberar memoria después de un tiempo prudente
    setTimeout(() => URL.revokeObjectURL(url), 10000);
};