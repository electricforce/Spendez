const idUsuario = localStorage.getItem('idUsuario');
const apiEndpointPresupuestos = `http://localhost:5004/VisualizarPresupuestos?idCliente=${idUsuario}`;
const apiEndpointCategorias = "http://localhost:5004/ObtenerCategorias_";

async function cargarPresupuestos() {
  try {
    const response = await fetch(apiEndpointPresupuestos);
    if (!response.ok) throw new Error("Error al obtener los presupuestos.");
    const data = await response.json();
    const presupuestos = data || [];
    const tbody = document.querySelector("#presupuestos-table tbody");
    tbody.innerHTML = '';

    for (const presupuesto of presupuestos) {
      const tr = document.createElement('tr');
      const tdNombre = document.createElement('td');
      tdNombre.textContent = presupuesto.nombrePresupuesto || 'Sin nombre'; 
      tr.appendChild(tdNombre);

      const tdMonto = document.createElement('td');
      tdMonto.textContent = `$${presupuesto.montoPresupuesto || 0}`;
      tr.appendChild(tdMonto);

      const tddiferencia = document.createElement('td');
      tddiferencia.textContent = `$${presupuesto.diferencia || 0}`;
      tr.appendChild(tddiferencia);

      const tdCategorias = document.createElement('td');
      const categorias = await obtenerCategorias(presupuesto.idPresupuesto);
      if (categorias && categorias.length > 0) {
        categorias.forEach(categoria => {
          const button = document.createElement('button');
          button.classList.add('btnCategoria');
          button.textContent = categoria.nombre_Cat;
          button.onclick = () => {
            const url = `gato.html?idCategoria=${categoria.iD_Categoria}`;
            window.location.href = url;
          };
          tdCategorias.appendChild(button);
        });
      }
      tr.appendChild(tdCategorias);
      tbody.appendChild(tr);
    }
  } catch (error) {
    console.error("Error al cargar los presupuestos:", error);
    alert("Error al cargar los presupuestos.");
  }
}

async function obtenerCategorias(idPresupuesto) {
  try {
    const response = await fetch(`${apiEndpointCategorias}?idPresupuesto=${idPresupuesto}`);
    if (!response.ok) throw new Error("Error al obtener las categorías.");
    const data = await response.json();
    return data || [];
  } catch (error) {
    console.error("Error al obtener las categorías:", error);
    return [];
  }
}

let selectedCategory = '';

function setCategory(idCategoria, categoryName) {
  const expenseCategoryInput = document.getElementById('expenseCategory');
  selectedCategory = idCategoria;
  expenseCategoryInput.value = categoryName;
  addExpense();
}

function addExpense() {
  const modal = document.getElementById('expenseModal');
  const form = document.getElementById('expenseForm');
  const expenseAmount = document.getElementById('expenseAmount');
  const expenseDescription = document.getElementById('expenseDescription');

  expenseAmount.value = '';
  expenseDescription.value = '';
  modal.style.display = "block";

  form.onsubmit = async function (event) {
    event.preventDefault();

    if (!expenseAmount.value || !expenseDescription.value || !selectedCategory) {
      alert("Por favor, complete todos los campos.");
      return;
    }

    const gastoData = new FormData();
    gastoData.append('idCat', selectedCategory);
    gastoData.append('descripcion', expenseDescription.value);
    gastoData.append('monto', expenseAmount.value);
    gastoData.append('fecha', new Date().toISOString());
    gastoData.append('idUsuario', 3);

    try {
      const response = await fetch(apiEndpointAgregarGasto, {
        method: 'POST',
        body: gastoData
      });

      const result = await response.json();
      if (response.ok) {
        alert(result.Mensaje);
        closeModal();
      } else {
        alert(result.Mensaje || "Error al agregar el gasto.");
      }
    } catch (error) {
      console.error("Error al agregar el gasto:", error);
      alert("Error al agregar el gasto.");
    }
  };
}

async function cargarCategorias() {
  const selectCategory = document.getElementById('expenseCategory');
  selectCategory.innerHTML = '';

  try {
    const response = await fetch(apiEndpointCategorias);
    if (!response.ok) throw new Error("Error al obtener las categorías.");
    const categorias = await response.json();
    if (categorias && categorias.length > 0) {
      categorias.forEach(categoria => {
        const option = document.createElement('option');
        option.value = categoria.iD_Categoria;
        option.textContent = categoria.nombre_Cat;
        selectCategory.appendChild(option);
      });
    }
  } catch (error) {
    console.error("Error al cargar las categorías:", error);
  }
}

document.addEventListener('DOMContentLoaded', () => {
  cargarPresupuestos();
});
