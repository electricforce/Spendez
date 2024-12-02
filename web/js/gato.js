
const idUsuario = localStorage.getItem('idUsuario'); // Obtener el id del usuario del localStorage

const urlParams = new URLSearchParams(window.location.search);
const idCategoria = urlParams.get('idCategoria');  


async function regisGasto(event) {
  event.preventDefault();

  const descripcion = document.getElementById("descripcion").value;
  const monto = document.getElementById("monto").value;
  const fecha = document.getElementById("fecha").value;
  
  const url = "http://localhost:5004/AgregarGasto";

  const formData = new FormData();
  formData.append('idCat', idCategoria);
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
