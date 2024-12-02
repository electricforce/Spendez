const container = document.getElementById('container');
const registerBtn = document.getElementById('register');
const loginBtn = document.getElementById('loginbtn');

registerBtn.addEventListener('click', () => {
    container.classList.add("active");
});

loginBtn.addEventListener('click', () => {
    container.classList.remove("active");
});

async function loguearUsuario(event) {
    event.preventDefault();

    const emailU = document.getElementById('email').value;
    const passwordU = document.getElementById('password').value;
    const nombre = null;

    const url = `http://localhost:5004/loguearUsuario?emailU=${encodeURIComponent(emailU)}&password=${encodeURIComponent(passwordU)}&nomIniU=${encodeURIComponent(nombre)}`;
    
    try {
        const response = await fetch(url);

        if (!response.ok) {
            const errorData = await response.json();
            console.error('Error de login:', errorData.Mensaje);
            alert('Error de login: ' + errorData.Mensaje);
            return;
        }

        const data = await response.json();
        console.log('Datos recibidos del servidor:', data);

        if (data && data.length > 0) {
            const idUsuario = data[0].idUsuario;
            localStorage.setItem('idUsuario', idUsuario);
            console.log('Login exitoso:', idUsuario);
            alert('Login exitoso');
            window.location.href = 'home.html';
        } else {
            alert('Usuario no encontrado');
        }

    } catch (err) {
        console.error("Error en la comunicación del API", err);
        alert('Ocurrió un error inesperado');
    }
}

document.getElementById("login").addEventListener('submit', loguearUsuario);
