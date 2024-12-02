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

        for (const presupuesto of data) {
            const li = document.createElement('li');
            const a = document.createElement('a');
            a.href = "#";
            a.setAttribute("data-url", "GestionCategorias.html");

            const h2 = document.createElement('h2');
            h2.textContent = presupuesto.nombrePresupuesto;

            const p = document.createElement('p');
            p.textContent = `Monto: ${presupuesto.montoPresupuesto}`;

            const categorias = await obtenerCategoriasPorPresupuesto(presupuesto.idPresupuesto);
            
            const categoriasList = document.createElement('ul');
            categorias.forEach(categoria => {
                const categoriaItem = document.createElement('li');
                categoriaItem.textContent = `${categoria.Nombre_Cat}: ${categoria.MontoEstimado}`;
                categoriasList.appendChild(categoriaItem);
            });

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
            a.appendChild(categoriasList);
            li.appendChild(a);
            li.appendChild(btnEliminar);
            listaPresupuestos.appendChild(li);
        }
    } catch (error) {
        console.error("Error al cargar los datos:", error);
    }
}

async function obtenerCategoriasPorPresupuesto(idPresupuesto) {
    const apiEndpoint = `http://localhost:5004/ObtenerCategorias?idPresupuesto=${idPresupuesto}`;

    try {
        const response = await fetch(apiEndpoint);
        if (!response.ok) throw new Error("Error al obtener las categorías.");

        const data = await response.json();
        return data;
    } catch (error) {
        console.error("Error al obtener las categorías:", error);
        return [];
    }
}

cargarPresupuestos();
