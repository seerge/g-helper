// Function to fetch the README content from GitHub
async function fetchReadme() {
    const url = 'https://api.github.com/repos/seerge/g-helper/readme';

    try {
        const response = await fetch(url, {
            headers: {
                'Accept': 'application/vnd.github.v3.raw+json'
            }
        });

        // Throw an error if the response is not okay
        if (!response.ok) {
            throw new Error('Failed to fetch README.');
        }

        // Read the text from the response
        const readme = await response.text();

        // Inject the README content into the HTML, converting markdown to HTML using marked.js
        document.getElementById('readme-content').innerHTML = marked.parse(readme);
    } catch (error) {
        // Log any errors to the console
        console.error(error);
    }
}

// Add an event listener to trigger the fetchReadme function when the page loads
document.addEventListener('DOMContentLoaded', fetchReadme);

