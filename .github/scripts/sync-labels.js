const fs = require("fs");
const path = ".github/labels.json";

// Check if the file exists and is readable
if (!fs.existsSync(path)) {
    throw new Error(`File not found: ${path}`);
}

const labels = JSON.parse(fs.readFileSync(path, "utf8"));

// Function to synchronise labels
async function syncLabels(github, context) {
    try {
        // Fetch existing labels from the repository
        const existingLabelsResponse = await github.rest.issues.listLabelsForRepo({
            owner: context.repo.owner,
            repo: context.repo.repo
        });
        const existingLabels = existingLabelsResponse.data;

        console.log(`Found ${existingLabels.length} existing labels.`);

        // Convert existing labels to a map for quick lookup
        const existingLabelsMap = new Map(
            existingLabels.map(label => [label.name, label])
        );

        // Process each label in the JSON file
        for (const label of labels) {
            if (existingLabelsMap.has(label.name)) {
                // The label exists, so check if it needs updating
                const existingLabel = existingLabelsMap.get(label.name);

                // Update label only if colour or description differs
                if (existingLabel.color !== label.color || existingLabel.description !== label.description) {
                    console.log(`Updating label: ${label.name}`);
                    try {
                        await github.rest.issues.updateLabel({
                            owner: context.repo.owner,
                            repo: context.repo.repo,
                            name: label.name,
                            color: label.color,
                            description: label.description
                        });
                    } catch (error) {
                        console.error(`Failed to update label: ${label.name}`, error);
                    }
                } else {
                    console.log(`No changes for label: ${label.name}`);
                }

                // Remove the label from the map to track labels that exist in the repo but not in the JSON file
                existingLabelsMap.delete(label.name);
            } else {
                // The label does not exist, so create it
                console.log(`Creating label: ${label.name}`);
                try {
                    await github.rest.issues.createLabel({
                        owner: context.repo.owner,
                        repo: context.repo.repo,
                        name: label.name,
                        color: label.color,
                        description: label.description
                    });
                } catch (error) {
                    console.error(`Failed to create label: ${label.name}`, error);
                }
            }
        }

        // Delete labels that exist in the repository but not in the JSON file
        for (const [labelName, label] of existingLabelsMap.entries()) {
            console.log(`Deleting label: ${labelName}`);
            try {
                await github.rest.issues.deleteLabel({
                    owner: context.repo.owner,
                    repo: context.repo.repo,
                    name: labelName
                });
            } catch (error) {
                console.error(`Failed to delete label: ${labelName}`, error);
            }
        }

    } catch (error) {
        console.error("Error synchronising labels:", error);
        throw error;
    }
}

module.exports = async ({ github, context }) => {
    try {
        // Synchronise labels with the JSON file
        await syncLabels(github, context);
        console.log("Labels synchronised successfully.");
    } catch (error) {
        console.error("Error updating labels:", error);
    }
};
