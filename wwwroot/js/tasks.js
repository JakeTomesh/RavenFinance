// Dashboard task list: add, edit (double-click), complete (deletes), and drag to reorder.
// Each change is saved immediately through the Task controller.
(function () {
    const list = document.getElementById("task-list");
    const addForm = document.getElementById("task-add-form");
    const addInput = document.getElementById("task-add-input");
    if (!list || !addForm) return;

    /** POSTs form-encoded data to /Task/{action}; resolves to the Response. */
    function post(action, data) {
        return fetch("/Task/" + action, {
            method: "POST",
            headers: { "Content-Type": "application/x-www-form-urlencoded" },
            body: new URLSearchParams(data)
        });
    }

    /** Builds one task <li> (drag handle, text, complete button). */
    function createItem(id, text) {
        const li = document.createElement("li");
        li.className = "task-item";
        li.dataset.id = id;
        li.draggable = true;
        li.innerHTML =
            '<input type="checkbox" class="form-check-input task-done" title="Mark complete" />' +
            '<span class="task-text" title="Double-click to edit"></span>';
        li.querySelector(".task-text").textContent = text;
        return li;
    }

    /** Sends the current top-to-bottom order to the server. */
    function saveOrder() {
        const ids = [...list.children].map(li => li.dataset.id);
        post("Reorder", ids.map(id => ["ids", id]));
    }

    // Add a task
    addForm.addEventListener("submit", async e => {
        e.preventDefault();
        const text = addInput.value.trim();
        if (!text) return;
        const res = await post("Add", { text });
        if (res.ok) {
            const { id } = await res.json();
            list.appendChild(createItem(id, text));
            addInput.value = "";
        }
    });

    // Complete a task: it is deleted on the server and removed from the list
    list.addEventListener("change", async e => {
        if (!e.target.classList.contains("task-done")) return;
        const li = e.target.closest(".task-item");
        const res = await post("Complete", { id: li.dataset.id });
        if (res.ok) li.remove(); else e.target.checked = false;
    });

    // Edit a task: double-click to type, Enter or clicking away saves, Escape cancels
    list.addEventListener("dblclick", e => {
        const span = e.target.closest(".task-text");
        if (!span) return;
        const li = span.closest(".task-item");
        const original = span.textContent;
        li.draggable = false; // lets the text be selected while editing
        span.contentEditable = "true";
        span.focus();

        let cancelled = false;
        const finish = async () => {
            span.contentEditable = "false";
            li.draggable = true;
            const text = span.textContent.trim();
            if (cancelled || !text || text === original) { span.textContent = original; return; }
            const res = await post("Edit", { id: li.dataset.id, text });
            if (!res.ok) span.textContent = original;
        };
        span.addEventListener("blur", finish, { once: true });
        span.addEventListener("keydown", ev => {
            if (ev.key === "Enter") { ev.preventDefault(); span.blur(); }
            if (ev.key === "Escape") { cancelled = true; span.blur(); }
        });
    });

    // Drag to reorder: the dragged item is moved above/below whichever item the pointer is over
    let dragged = null;
    list.addEventListener("dragstart", e => {
        dragged = e.target.closest(".task-item");
        if (dragged) dragged.classList.add("dragging");
    });
    list.addEventListener("dragover", e => {
        e.preventDefault();
        const over = e.target.closest(".task-item");
        if (!dragged || !over || over === dragged) return;
        const middle = over.getBoundingClientRect().top + over.offsetHeight / 2;
        list.insertBefore(dragged, e.clientY < middle ? over : over.nextSibling);
    });
    list.addEventListener("dragend", () => {
        if (!dragged) return;
        dragged.classList.remove("dragging");
        dragged = null;
        saveOrder();
    });
})();
