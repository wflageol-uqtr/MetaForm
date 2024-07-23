// Variables globales
let formId, listName, recordId;
let fieldMappings = {};

// Fonction pour associer un formulaire à une liste et un enregistrement
function associateForm(fId, lName, rId = null) {
    formId = fId;
    listName = lName;
    recordId = rId;

    console.log('associateForm called', { formId, listName, recordId }); // Debug message

    if (recordId) {
        fetch(`/api/Form/getRecord/${recordId}`)
            .then(response => {
                if (!response.ok) {
                    throw new Error('Network response was not ok');
                }
                return response.json();
            })
            .then(data => {
                console.log('Data fetched:', data); // Debug message
                for (let key in data) {
                    const field = document.getElementById(key);
                    if (field) {
                        field.value = data[key];
                    }
                }
            })
            .catch(error => {
                console.error('Error fetching record:', error); // Debug message
            });
    }
}

// Fonction pour associer un champ texte à une colonne de la liste
function associateTextField(fieldId, columnName) {
    fieldMappings[fieldId] = {
        type: 'text',
        column: columnName
    };
    console.log('associateTextField called', { fieldId, columnName }); // Debug message
}

// Fonction pour associer un champ de sélection multiple à une colonne d'une autre liste
function associateMultiSelectField(fieldId, refList, refColumn) {
    fieldMappings[fieldId] = {
        type: 'multi-select',
        refList: refList,
        refColumn: refColumn
    };
    console.log('associateMultiSelectField called', { fieldId, refList, refColumn }); // Debug message
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
    console.log('saveCurrentRecord called'); // Debug message

    const formData = {};
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

    console.log('formData:', formData); // Debug message

    const requestOptions = {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ listName, recordId, formData })
    };

    fetch('/api/Form/saveRecord', requestOptions)
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.json();
        })
        .then(data => {
            console.log('Record saved successfully:', data); // Debug message
            showMessage('Record saved successfully');
        })
        .catch(error => {
            console.error('Error saving record:', error); // Debug message
            showMessage('Error saving record: ' + error.message, true);
        });
}
