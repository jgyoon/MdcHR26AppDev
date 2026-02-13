// 엑셀 파일 다운로드
window.downloadURI = function (uri, name) {
    var link = document.createElement("a");
    link.download = name;
    link.href = uri;
    link.target = "_blank";
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
}
