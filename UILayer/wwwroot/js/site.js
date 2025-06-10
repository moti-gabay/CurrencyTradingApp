// Function to format numbers with commas
function formatNumber(num) {
    if (typeof num === 'number') {
        return num.toLocaleString();
    }
    return num; // Return as is if not a number
}

// Global flag to track simulation state
let isSimulationRunning = false; // Initialize to false

// SignalR connection setup
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/tradehub") // Ensure this matches your SignalR Hub path in Program.cs
    .withAutomaticReconnect()
    .build();

// --- Event Listeners for SignalR Updates ---

// Listener for currency pair updates (now receives a LIST of pairs)
connection.on("ReceiveCurrencyPairUpdate", (currencyPairs) => {
    // console.log("SignalR: Received currency pair updates (list):", currencyPairs); // Uncomment for debugging
    if (Array.isArray(currencyPairs)) {
        currencyPairs.forEach(pair => {
            updateCurrencyPairCard(pair);
            updateCurrencyPairTableRow(pair);
        });
    } else {
        // This case should ideally not happen if the backend always sends an array.
        // It indicates an unexpected data format.
        console.warn("SignalR: Expected an array for ReceiveCurrencyPairUpdate, but received a single object or invalid type:", currencyPairs);
        // Attempt to update as a single object as a fallback, but investigate why it's not an array
        updateCurrencyPairCard(currencyPairs);
        updateCurrencyPairTableRow(currencyPairs);
    }
});

// Listener for dashboard data updates (receives a full DashboardData object directly)
connection.on("ReceiveDashboardData", (dashboardData) => {
    // console.log("SignalR: Received full dashboard data update:", dashboardData); // Uncomment for debugging
    // The dashboardData object itself contains the summary data, no need for dashboardData.dashboardSummary
    updateDashboardSummary(dashboardData); // Pass the entire dashboardData object
});

// --- UI Update Functions ---

// Updates an individual currency pair card
function updateCurrencyPairCard(data) {
    const pairId = data.originalId; // OriginalId serves as the unique identifier for the currency pair in HTML
    if (!pairId) {
        console.error("updateCurrencyPairCard: Missing originalId in data:", data);
        return;
    }
    // console.log(`Attempting to update card for ID: card-${pairId}`); // Uncomment for debugging (note: card- prefix in log)

    const rateElement = document.getElementById(`card-rate-${pairId}`);
    const changeElement = document.getElementById(`card-change-${pairId}`);
    const minElement = document.getElementById(`card-min-${pairId}`);
    const maxElement = document.getElementById(`card-max-${pairId}`);
    const volumeElement = document.getElementById(`card-volume-${pairId}`);
    const trendIconElement = document.getElementById(`card-trend-icon-${pairId}`);

    // Update Current Rate
    if (rateElement && data.currentRate !== undefined) {
        rateElement.textContent = data.currentRate.toFixed(4);
    } else if (!rateElement) {
        // Log the actual ID being searched for, not just the base pairId
        console.warn(`Card rate element with ID 'card-rate-${pairId}' not found!`);
    }

    // Update Change Percentage and apply color
    if (changeElement && data.changePercentage !== undefined) {
        changeElement.textContent = `${(data.changePercentage >= 0 ? "+" : "") + data.changePercentage.toFixed(2)}%`;
        changeElement.classList.remove('up', 'down'); // Remove existing classes
        if (data.changePercentage > 0) {
            changeElement.classList.add('up');
        } else if (data.changePercentage < 0) {
            changeElement.classList.add('down');
        }
    } else if (!changeElement) {
        console.warn(`Card change element with ID 'card-change-${pairId}' not found!`);
    }

    // Update Min Value
    if (minElement && data.minValue !== undefined) {
        minElement.textContent = data.minValue.toFixed(4);
    } else if (!minElement) { console.warn(`Card min element with ID 'card-min-${pairId}' not found!`); }

    // Update Max Value
    if (maxElement && data.maxValue !== undefined) {
        maxElement.textContent = data.maxValue.toFixed(4);
    } else if (!maxElement) { console.warn(`Card max element with ID 'card-max-${pairId}' not found!`); }

    // Update Volume
    if (volumeElement && data.volume !== undefined) {
        volumeElement.textContent = formatNumber(data.volume);
    } else if (!volumeElement) { console.warn(`Card volume element with ID 'card-volume-${pairId}' not found!`); }

    // Update Trend Icon (assuming TradeTrend enum: 0 = Up, 1 = Down, 2 = Stable)
    // We only modify classes for FontAwesome icons, not textContent
    if (trendIconElement && data.trend !== undefined) {
        trendIconElement.classList.remove('fa-arrow-trend-up', 'fa-arrow-trend-down', 'fa-minus', 'up', 'down', 'stable'); // Clear all related classes

        if (data.trend === 0) { // Up
            trendIconElement.classList.add('fa-arrow-trend-up', 'up'); // Add FontAwesome class and custom 'up' class
        } else if (data.trend === 1) { // Down
            trendIconElement.classList.add('fa-arrow-trend-down', 'down'); // Add FontAwesome class and custom 'down' class
        } else if (data.trend === 2) { // Stable
            trendIconElement.classList.add('fa-minus', 'stable'); // Add FontAwesome class for minus and custom 'stable' class
        } else {
            console.warn(`Unexpected trend value for ${pairId}: ${data.trend}. Defaulting to stable icon.`);
            trendIconElement.classList.add('fa-minus', 'stable');
        }
    } else if (!trendIconElement) { console.warn(`Card trend icon element with ID 'card-trend-icon-${pairId}' not found!`); }
}

// Updates a row in the "Live Currency Pairs" table
function updateCurrencyPairTableRow(data) {
    const pairId = data.originalId;
    if (!pairId) {
        console.error("updateCurrencyPairTableRow: Missing originalId in data:", data);
        return;
    }
    const rowElement = document.getElementById(`row-${pairId}`); // Now uses OriginalId
    // console.log(`Attempting to update table row for ID: row-${pairId}`); // Uncomment for debugging

    if (rowElement) {
        // Update Rate
        const rateCell = rowElement.querySelector('.current-rate-col');
        if (rateCell && data.currentRate !== undefined) {
            rateCell.textContent = data.currentRate.toFixed(4);
        }

        // Update Change Percentage and apply color
        const changeCell = rowElement.querySelector('.change-col');
        if (changeCell && data.changePercentage !== undefined) {
            changeCell.textContent = `${(data.changePercentage >= 0 ? "+" : "") + data.changePercentage.toFixed(2)}%`;
            changeCell.classList.remove('up', 'down');
            if (data.changePercentage > 0) {
                changeCell.classList.add('up');
            } else if (data.changePercentage < 0) {
                changeCell.classList.add('down');
            }
        }

        // Update Min Value
        const minCell = rowElement.querySelector('.min-value-col');
        if (minCell && data.minValue !== undefined) {
            minCell.textContent = data.minValue.toFixed(4);
        }

        // Update Max Value
        const maxCell = rowElement.querySelector('.max-value-col');
        if (maxCell && data.maxValue !== undefined) {
            maxCell.textContent = data.maxValue.toFixed(4);
        }

        // Update Volume
        const volumeCell = rowElement.querySelector('.volume-col');
        if (volumeCell && data.volume !== undefined) {
            volumeCell.textContent = formatNumber(data.volume);
        }

        // Update Trend Icon (assuming TradeTrend enum: 0 = Up, 1 = Down, 2 = Stable)
        // This targets the <i> element directly which has the 'trend-icon' class
        const trendIcon = rowElement.querySelector('.trend-icon');
        if (trendIcon && data.trend !== undefined) {
            trendIcon.classList.remove('fa-arrow-trend-up', 'fa-arrow-trend-down', 'fa-minus', 'up', 'down', 'stable'); // Clear all related classes

            if (data.trend === 0) { // Up
                trendIcon.classList.add('fa-arrow-trend-up', 'up');
            } else if (data.trend === 1) { // Down
                trendIcon.classList.add('fa-arrow-trend-down', 'down');
            } else if (data.trend === 2) { // Stable
                trendIcon.classList.add('fa-minus', 'stable');
            } else {
                console.warn(`Unexpected trend value for ${pairId} in table: ${data.trend}. Defaulting to stable icon.`);
                trendIcon.classList.add('fa-minus', 'stable');
            }
        }

        // Update Last Update Time
        const lastUpdateCell = rowElement.querySelector('.last-update-col');
        if (lastUpdateCell && data.lastUpdate !== undefined) {
            lastUpdateCell.textContent = new Date(data.lastUpdate).toLocaleTimeString('he-IL', { hour: '2-digit', minute: '2-digit', second: '2-digit' });
        }
        // console.log(`- Updated table row for ${pairId}.`);
    } else {
        console.warn(`Table row with ID 'row-${pairId}' not found!`);
    }
}

// Updates the top dashboard summary cards
function updateDashboardSummary(data) {
    // console.log("Attempting to update dashboard summary with data:", data); // Uncomment for debugging
    const totalVolumeElement = document.getElementById('total-volume');
    const avgChangeElement = document.getElementById('average-change');
    const totalActivePairsElement = document.getElementById('active-pairs');
    const lastUpdatedElement = document.getElementById('last-updated');

    // Update Total Volume
    if (totalVolumeElement && data.totalVolume !== undefined) {
        totalVolumeElement.textContent = formatNumber(data.totalVolume);
    } else if (!totalVolumeElement) { console.warn("Summary element 'total-volume' not found!"); }

    // Update Average Change Percentage
    if (avgChangeElement && data.averageChangePercentage !== undefined) {
        avgChangeElement.textContent = `${(data.averageChangePercentage >= 0 ? "+" : "") + data.averageChangePercentage.toFixed(2)}%`;
        avgChangeElement.classList.remove('up', 'down');
        if (data.averageChangePercentage > 0) {
            avgChangeElement.classList.add('up');
        } else if (data.averageChangePercentage < 0) {
            avgChangeElement.classList.add('down');
        }
    } else if (!avgChangeElement) { console.warn("Summary element 'average-change' not found!"); }

    // Update Total Active Pairs
    if (totalActivePairsElement && data.totalActivePairs !== undefined) {
        totalActivePairsElement.textContent = data.totalActivePairs;
    } else if (!totalActivePairsElement) { console.warn("Summary element 'active-pairs' not found!"); }

    // Update Last Updated Time
    if (lastUpdatedElement && data.lastUpdated !== undefined) {
        const date = new Date(data.lastUpdated);
        lastUpdatedElement.textContent = date.toLocaleTimeString('he-IL', { hour: '2-digit', minute: '2-digit', second: '2-digit' });
    } else if (!lastUpdatedElement) { console.warn("Summary element 'last-updated' not found!"); }

    // console.log("Dashboard summary update attempted."); // Uncomment for debugging
}

// --- Button Event Handlers ---
const startSimulationBtn = document.getElementById('startSimulationBtn');
const refreshBtn = document.getElementById('refreshBtn');

if (startSimulationBtn) {
    startSimulationBtn.addEventListener('click', async () => {
        if (!isSimulationRunning) {
            // Start simulation
            try {
                const response = await fetch('/api/simulation/start', { method: 'POST' });
                if (response.ok) {
                    console.log('Simulation start request sent. Simulation is running.');
                    isSimulationRunning = true;
                    startSimulationBtn.innerHTML = '<i class="fas fa-stop"></i> Stop Simulation';
                    startSimulationBtn.classList.remove('btn-success');
                    startSimulationBtn.classList.add('btn-danger');
                    if (refreshBtn) {
                        refreshBtn.disabled = true; // Disable refresh button
                    }
                } else {
                    console.error('Failed to send simulation start request:', response.status, response.statusText);
                }
            } catch (error) {
                console.error('Error sending simulation start request:', error);
            }
        } else {
            // Stop simulation
            try {
                const response = await fetch('/api/simulation/stop', { method: 'POST' });
                if (response.ok) {
                    console.log('Simulation stop request sent. Simulation is stopped.');
                    isSimulationRunning = false;
                    startSimulationBtn.innerHTML = '<i class="fas fa-play"></i> Start Simulation';
                    startSimulationBtn.classList.remove('btn-danger');
                    startSimulationBtn.classList.add('btn-success');
                    if (refreshBtn) {
                        refreshBtn.disabled = false; // Enable refresh button
                    }
                } else {
                    console.error('Failed to send simulation stop request:', response.status, response.statusText);
                }
            } catch (error) {
                console.error('Error sending simulation stop request:', error);
            }
        }
    });
}

if (refreshBtn) {
    refreshBtn.addEventListener('click', () => {
        // Refresh only if simulation is not running
        if (!isSimulationRunning) {
            fetch('/api/data/current')
                .then(response => {
                    if (!response.ok) {
                        throw new Error(`HTTP error! status: ${response.status}`);
                    }
                    return response.json();
                })
                .then(data => {
                    if (data.currencyPairs && Array.isArray(data.currencyPairs)) {
                        data.currencyPairs.forEach(pair => {
                            updateCurrencyPairCard(pair);
                            updateCurrencyPairTableRow(pair);
                        });
                    } else {
                        console.warn('Initial data fetch: currencyPairs not found or not an array.', data);
                    }
                    // Pass the entire dashboardData object to updateDashboardSummary
                    if (data) { // data is the DashboardData object
                        updateDashboardSummary(data);
                    } else {
                        console.warn('Initial data fetch: DashboardData object not found.', data);
                    }
                    console.log('Dashboard refreshed with current data.');
                })
                .catch(error => console.error('Error refreshing data:', error));
        } else {
            console.log("Refresh disabled while simulation is running.");
        }
    });
}

// --- Initial SignalR Connection & Data Load ---
connection.start()
    .then(() => {
        console.log("SignalR Connected!");
        // Fetch initial data after SignalR connection is established
        fetch('/api/data/current')
            .then(response => {
                if (!response.ok) {
                    throw new Error(`HTTP error! status: ${response.status}`);
                }
                return response.json();
            })
            .then(data => {
                // data is the DashboardData object from the API
                if (data.currencyPairs && Array.isArray(data.currencyPairs)) {
                    data.currencyPairs.forEach(pair => {
                        updateCurrencyPairCard(pair);
                        updateCurrencyPairTableRow(pair);
                    });
                } else {
                    console.warn('Initial load: currencyPairs not found or not an array.', data);
                }
                // Pass the entire dashboardData object to updateDashboardSummary
                if (data) { // data is the DashboardData object
                    updateDashboardSummary(data);
                } else {
                    console.warn('Initial load: DashboardData object not found.', data);
                }
                console.log('Initial dashboard data loaded.');
            })
            .catch(error => console.error('Error loading initial dashboard data:', error));
    })
    .catch(err => console.error("SignalR connection error: ", err));

// Initial check for simulation status (if it persists across refreshes)
// This might require an API endpoint to check current simulation status.
// For now, assume it's stopped on page load.