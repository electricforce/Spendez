const idUsuario = localStorage.getItem('idUsuario');

const apiEndpointPresupuestos = `http://localhost:5004/GastosUsuario?idUsuario=${idUsuario}`;


async function cargarGastos() {
  try {
    const response = await fetch(apiEndpointPresupuestos);
    if (!response.ok) throw new Error("Error al obtener los presupuestos.");

    const data = await response.json();
    const presupuestos = data || [];
    const tbody = document.querySelector("#presupuestos-table tbody");
    tbody.innerHTML = '';

    for (const presupuesto of presupuestos) {
      const tr = document.createElement('tr');

     
      const tdDescripcionGas = document.createElement('td');
      tdDescripcionGas.textContent = presupuesto.descripcionGas || 'Sin nombre'; 
      tr.appendChild(tdDescripcionGas);

     
      const tdMonto = document.createElement('td');
      tdMonto.textContent = `$${presupuesto.montoGas || 0}`;
      tr.appendChild(tdMonto);

      
      const tdFechaGas = document.createElement('td');
      if (presupuesto.fechaGas) {
        const fecha = new Date(presupuesto.fechaGas);
        const año = fecha.getFullYear();
        const mes = (fecha.getMonth() + 1).toString().padStart(2, '0'); // Mes con 2 dígitos
        const dia = fecha.getDate().toString().padStart(2, '0'); // Día con 2 dígitos
        tdFechaGas.textContent = `${año}-${mes}-${dia}`;
      } else {
        tdFechaGas.textContent = 'Sin fecha';
      }
      tr.appendChild(tdFechaGas);

      tbody.appendChild(tr);
    }
  } catch (error) {
    console.error("Error al cargar los presupuestos:", error);
    alert("Error al cargar los presupuestos.");
  }
}
cargarGastos();



async function regisGasto(event) {
    event.preventDefault();

    const idCat = null; // Cambia el id de los campos
    const descripcion = document.getElementById("descripcion").value;
    const monto = document.getElementById("monto").value;
    const fecha = document.getElementById("fecha").value;
    

    const url = "http://localhost:5004/AgregarGasto";

    const formData = new FormData();
    formData.append('idCat', null);
    formData.append('descripcion', descripcion);
    formData.append('monto', monto);
    formData.append('fecha', fecha);
    formData.append('idUsuario', idUsuario);

    try {
        const response = await fetch(url, {
            method: 'POST',
            body: formData,
        });

        if (!response.ok) {
            const errorData = await response.text(); // Obtener el error como texto
            throw new Error(errorData || 'Error al registrar el gasto');
        }

        const datos = await response.json();
        console.log('Gasto Registrado: ', datos);
        const alertMessage = document.getElementById("alert-message");
        alertMessage.textContent = "Gasto registrado con éxito";
        alertMessage.className = "alert alert-success";
        document.getElementById("alert-container").classList.remove("d-none");
        document.getElementById("gasto-form").reset();
    } catch (err) {
        console.error('Error al registrar el gasto:', err);
        const alertMessage = document.getElementById("alert-message");
        alertMessage.textContent = "Error al registrar el gasto";
        alertMessage.className = "alert alert-danger";
        document.getElementById("alert-container").classList.remove("d-none");
    }
}

document.getElementById("gasto-form").addEventListener('submit', regisGasto);
