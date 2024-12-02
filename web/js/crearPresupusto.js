document.getElementById('agregarPresupuesto').addEventListener('click', () => {
    document.getElementById('formularioPresupuesto').style.display = 'block';
});

document.getElementById('formCrearPresupuesto').addEventListener('submit', async (e) => {
    e.preventDefault();

    const nombrePresupuesto = document.getElementById('nombrePresupuesto').value;
    const montoPresupuesto = document.getElementById('montoPresupuesto').value;
    const fechaInicio = document.getElementById('fechaInicio').value;
    const fechaFin = document.getElementById('fechaFin').value;
    const periodo = document.getElementById('periodo').value;

    const idUsuario = localStorage.getItem('idUsuario'); 

    if (!idUsuario) {
        alert('No se ha encontrado el ID del cliente en el almacenamiento local.');
        return;
    }

    const presupuesto = {
        NombrePresupuesto: nombrePresupuesto,
        MontoPresupuesto: montoPresupuesto,
        FechaInicio: fechaInicio,
        FechaFin: fechaFin,
        Periodo: periodo,
        IDCliente: idUsuario 
    };

    try {
        const response = await fetch('http://localhost:5004/IngresarPresupuesto', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(presupuesto)
        });

        const data = await response.json();
        if (response.ok) {
            alert('Presupuesto creado con éxito!');
            location.reload();
        } else {
            throw new Error(data.Message || 'Error al crear el presupuesto.');
        }
    } catch (error) {
        console.error('Error al crear el presupuesto:', error);
    }
});
