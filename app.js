document.addEventListener('DOMContentLoaded', () => {
    // Function to fetch the README content from GitHub
    async function fetchReadme() {
        const url = 'https://api.github.com/repos/seerge/g-helper/readme';

        try {
            const response = await fetch(url, {
                headers: {
                    'Accept': 'application/vnd.github.v3.raw+json'
                }
            });

            if (!response.ok) {
                throw new Error('Failed to fetch README.');
            }

            const readme = await response.text();
            console.log('README fetched successfully.');

            const md = window.markdownit().use(window.markdownitEmoji);

            document.getElementById('readme-content').innerHTML = md.render(readme);
        } catch (error) {
            console.error('Error:', error);
        }
    }

    fetchReadme();
});
