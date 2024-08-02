// Variables globales
let formId, listId, recordId;
let fieldMappings = {};

// Fonction pour associer un formulaire à une liste et un enregistrement
function associateForm(fId, lId, rId = null) {
    formId = fId;
    listId = lId;
    recordId = rId;

    console.log('associateForm called', { formId, listId, recordId }); // Message de débogage

    // Si un ID d'enregistrement est fourni, récupérer les données de l'enregistrement
    if (recordId) {
        fetch(`/api/Form/getRecord/${listId}/${recordId}`)
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

// Fonction pour associer un champ de sélection unique à une colonne d'une autre liste
function associateSelectField(fieldId, refListId, refColumn, displayColumn, targetColumnName) {
    fieldMappings[fieldId] = {
        type: 'select',
        refList: refListId,
        refColumn: refColumn,
        displayColumn: displayColumn,
        targetColumn: targetColumnName
    };
    console.log('associateSelectField called', { fieldId, refListId, refColumn, displayColumn, targetColumnName }); // Message de débogage

    // Récupérer les options de la liste et les ajouter au champ de sélection
    fetch(`/api/Form/getOptions/${refListId}`)
        .then(response => {
            if (!response.ok) {
                return response.text().then(text => { throw new Error(text); });
            }
            return response.json();
        })
        .then(data => {
            console.log('Options récupérées:', data); // Message de débogage
            const selectField = document.getElementById(fieldId);
            data.forEach(option => {
                const optionElement = document.createElement('option');
                optionElement.value = option[refColumn];
                optionElement.text = option[displayColumn];
                selectField.add(optionElement);
            });

            // Ajouter un événement pour changer la couleur de fond en vert lorsqu'une option est sélectionnée
            selectField.addEventListener('change', function () {
                for (let i = 0; i < selectField.options.length; i++) {
                    if (selectField.options[i].selected) {
                        selectField.options[i].style.backgroundColor = 'lightgreen';
                    } else {
                        selectField.options[i].style.backgroundColor = '';
                    }
                }
            });
        })
        .catch(error => {
            console.error('Erreur lors de la récupération des options:', error); // Message de débogage
        });
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
        } else if (mapping.type === 'select') {
            // Pour les champs de sélection, récupérer la valeur sélectionnée
            const selectedOption = field.value;
            console.log('Selected option for field', fieldId, selectedOption); // Log de l'option sélectionnée
            formData[mapping.targetColumn] = selectedOption;
        }
    }

    console.log('formData avant envoi:', formData); // Message de débogage

    const requestOptions = {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ ListId: listId, RecordId: recordId, FormData: formData })
    };

    // Envoyer les données du formulaire au serveur
    fetch('/api/Form/saveRecord', requestOptions)
        .then(response => {
            if (!response.ok) {
                return response.text().then(text => { throw new Error(text); });
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
