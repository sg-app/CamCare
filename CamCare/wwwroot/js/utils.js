window.downloadFileFromByteArray = function (fileName, byteArray) {
  var blob = new Blob([byteArray], { type: "application/octet-stream" });
  var url = URL.createObjectURL(blob);
  var a = document.createElement("a");
  a.style.display = "none";
  a.href = url;
  a.download = fileName;
  document.body.appendChild(a);
  a.click();
  URL.revokeObjectURL(url);
  document.body.removeChild(a);
};

// Rendert anhand der übergebenen URL einen QR-Code (qrcode.min.js, MIT, davidshimjs)
// in das Element mit der angegebenen ID. Vorheriger Inhalt wird ersetzt.
window.renderQrCode = function (elementId, text) {
  var container = document.getElementById(elementId);
  if (!container) return;
  container.innerHTML = "";
  new QRCode(container, { text: text, width: 100, height: 100, correctLevel: QRCode.CorrectLevel.M });
};

// Kopiert Text in die Zwischenablage (Fallback mit execCommand, falls Clipboard-API blockiert ist).
window.copyTextToClipboard = function (text) {
  if (navigator.clipboard && navigator.clipboard.writeText) {
    navigator.clipboard.writeText(text);
    return;
  }
  var ta = document.createElement("textarea");
  ta.value = text;
  ta.style.position = "fixed";
  ta.style.opacity = "0";
  document.body.appendChild(ta);
  ta.select();
  document.execCommand("copy");
  document.body.removeChild(ta);
};
