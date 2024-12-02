async function eliminarPresupuesto(idPresupuesto) {
    const idUsuario = localStorage.getItem('idUsuario');

    if (!idUsuario) {
        alert("No se ha encontrado el ID del usuario.");
        return;
    }

    if (confirm("¿Estás seguro de que deseas eliminar este presupuesto?")) {
        try {
            const response = await fetch(`http://localhost:5004/EliminarPresupuesto?idPresupuesto=${idPresupuesto}&idUsuario=${idUsuario}`, {
                method: 'DELETE',
            });

            if (response.ok) {
                alert('Presupuesto eliminado exitosamente');
                cargarPresupuestos();
            } else {
                alert('Error al eliminar el presupuesto');
            }
        } catch (error) {
            console.error('Error al eliminar el presupuesto:', error);
            alert('Hubo un error al eliminar el presupuesto.');
        }
    }
}

async function cargarPresupuestos() {
    const idUsuario = localStorage.getItem('idUsuario');

    if (!idUsuario) {
        alert("No se ha encontrado el ID del usuario.");
        return;
    }

    const apiEndpoint = `http://localhost:5004/VisualizarPresupuestos?idCliente=${idUsuario}`;

    try {
        const response = await fetch(apiEndpoint);
        if (!response.ok) throw new Error("Error al obtener los presupuestos.");

        const data = await response.json();
        const listaPresupuestos = document.getElementById('presupuestos-list');

        listaPresupuestos.innerHTML = '';

        data.forEach(presupuesto => {
            const li = document.createElement('li');
            const a = document.createElement('a');
            a.href = "#";
            a.setAttribute("data-url", "GestionCategorias.html");

            const h2 = document.createElement('h2');
            h2.textContent = presupuesto.nombrePresupuesto;

            const p = document.createElement('p');
            p.textContent = `Monto: ${presupuesto.montoPresupuesto}`;

            const btnEliminar = document.createElement('button');
            btnEliminar.classList.add('btn-eliminar');
            btnEliminar.textContent = 'Eliminar';
            btnEliminar.addEventListener('click', () => {
                eliminarPresupuesto(presupuesto.idPresupuesto);
            });

            a.addEventListener('click', () => {
                actualizarPagina(presupuesto.idPresupuesto);
            });

            a.appendChild(h2);
            a.appendChild(p);
            li.appendChild(a);
            li.appendChild(btnEliminar);
            listaPresupuestos.appendChild(li);
        });
    } catch (error) {
        console.error("Error al cargar los datos:", error);
    }
}

function actualizarPagina(idPresupuesto) {
    window.location.href = 'GestionCategorias.html';
    localStorage.setItem('idPresupuesto', idPresupuesto);
    console.log("ID de presupuesto guardado en localStorage:", idPresupuesto);
}

document.getElementById('agregarPresupuesto').addEventListener('click', () => {
    document.getElementById('formularioPresupuesto').style.display = 'block';
});

function closeFormulario() {
    document.getElementById("formularioPresupuesto").style.display = "none";
}

document.getElementById("agregarPresupuesto").onclick = function() {
    document.getElementById("formularioPresupuesto").style.display = "block";
};

cargarPresupuestos();
