const fs = require('fs');
const path = require('path');

// Find all maze txt files
const mazeDir = path.join(__dirname, 'MazeGuy/MazeGuy/bin/x86');
const files = fs.readdirSync(mazeDir).filter(f => f.endsWith('.txt') && f.startsWith('Maze'));

// Sort files by level number
const levelFiles = [];
for (const file of files) {
    const match = file.match(/Maze\s*(\d*)\.txt/i);
    if (match) {
        const levelNum = match[1] === '' ? 1 : parseInt(match[1]);
        // Skip "old" files
        if (!file.includes('old') && !file.includes('sam') && !file.includes('jack')) {
            levelFiles.push({ file, levelNum });
        }
    }
}

// Also include sam and jack as bonus levels
for (const file of files) {
    if (file.includes('sam')) {
        levelFiles.push({ file, levelNum: 100 }); // Sam's level
    } else if (file.includes('jack')) {
        levelFiles.push({ file, levelNum: 101 }); // Jack's level
    }
}

levelFiles.sort((a, b) => a.levelNum - b.levelNum);

let output = `using System.Collections.Generic;

namespace MazeGuy
{
    public partial class Game1
    {
        // Embedded levels - stored as string arrays
        private static readonly Dictionary<int, string[]> Levels = new Dictionary<int, string[]>
        {
`;

for (let i = 0; i < levelFiles.length; i++) {
    const { file, levelNum } = levelFiles[i];
    const filePath = path.join(mazeDir, file);
    const content = fs.readFileSync(filePath, 'utf8');
    const lines = content.split(/\r?\n/);

    // Remove trailing empty lines
    while (lines.length > 0 && lines[lines.length - 1].trim() === '') {
        lines.pop();
    }

    output += `            { ${levelNum}, new string[] {\n`;

    for (let j = 0; j < lines.length; j++) {
        // Escape backslashes and quotes in the line
        const escapedLine = lines[j]
            .replace(/\\/g, '\\\\')
            .replace(/"/g, '\\"');

        const comma = j < lines.length - 1 ? ',' : '';
        output += `                "${escapedLine}"${comma}\n`;
    }

    const trailingComma = i < levelFiles.length - 1 ? ',' : '';
    output += `            }}${trailingComma}\n`;
}

output += `        };
    }
}
`;

// Write to the partial class file
const outputPath = path.join(__dirname, 'MazeGuy.Bridge/Levels.cs');
fs.writeFileSync(outputPath, output);

console.log(`Written to ${outputPath}`);
console.log('\nProcessed files:');
for (const { file, levelNum } of levelFiles) {
    console.log(`  Level ${levelNum}: ${file}`);
}
