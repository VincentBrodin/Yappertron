document.addEventListener("DOMContentLoaded", main)

window.chrome.webview.addEventListener('message', (event) => {
    const id = event.data.id;
    switch (id) {
        case "outputs":
            Outputs(event.data.content);
            break;
    }
});

function Outputs(content) {
    const outputList = document.getElementById("output-list");
    outputList.innerHTML = "";
    const list = document.createElement("ul");
    list.classList.add("list-disc", "pl-5");

    content.forEach((file) => {
        const listItem = document.createElement("li");
        listItem.classList.add("py-1", "border-b", "border-gray-200", "hover:bg-gray-100");
        listItem.textContent = file;
        list.appendChild(listItem);
    });

    outputList.appendChild(list)

    HideLoading();
    ShowDone();
}
function main() {
    const filesInput = document.getElementById("files");
    const videoInput = document.getElementById("video");
    const fileList = document.getElementById("file-list");
    const fileCount = document.getElementById("file-count");

    const sendForm = document.getElementById("send");

    filesInput.addEventListener("change", () => {
        console.log("Test")
        fileList.innerHTML = ""; // Clear any existing content

        const files = Array.from(filesInput.files);

        fileCount.innerText = `${files.length} files added`

        if (files.length === 0) {
            fileList.innerHTML = "<p>No files selected.</p>";
        } else {
            const list = document.createElement("ul");
            list.classList.add("list-disc", "pl-5");

            files.forEach((file) => {
                const listItem = document.createElement("li");
                listItem.classList.add("py-1", "border-b", "border-gray-200", "hover:bg-gray-100");
                listItem.textContent = `${file.name} (${formatFileSize(file.size)})`;
                list.appendChild(listItem);
            });

            fileList.appendChild(list);
        }
    });

    sendForm.addEventListener("submit", async (e) => {
        e.preventDefault();
        ShowLoading();

        const textFiles = Array.from(filesInput.files);
        const videoFile = videoInput.files[0];

        const filesData = await Promise.all(textFiles.map(file => readFileAsBase64(file)));
        const videoData = await readFileAsBase64(videoFile);

        const fileData = {
            id: "generate",
            content: {
                files: filesData,
                video: videoData
            }
        };

        window.chrome.webview.postMessage(fileData);
    });
}

function readFileAsBase64(file) {
    return new Promise((resolve, reject) => {
        const reader = new FileReader();
        reader.onload = () => {
            const base64Data = reader.result.split(',')[1];
            resolve({
                name: file.name,
                type: file.type,
                base64Data: base64Data
            });
        };
        reader.onerror = reject;
        reader.readAsDataURL(file);
    });
}

function formatFileSize(bytes) {
    const units = ['B', 'KB', 'MB', 'GB', 'TB'];
    let size = bytes;
    let unitIndex = 0;

    while (size >= 1024 && unitIndex < units.length - 1) {
        size /= 1024;
        unitIndex++;
    }

    return `${size.toFixed(2)} ${units[unitIndex]}`;
}

function ShowLoading() {
    const div = document.getElementById("loading");
    div.classList.add("overlay")
    div.classList.remove("noOverlay")
}

function HideLoading() {
    const div = document.getElementById("loading");
    div.classList.add("noOverlay")
    div.classList.remove("overlay")
}

function ShowDone() {
    const div = document.getElementById("done");
    div.classList.add("overlay")
    div.classList.remove("noOverlay")
}

function HideDone() {
    const div = document.getElementById("done");
    div.classList.add("noOverlay")
    div.classList.remove("overlay")
}