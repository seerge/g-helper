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
            console.log('Raw README content:', readme);

            // Initialize Markdown-it with HTML rendering and links enabled
            const md = window.markdownit({
                html: true, 
                linkify: true 
            }).use(window.markdownitEmoji);

            // Render Markdown to HTML
            let renderedContent = md.render(readme);

            // Insert the sanitized HTML into the page
            document.getElementById('readme-content').innerHTML = renderedContent;
        } catch (error) {
            console.error('Error:', error);
        }
    }

    fetchReadme();
});
