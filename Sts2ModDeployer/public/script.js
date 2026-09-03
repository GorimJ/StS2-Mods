document.addEventListener('DOMContentLoaded', () => {
    const modList = document.getElementById('mod-list');
    const loading = document.getElementById('loading');
    const deployBtn = document.getElementById('deploy-btn');
    const statusMsg = document.getElementById('status-message');

    let modsData = [];

    // Fetch mods on load
    fetch('/api/mods')
        .then(res => res.json())
        .then(data => {
            modsData = data.mods;
            renderMods();
            loading.classList.add('hidden');
            modList.classList.remove('hidden');
            updateDeployBtn();
        })
        .catch(err => {
            loading.innerText = "Error loading mods. Is the server running?";
            console.error(err);
        });

    function renderMods() {
        modList.innerHTML = '';
        modsData.forEach((mod, index) => {
            const card = document.createElement('div');
            card.className = `mod-card ${mod.isDeployed ? 'selected' : ''}`;
            
            card.innerHTML = `
                <div class="mod-info">
                    <h2>${mod.name} <span style="font-size: 0.8rem; color: #7d8590;">${mod.version}</span></h2>
                    <p>${mod.description}</p>
                    <p style="font-size: 0.75rem; margin-top: 4px; color: #4a5057;">ID: ${mod.id} | Author: ${mod.author}</p>
                </div>
                <div class="toggle-switch">
                    <div class="toggle-thumb"></div>
                </div>
            `;

            card.addEventListener('click', () => {
                mod.isDeployed = !mod.isDeployed;
                card.classList.toggle('selected', mod.isDeployed);
                updateDeployBtn();
            });

            modList.appendChild(card);
        });
    }

    function updateDeployBtn() {
        deployBtn.disabled = false;
    }

    deployBtn.addEventListener('click', () => {
        const deployIds = modsData.filter(m => m.isDeployed).map(m => m.id);
        
        deployBtn.disabled = true;
        deployBtn.innerText = "Applying...";
        statusMsg.innerText = "";
        
        fetch('/api/deploy', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ deployIds })
        })
        .then(res => res.json())
        .then(data => {
            if (data.success) {
                statusMsg.innerText = `Deploy successful. ${deployIds.length} mods enabled.`;
                setTimeout(() => { statusMsg.innerText = ''; }, 3000);
            }
        })
        .catch(err => {
            statusMsg.innerText = "Deployment failed.";
            statusMsg.style.color = "var(--danger)";
        })
        .finally(() => {
            deployBtn.disabled = false;
            deployBtn.innerText = "Apply Changes";
        });
    });
});
