# Directory Diff Tool

A cross-platform directory comparison tool built with C# and .NET that helps identify differences between two directories.

## Features

- Compare files across two directories
- Identify files that exist in only one directory
- Compare file sizes and modification dates
- Display detailed comparison results
- Works on macOS, Windows, and Linux

## Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download) 6.0 or later
- Any text editor (Visual Studio Code recommended)

## Getting Started

### Installation

1. Clone this repository:
   ```
   git clone https://github.com/yourusername/DirectoryDiff.git
   cd DirectoryDiff
   ```

2. Build the application:
   ```
   dotnet build
   ```

### Usage

Run the application with two directory paths as arguments:

```
dotnet run /path/to/first/directory /path/to/second/directory
```

Example:
```
dotnet run /Users/me/Documents/ProjectA /Users/me/Documents/ProjectB
```

## How It Works

The tool compares directories using the following process:

1. Lists all files in both directories
2. Identifies files that exist in only one directory
3. For files that exist in both directories:
   - Compares file sizes
   - Compares last modified dates
4. Generates a report showing all differences

## Project Structure

- `Program.cs`: Entry point and user interface logic
- `DirectoryComparer.cs`: Handles directory comparison logic
- `FileComparer.cs`: Handles individual file comparison
- `ComparisonResult.cs`: Data structures for storing comparison results

## Roadmap

- [ ] Add recursive directory comparison
- [ ] Implement file content comparison for text files
- [ ] Add file pattern ignore options (e.g., ignore .git folder)
- [ ] Create a GUI interface with Avalonia UI
- [ ] Add comparison report export options (HTML, JSON)

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

This project is licensed under the MIT License - see the LICENSE file for details.