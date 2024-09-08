// Changer la couleur de fond lors du chargement de la page
document.body.style.backgroundColor = '#f0f8ff';

// Afficher un message dans la console pour indiquer que le fichier JavaScript a été chargé
console.log('JavaScript file loaded successfully.');

// Afficher une alerte après 3 secondes pour tester l'exécution
setTimeout(() => {
    alert('Bienvenue sur le site des loisirs pour les étudiants!');
}, 3000);

// Ajouter un événement pour surligner la table des activités lorsque la souris passe dessus
const table = document.querySelector('table');
table.addEventListener('mouseover', () => {
    table.style.backgroundColor = 'lightyellow';
});

table.addEventListener('mouseout', () => {
    table.style.backgroundColor = '';
});
