// Variables globales
let formId, listName, recordId;
let fieldMappings = {};

// Fonction pour associer un formulaire à une liste et un enregistrement
function associateForm(fId, lName, rId = null) {
    formId = fId;
    listName = lName;
    recordId = rId;

    console.log('associateForm called', { formId, listName, recordId }); // Message de débogage

    // Si un ID d'enregistrement est fourni, récupérer les données de l'enregistrement
    if (recordId) {
        fetch(`/api/Form/getRecord/${recordId}`)
            .then(response => {
                if (!response.ok) {
                    throw new Error('La réponse du réseau n\'était pas correcte');
                }
                return response.json();
            })
            .then(data => {
                console.log('Données récupérées:', data); // Message de débogage
                // Remplir les champs du formulaire avec les données récupérées
                for (let key in data) {
                    const field = document.getElementById(key);
                    if (field) {
                        field.value = data[key];
                    }
                }
            })
            .catch(error => {
                console.error('Erreur lors de la récupération de l\'enregistrement:', error); // Message de débogage
            });
    }
}

// Fonction pour associer un champ texte à une colonne de la liste
function associateTextField(fieldId, columnName) {
    fieldMappings[fieldId] = {
        type: 'text',
        column: columnName
    };
    console.log('associateTextField called', { fieldId, columnName }); // Message de débogage
}

// Fonction pour associer un champ de sélection multiple à une colonne d'une autre liste
function associateMultiSelectField(fieldId, refList, refColumn) {
    fieldMappings[fieldId] = {
        type: 'multi-select',
        refList: refList,
        refColumn: refColumn
    };
    console.log('associateMultiSelectField called', { fieldId, refList, refColumn }); // Message de débogage
}

// Fonction pour afficher des messages sur le site
function showMessage(message, isError = false) {
    const messageContainer = document.getElementById('messageContainer');
    if (messageContainer) {
        messageContainer.textContent = message;
        messageContainer.style.color = isError ? 'red' : 'green';
        messageContainer.style.display = 'block';
    }
}

// Fonction pour sauvegarder l'enregistrement en cours
function saveCurrentRecord() {
    console.log('saveCurrentRecord called'); // Message de débogage

    const formData = {};
    // Rassembler les données du formulaire en fonction des mappages de champs
    for (let fieldId in fieldMappings) {
        const field = document.getElementById(fieldId);
        const mapping = fieldMappings[fieldId];

        if (mapping.type === 'text') {
            formData[mapping.column] = field.value;
        } else if (mapping.type === 'multi-select') {
            const selectedOptions = Array.from(field.selectedOptions).map(option => option.value);
            formData[mapping.refColumn] = selectedOptions;
        }
    }

    console.log('formData:', formData); // Message de débogage

    const requestOptions = {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ listName, recordId, formData })
    };

    // Envoyer les données du formulaire au serveur
    fetch('/api/Form/saveRecord', requestOptions)
        .then(response => {
            if (!response.ok) {
                throw new Error('La réponse du réseau n\'était pas correcte');
            }
            return response.json();
        })
        .then(data => {
            console.log('Enregistrement sauvegardé avec succès:', data); // Message de débogage
            showMessage('Enregistrement sauvegardé avec succès');
        })
        .catch(error => {
            console.error('Erreur lors de la sauvegarde de l\'enregistrement:', error); // Message de débogage
            showMessage('Erreur lors de la sauvegarde de l\'enregistrement: ' + error.message, true);
        });
}
