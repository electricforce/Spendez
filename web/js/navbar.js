async function loadNavbar() {
    fetch('navbar.html')
        .then(response => {
            if (!response.ok) {
                console.log("Error: " + response.status);
                throw new Error("Error: " + response.status);
            }
            return response.text();
        })
        .then(data => {
            document.getElementById("navbar-container").innerHTML = data;
        })
        .catch(error => console.log("Error al cargar el navbar:", error));
}
window.onload=loadNavbar;